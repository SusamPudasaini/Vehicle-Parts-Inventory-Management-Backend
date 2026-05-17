using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    //public class PartRequestService : IPartRequestService
    //{
    //    private readonly AppDbContext _db;
    //    public PartRequestService(AppDbContext db) => _db = db;

    //    public async Task<PartRequest> CreateAsync(CreatePartRequestRequest req)
    //    {
    //        var pr = new PartRequest
    //        {
    //            CustomerId = req.CustomerId,
    //            PartName = req.PartName,
    //            Quantity = req.Quantity,
    //            VehicleNumber = req.VehicleNumber,
    //            Description = req.Description,
    //            Status = PartRequestStatus.Pending,
    //            CreatedAtUtc = DateTime.UtcNow
    //        };

    //        _db.PartRequests.Add(pr);
    //        await _db.SaveChangesAsync();
    //        return pr;
    //    }

    //    public Task<List<PartRequest>> GetByCustomerAsync(int customerId)
    //    {
    //        return _db.PartRequests
    //            .Where(x => x.CustomerId == customerId)
    //            .OrderByDescending(x => x.CreatedAtUtc)
    //            .ToListAsync();
    //    }
    //}
}