using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.Entities;
namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IPartRequestService
    {
        Task<PartRequestEntity> CreateAsync(int customerId, CreatePartRequestRequest req);
        Task<List<PartRequestResponse>> GetByCustomerAsync(int customerId);
        Task<List<PartRequestResponse>> GetAllAsync();
        Task<PartRequestEntity?> UpdateStatusAsync(int partRequestId, PartRequestStatus status);
    }
}