using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IPartService
    {
        Task<List<PartResponse>> GetAllAsync();
        Task<PartResponse?> GetByIdAsync(int id);
        Task<List<PartResponse>> GetLowStockAsync();
        Task<PartResponse> CreateAsync(PartRequest request);
        Task<PartResponse?> UpdateAsync(int id, PartRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
