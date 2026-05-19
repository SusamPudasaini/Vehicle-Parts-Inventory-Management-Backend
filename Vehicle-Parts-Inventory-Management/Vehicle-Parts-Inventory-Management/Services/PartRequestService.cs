using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class PartRequestService : IPartRequestService
    {
        private readonly AppDbContext _db;
        public PartRequestService(AppDbContext db) => _db = db;

        public async Task<PartRequestEntity> CreateAsync(int customerId, CreatePartRequestRequest req)
        {
            var pr = new PartRequestEntity
            {
                CustomerId = customerId,
                PartName = req.PartName.Trim(),
                Quantity = req.Quantity,
                VehicleNumber = string.IsNullOrWhiteSpace(req.VehicleNumber) ? null : req.VehicleNumber.Trim(),
                Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
                Status = PartRequestStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.PartRequests.Add(pr);
            await _db.SaveChangesAsync();
            return pr;
        }

        public Task<List<PartRequestResponse>> GetByCustomerAsync(int customerId)
        {
            return (from pr in _db.PartRequests
                    join c in _db.Customers on pr.CustomerId equals c.Id
                    where pr.CustomerId == customerId
                    orderby pr.CreatedAtUtc descending
                    select new PartRequestResponse
                    {
                        PartRequestId = pr.PartRequestId,
                        CustomerId = c.Id,
                        CustomerName = c.FullName,
                        PartName = pr.PartName,
                        Quantity = pr.Quantity,
                        VehicleNumber = pr.VehicleNumber,
                        Description = pr.Description,
                        Status = pr.Status,
                        CreatedAtUtc = pr.CreatedAtUtc
                    }).ToListAsync();
        }

        public Task<List<PartRequestResponse>> GetAllAsync()
        {
            return (from pr in _db.PartRequests
                    join c in _db.Customers on pr.CustomerId equals c.Id
                    orderby pr.CreatedAtUtc descending
                    select new PartRequestResponse
                    {
                        PartRequestId = pr.PartRequestId,
                        CustomerId = c.Id,
                        CustomerName = c.FullName,
                        PartName = pr.PartName,
                        Quantity = pr.Quantity,
                        VehicleNumber = pr.VehicleNumber,
                        Description = pr.Description,
                        Status = pr.Status,
                        CreatedAtUtc = pr.CreatedAtUtc
                    }).ToListAsync();
        }

        public async Task<PartRequestEntity?> UpdateStatusAsync(int partRequestId, PartRequestStatus status)
        {
            var pr = await _db.PartRequests.FirstOrDefaultAsync(p => p.PartRequestId == partRequestId);
            if (pr == null)
                return null;

            pr.Status = status;
            await _db.SaveChangesAsync();
            return pr;
        }
    }
}