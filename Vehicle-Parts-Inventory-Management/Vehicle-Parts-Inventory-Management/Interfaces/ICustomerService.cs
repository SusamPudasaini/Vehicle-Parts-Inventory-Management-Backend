
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> RegisterAsync(CustomerCreateRequest request);
        Task<CustomerResponse?> GetByIdAsync(int id);
        Task<List<CustomerResponse>> SearchAsync(string query); // F10
        Task<List<CustomerResponse>> GetAllAsync();
    }
}
