using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;
using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;

namespace Vehicle_Parts_Inventory_Management.Services
{
    /// <summary>
    /// Feature 3: Admin can perform parts management (add, edit, delete).
    /// Feature 15: Detects low stock parts below threshold.
    /// </summary>
    public class PartService : IPartService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<PartService> _logger;

        public PartService(AppDbContext db, ILogger<PartService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<PartResponse>> GetAllAsync()
        {
            return await _db.Parts
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .Select(p => MapToResponse(p))
                .ToListAsync();
        }

        public async Task<PartResponse?> GetByIdAsync(int id)
        {
            var part = await _db.Parts.FindAsync(id);
            return part == null ? null : MapToResponse(part);
        }

        /// <summary>Feature 15: Returns all parts with stock at or below their threshold.</summary>
        public async Task<List<PartResponse>> GetLowStockAsync()
        {
            return await _db.Parts
                .Where(p => p.StockQuantity <= p.LowStockThreshold && p.IsActive)
                .OrderBy(p => p.StockQuantity)
                .Select(p => MapToResponse(p))
                .ToListAsync();
        }

        public async Task<PartResponse> CreateAsync(PartRequest request)
        {
            var exists = await _db.Parts.AnyAsync(p => p.PartNumber == request.PartNumber);
            if (exists)
                throw new InvalidOperationException($"A part with number '{request.PartNumber}' already exists.");

            var part = new Part
            {
                Name = request.Name,
                PartNumber = request.PartNumber,
                Category = request.Category,
                Description = request.Description,
                SellingPrice = request.SellingPrice,
                PurchasePrice = request.PurchasePrice,
                StockQuantity = request.StockQuantity,
                LowStockThreshold = request.LowStockThreshold,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Parts.Add(part);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Part '{Name}' (#{PartNumber}) created with ID {Id}.", part.Name, part.PartNumber, part.Id);
            return MapToResponse(part);
        }

        public async Task<PartResponse?> UpdateAsync(int id, PartRequest request)
        {
            var part = await _db.Parts.FindAsync(id);
            if (part == null)
            {
                _logger.LogWarning("Update attempted on non-existent part ID {Id}.", id);
                return null;
            }

            var duplicate = await _db.Parts.AnyAsync(p => p.PartNumber == request.PartNumber && p.Id != id);
            if (duplicate)
                throw new InvalidOperationException($"A part with number '{request.PartNumber}' already exists.");

            part.Name = request.Name;
            part.PartNumber = request.PartNumber;
            part.Category = request.Category;
            part.Description = request.Description;
            part.SellingPrice = request.SellingPrice;
            part.PurchasePrice = request.PurchasePrice;
            part.StockQuantity = request.StockQuantity;
            part.LowStockThreshold = request.LowStockThreshold;
            part.IsActive = request.IsActive;
            part.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Part ID {Id} updated.", id);
            return MapToResponse(part);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var part = await _db.Parts.FindAsync(id);
            if (part == null)
            {
                _logger.LogWarning("Delete attempted on non-existent part ID {Id}.", id);
                return false;
            }

            _db.Parts.Remove(part);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Part ID {Id} deleted.", id);
            return true;
        }

        private static PartResponse MapToResponse(Part p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            PartNumber = p.PartNumber,
            Category = p.Category,
            Description = p.Description,
            SellingPrice = p.SellingPrice,
            PurchasePrice = p.PurchasePrice,
            StockQuantity = p.StockQuantity,
            LowStockThreshold = p.LowStockThreshold,
            IsLowStock = p.StockQuantity <= p.LowStockThreshold,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
