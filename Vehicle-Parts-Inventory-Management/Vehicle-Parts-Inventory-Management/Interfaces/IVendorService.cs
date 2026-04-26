using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IVendorService
    {
        Task<List<VendorResponse>> GetAllAsync();
        Task<VendorResponse?> GetByIdAsync(int id);
        Task<VendorResponse> CreateAsync(VendorRequest request);
        Task<VendorResponse?> UpdateAsync(int id, VendorRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
