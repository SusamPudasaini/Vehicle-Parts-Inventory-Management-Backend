using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IPartRequestService
    {
        Task<PartRequest> CreateAsync(CreatePartRequestRequest req);
        Task<List<PartRequest>> GetByCustomerAsync(int customerId);
    }
}