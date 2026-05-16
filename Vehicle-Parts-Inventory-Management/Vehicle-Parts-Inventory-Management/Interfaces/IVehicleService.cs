using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IVehicleService
    {
        Task<List<VehicleResponse>> GetAllAsync();
        Task<List<VehicleResponse>> GetByCustomerIdAsync(int customerId);
        Task<VehicleResponse?> GetByIdAsync(int id);
        Task<VehicleResponse> CreateAsync(VehicleCreateRequest request);
        Task<VehicleResponse> CreateForCustomerAsync(int customerId, VehicleRequest request);
        Task<VehicleResponse?> UpdateAsync(int id, VehicleRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
