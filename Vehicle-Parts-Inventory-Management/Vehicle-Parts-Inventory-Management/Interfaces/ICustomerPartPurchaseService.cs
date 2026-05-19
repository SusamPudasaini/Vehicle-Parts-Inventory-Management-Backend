using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface ICustomerPartPurchaseService
    {
        Task<List<CustomerPartCatalogResponse>> GetCatalogAsync();
        Task<CustomerPurchaseCheckoutResponse> SubmitOrderAsync(int customerId, CustomerPurchaseCheckoutRequest request);
        Task<List<CustomerPartOrderResponse>> GetCustomerOrdersAsync(int customerId);
        Task<List<CustomerPartOrderResponse>> GetAllOrdersAsync();
        Task<CustomerPartOrderResponse> ApproveOrderAsync(int orderId, string? staffNotes);
        Task<CustomerPartOrderResponse> RejectOrderAsync(int orderId, string? staffNotes);
    }
}
