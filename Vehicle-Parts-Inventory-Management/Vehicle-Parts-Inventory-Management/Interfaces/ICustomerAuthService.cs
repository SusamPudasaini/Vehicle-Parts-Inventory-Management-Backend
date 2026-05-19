using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface ICustomerAuthService
    {
        // Auth
        Task<(bool Success, string Message)> RegisterAsync(CustomerRegisterRequest request);
        Task<Customer?> LoginAsync(LoginRequest request);

        // Email verification
        Task<(bool Success, string Message)> VerifyEmailAsync(string token);

        // Profile
        Task<Customer?> GetProfileAsync(int customerId);
        Task<(bool Success, string Message)> UpdateProfileAsync(int customerId, UpdateProfileRequest request);
        Task<(bool Success, string Message)> ChangePasswordAsync(int customerId, ChangePasswordRequest request);

        // Vehicles
        Task<List<Vehicle>> GetVehiclesAsync(int customerId);
        Task<(bool Success, string Message)> AddVehicleAsync(int customerId, VehicleRequest request);
        Task<(bool Success, string Message)> UpdateVehicleAsync(int customerId, int vehicleId, VehicleRequest request);
        Task<(bool Success, string Message)> DeleteVehicleAsync(int customerId, int vehicleId);

        // History
        Task<List<PurchaseHistory>> GetPurchaseHistoryAsync(int customerId);
        Task<List<ServiceHistory>> GetServiceHistoryAsync(int customerId);
    }
}