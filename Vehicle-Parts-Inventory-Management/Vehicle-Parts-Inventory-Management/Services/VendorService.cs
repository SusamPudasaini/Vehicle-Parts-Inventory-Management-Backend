using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    /// Handles all vendor CRUD operations.
    /// Feature 5: Admin can manage vendor details.
    public class VendorService : IVendorService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<VendorService> _logger;

        public VendorService(AppDbContext db, ILogger<VendorService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<VendorResponse>> GetAllAsync()
        {
            return await _db.Vendors
                .OrderByDescending(v => v.CreatedAt)
                .Select(v => MapToResponse(v))
                .ToListAsync();
        }

        public async Task<VendorResponse?> GetByIdAsync(int id)
        {
            var vendor = await _db.Vendors.FindAsync(id);
            return vendor == null ? null : MapToResponse(vendor);
        }

        public async Task<VendorResponse> CreateAsync(VendorRequest request)
        {
            var vendor = new Vendor
            {
                Name = request.Name,
                ContactPerson = request.ContactPerson,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                CreatedAt = DateTime.UtcNow
            };

            _db.Vendors.Add(vendor);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Vendor '{Name}' created with ID {Id}.", vendor.Name, vendor.Id);

            return MapToResponse(vendor);
        }

        public async Task<VendorResponse?> UpdateAsync(int id, VendorRequest request)
        {
            var vendor = await _db.Vendors.FindAsync(id);
            if (vendor == null)
            {
                _logger.LogWarning("Update attempted on non-existent vendor ID {Id}.", id);
                return null;
            }

            vendor.Name = request.Name;
            vendor.ContactPerson = request.ContactPerson;
            vendor.Email = request.Email;
            vendor.Phone = request.Phone;
            vendor.Address = request.Address;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Vendor ID {Id} updated.", id);

            return MapToResponse(vendor);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vendor = await _db.Vendors.FindAsync(id);
            if (vendor == null)
            {
                _logger.LogWarning("Delete attempted on non-existent vendor ID {Id}.", id);
                return false;
            }

            _db.Vendors.Remove(vendor);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Vendor ID {Id} deleted.", id);

            return true;
        }

        private static VendorResponse MapToResponse(Vendor v) => new()
        {
            Id = v.Id,
            Name = v.Name,
            ContactPerson = v.ContactPerson,
            Email = v.Email,
            Phone = v.Phone,
            Address = v.Address
        };
    }
}
