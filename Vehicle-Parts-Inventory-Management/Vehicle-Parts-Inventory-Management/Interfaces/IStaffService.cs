
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;


namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IStaffService
    {
        Task<List<StaffResponse>> GetAllAsync();
        Task<StaffResponse?> GetByIdAsync(int id);
        Task<StaffResponse> CreateAsync(StaffCreateRequest request);
        Task<StaffResponse?> UpdateAsync(int id, StaffUpdateRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
