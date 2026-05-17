using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IPurchaseInvoiceService
    {
        Task<List<PurchaseInvoiceResponse>> GetAllAsync();
        Task<PurchaseInvoiceResponse?> GetByIdAsync(int id);
        Task<PurchaseInvoiceResponse> CreateAsync(PurchaseInvoiceRequest request);

        // Feature 1: Financial reports
        Task<FinancialReportResponse> GetDailyReportAsync(DateTime date);
        Task<FinancialReportResponse> GetMonthlyReportAsync(int year, int month);
        Task<FinancialReportResponse> GetYearlyReportAsync(int year);
    }
}
