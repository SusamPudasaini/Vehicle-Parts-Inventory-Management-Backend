using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    /// Handles staff-facing vehicle registration and management.
    public class VehicleService : IVehicleService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(AppDbContext db, ILogger<VehicleService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<VehicleResponse>> GetAllAsync()
        {
            return await _db.Vehicles
                .Include(v => v.Customer)
                .AsNoTracking()
                .OrderBy(v => v.VehicleNumber)
                .Select(v => MapToResponse(v))
                .ToListAsync();
        }

        public async Task<List<VehicleResponse>> GetByCustomerIdAsync(int customerId)
        {
            return await _db.Vehicles
                .Include(v => v.Customer)
                .AsNoTracking()
                .Where(v => v.CustomerId == customerId)
                .OrderBy(v => v.VehicleNumber)
                .Select(v => MapToResponse(v))
                .ToListAsync();
        }

        public async Task<VehicleResponse?> GetByIdAsync(int id)
        {
            var vehicle = await _db.Vehicles
                .Include(v => v.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            return vehicle == null ? null : MapToResponse(vehicle);
        }

        public async Task<VehicleResponse> CreateAsync(VehicleCreateRequest request)
        {
            return await CreateForCustomerAsync(request.CustomerId, request);
        }

        public async Task<VehicleResponse> CreateForCustomerAsync(int customerId, VehicleRequest request)
        {
            var customerExists = await _db.Customers.AnyAsync(c => c.Id == customerId);
            if (!customerExists)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");

            var vehicleNumber = request.VehicleNumber.Trim().ToUpper();
            var vehicleExists = await _db.Vehicles.AnyAsync(v => EF.Functions.ILike(v.VehicleNumber, vehicleNumber));
            if (vehicleExists)
                throw new InvalidOperationException($"A vehicle with number '{request.VehicleNumber}' already exists.");

            var vehicle = new Vehicle
            {
                CustomerId = customerId,
                VehicleNumber = vehicleNumber,
                Make = request.Make.Trim(),
                Model = request.Model.Trim(),
                Year = request.Year,
                Color = request.Color.Trim()
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Vehicle '{VehicleNumber}' registered for customer ID {CustomerId}.", vehicle.VehicleNumber, customerId);

            var created = await _db.Vehicles
                .Include(v => v.Customer)
                .AsNoTracking()
                .FirstAsync(v => v.Id == vehicle.Id);

            return MapToResponse(created);
        }

        public async Task<VehicleResponse?> UpdateAsync(int id, VehicleRequest request)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                _logger.LogWarning("Update attempted on non-existent vehicle ID {Id}.", id);
                return null;
            }

            var vehicleNumber = request.VehicleNumber.Trim().ToUpper();
            var vehicleExists = await _db.Vehicles
                .AnyAsync(v => v.Id != id && EF.Functions.ILike(v.VehicleNumber, vehicleNumber));

            if (vehicleExists)
                throw new InvalidOperationException($"A vehicle with number '{request.VehicleNumber}' already exists.");

            vehicle.VehicleNumber = vehicleNumber;
            vehicle.Make = request.Make.Trim();
            vehicle.Model = request.Model.Trim();
            vehicle.Year = request.Year;
            vehicle.Color = request.Color.Trim();

            await _db.SaveChangesAsync();

            _logger.LogInformation("Vehicle ID {Id} updated.", id);

            var updated = await _db.Vehicles
                .Include(v => v.Customer)
                .AsNoTracking()
                .FirstAsync(v => v.Id == id);

            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                _logger.LogWarning("Delete attempted on non-existent vehicle ID {Id}.", id);
                return false;
            }

            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Vehicle ID {Id} deleted.", id);

            return true;
        }

        private static VehicleResponse MapToResponse(Vehicle v) => new()
        {
            Id = v.Id,
            CustomerId = v.CustomerId,
            CustomerName = v.Customer?.FullName ?? string.Empty,
            VehicleNumber = v.VehicleNumber,
            Make = v.Make,
            Model = v.Model,
            Year = v.Year,
            Color = v.Color
        };
    }
}
