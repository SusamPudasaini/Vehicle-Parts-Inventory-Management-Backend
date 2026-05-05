using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IServiceReviewService
    {
        Task<ServiceReview> CreateAsync(CreateServiceReviewRequest req);
        Task<List<ServiceReview>> GetByCustomerAsync(int customerId);
    }
}