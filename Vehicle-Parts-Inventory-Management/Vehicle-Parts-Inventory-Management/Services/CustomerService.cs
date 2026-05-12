 
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
            var exists = await _db.Customers.AnyAsync(c => c.Email == request.Email);
            if (exists)
                throw new InvalidOperationException($"A customer with email '{request.Email}' already exists.");

            var customer = new Customer
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                RegisteredAt = DateTime.UtcNow
            };

            // Attach vehicle if provided
            if (request.Vehicle != null)
            {
                customer.Vehicles.Add(new Vehicle
                {
                    VehicleNumber = request.Vehicle.VehicleNumber,
                    Make = request.Vehicle.Make,
                    Model = request.Vehicle.Model,
                    Year = request.Vehicle.Year,
                    Color = request.Vehicle.Color
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
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                _logger.LogWarning("Customer ID {Id} not found.", id);
                return null;
            }

            return MapToResponse(customer);
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
        /// All comparisons are case-insensitive.
        public async Task<List<CustomerResponse>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<CustomerResponse>();

            var lower = query.Trim().ToLower();
            var pattern = $"%{lower}%";

            var results = await _db.Customers
                .Include(c => c.Vehicles)
                .Where(c =>
                    EF.Functions.ILike(c.FullName, pattern) ||
                    EF.Functions.ILike(c.Phone, pattern) ||
                    c.Id.ToString() == lower ||
                    c.Vehicles.Any(v => EF.Functions.ILike(v.VehicleNumber, pattern))
                )
                .OrderBy(c => c.FullName)
                .Select(c => MapToResponse(c))
                .ToListAsync();

            _logger.LogInformation("Search for '{Query}' returned {Count} results.", query, results.Count);

            return results;
        }

        private static CustomerResponse MapToResponse(Customer c) => new()
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
                VehicleNumber = v.VehicleNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Color = v.Color
            }).ToList()
        };
    }
}
