using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public interface ICustomerAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(CustomerRegisterRequest request);
        Task<Customer?> LoginAsync(LoginRequest request);
        Task<Customer?> GetProfileAsync(int customerId);
        Task<(bool Success, string Message)> UpdateProfileAsync(int customerId, UpdateProfileRequest request);
        Task<(bool Success, string Message)> ChangePasswordAsync(int customerId, ChangePasswordRequest request);
        Task<(bool Success, string Message)> AddVehicleAsync(int customerId, VehicleRequest request);
        Task<(bool Success, string Message)> UpdateVehicleAsync(int customerId, int vehicleId, VehicleRequest request);
        Task<(bool Success, string Message)> DeleteVehicleAsync(int customerId, int vehicleId);
        Task<List<Vehicle>> GetVehiclesAsync(int customerId);
        Task<List<PurchaseHistory>> GetPurchaseHistoryAsync(int customerId);
        Task<List<ServiceHistory>> GetServiceHistoryAsync(int customerId);
    }
}