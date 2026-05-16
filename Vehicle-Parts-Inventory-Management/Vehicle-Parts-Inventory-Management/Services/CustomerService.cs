 
using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;


namespace Vehicle_Parts_Inventory_Management.Services
{
    /// Handles customer registration, retrieval, and search.
    /// Feature 6: Staff can register new customers with vehicle details.
    /// Feature 8: Staff can view customer details, history, and vehicle info.
    /// Feature 10: Staff can search customers by vehicle number, phone, ID, or name.
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(AppDbContext db, ILogger<CustomerService> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// Feature 6: Register a new customer along with their vehicle details.
        public async Task<CustomerResponse> RegisterAsync(CustomerCreateRequest request)
        {
            var email = request.Email.Trim().ToLower();
            var vehicleRequests = new List<VehicleRequest>();

            if (request.Vehicle != null)
                vehicleRequests.Add(request.Vehicle);

            if (request.Vehicles?.Count > 0)
                vehicleRequests.AddRange(request.Vehicles);

            var vehicleNumbers = vehicleRequests
                .Select(v => v.VehicleNumber.Trim().ToUpper())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();

            var exists = await _db.Customers.AnyAsync(c => EF.Functions.ILike(c.Email, email));
            if (exists)
                throw new InvalidOperationException($"A customer with email '{request.Email}' already exists.");

            var duplicateInRequest = vehicleNumbers
                .GroupBy(v => v)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateInRequest != null)
                throw new InvalidOperationException($"Vehicle number '{duplicateInRequest.Key}' was entered more than once.");

            if (vehicleNumbers.Count > 0)
            {
                var existingVehicle = await _db.Vehicles
                    .Where(v => vehicleNumbers.Contains(v.VehicleNumber.ToUpper()))
                    .Select(v => v.VehicleNumber)
                    .FirstOrDefaultAsync();

                if (existingVehicle != null)
                    throw new InvalidOperationException($"A vehicle with number '{existingVehicle}' already exists.");
            }

            var customer = new Customer
            {
                FullName = request.FullName.Trim(),
                Email = email,
                Phone = request.Phone.Trim(),
                Address = request.Address.Trim(),
                RegisteredAt = DateTime.UtcNow
            };

            foreach (var vehicle in vehicleRequests)
            {
                customer.Vehicles.Add(new Vehicle
                {
                    VehicleNumber = vehicle.VehicleNumber.Trim().ToUpper(),
                    Make = vehicle.Make.Trim(),
                    Model = vehicle.Model.Trim(),
                    Year = vehicle.Year,
                    Color = vehicle.Color.Trim()
                });
            }

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Customer '{FullName}' registered with ID {Id}.", customer.FullName, customer.Id);

            return MapToResponse(customer);
        }

        /// Feature 8: Get full customer detail including vehicles.
        public async Task<CustomerResponse?> GetByIdAsync(int id)
        {
            var customer = await _db.Customers
                .Include(c => c.Vehicles)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                _logger.LogWarning("Customer ID {Id} not found.", id);
                return null;
            }

            var purchaseHistory = await _db.PurchaseHistories
                .AsNoTracking()
                .Where(p => p.CustomerId == id)
                .OrderByDescending(p => p.PurchasedAt)
                .Select(p => new PurchaseHistoryResponse
                {
                    Id = p.Id,
                    PartName = p.PartName,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    TotalPrice = p.TotalPrice,
                    InvoiceNumber = p.InvoiceNumber,
                    PurchasedAt = p.PurchasedAt
                })
                .ToListAsync();

            var serviceHistory = await _db.ServiceHistories
                .AsNoTracking()
                .Where(s => s.CustomerId == id)
                .OrderByDescending(s => s.ServiceDate)
                .Select(s => new ServiceHistoryResponse
                {
                    Id = s.Id,
                    ServiceType = s.ServiceType,
                    Description = s.Description,
                    VehicleNumber = s.VehicleNumber,
                    Cost = s.Cost,
                    Status = s.Status,
                    ServiceDate = s.ServiceDate
                })
                .ToListAsync();

            return MapToResponse(customer, purchaseHistory, serviceHistory);
        }

        public async Task<List<CustomerResponse>> GetAllAsync()
        {
            var customers = await _db.Customers
                .Include(c => c.Vehicles)
                .AsNoTracking()
                .OrderByDescending(c => c.RegisteredAt)
                .ToListAsync();

            return customers.Select(MapToResponse).ToList();
        }


        /// Feature 10: Search customers by name, phone, ID, or vehicle number.
        /// Case-insensitive search using PostgreSQL ILIKE.
        public async Task<List<CustomerResponse>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<CustomerResponse>();

            var q = query.Trim();
            var pattern = $"%{q}%";

            // If user typed a number, treat it as ID search too
            var isId = int.TryParse(q, out var idValue);

            var customerQuery = _db.Customers
                .Include(c => c.Vehicles)
                .AsNoTracking()
                .Where(c =>
                    (isId && c.Id == idValue) ||
                    EF.Functions.ILike(c.FullName, pattern) ||
                    EF.Functions.ILike(c.Phone, pattern) ||
                    EF.Functions.ILike(c.Email, pattern) ||
                    c.Vehicles.Any(v => EF.Functions.ILike(v.VehicleNumber, pattern))
                )
                .OrderBy(c => c.FullName)
                .Take(50); // Safety limit

            var customers = await customerQuery.ToListAsync();

            _logger.LogInformation("Search for '{Query}' returned {Count} results.", query, customers.Count);

            return customers.Select(MapToResponse).ToList();
        }


        private static CustomerResponse MapToResponse(
            Customer c,
            List<PurchaseHistoryResponse>? purchaseHistory = null,
            List<ServiceHistoryResponse>? serviceHistory = null) => new()
        {
            Id = c.Id,
            FullName = c.FullName,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            RegisteredAt = c.RegisteredAt,
            Vehicles = c.Vehicles.Select(v => new VehicleResponse
            {
                Id = v.Id,
                CustomerId = v.CustomerId,
                VehicleNumber = v.VehicleNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Color = v.Color
            }).ToList(),
            PurchaseHistory = purchaseHistory ?? new List<PurchaseHistoryResponse>(),
            ServiceHistory = serviceHistory ?? new List<ServiceHistoryResponse>()
        };
    }
}
