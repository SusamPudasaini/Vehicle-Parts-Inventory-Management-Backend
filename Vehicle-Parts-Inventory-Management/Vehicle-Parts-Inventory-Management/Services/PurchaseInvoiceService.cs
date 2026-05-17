using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;
using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;

namespace Vehicle_Parts_Inventory_Management.Services
{
    /// <summary>
    /// Feature 4: Admin can create purchase invoices for stock updates.
    /// Feature 1: Admin can generate daily, monthly, and yearly financial reports.
    /// </summary>
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<PurchaseInvoiceService> _logger;

        public PurchaseInvoiceService(AppDbContext db, ILogger<PurchaseInvoiceService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<PurchaseInvoiceResponse>> GetAllAsync()
        {
            return await _db.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.Items).ThenInclude(item => item.Part)
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => MapToResponse(i))
                .ToListAsync();
        }

        public async Task<PurchaseInvoiceResponse?> GetByIdAsync(int id)
        {
            var invoice = await _db.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.Items).ThenInclude(item => item.Part)
                .FirstOrDefaultAsync(i => i.Id == id);

            return invoice == null ? null : MapToResponse(invoice);
        }

        /// <summary>
        /// Feature 4: Creates a purchase invoice and automatically updates stock quantities.
        /// </summary>
        public async Task<PurchaseInvoiceResponse> CreateAsync(PurchaseInvoiceRequest request)
        {
            var vendor = await _db.Vendors.FindAsync(request.VendorId)
                ?? throw new InvalidOperationException($"Vendor with ID {request.VendorId} not found.");

            // Generate unique invoice number
            var invoiceNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var invoice = new PurchaseInvoice
            {
                InvoiceNumber = invoiceNumber,
                VendorId = request.VendorId,
                Notes = request.Notes,
                InvoiceDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };

            decimal total = 0;

            foreach (var itemReq in request.Items)
            {
                var part = await _db.Parts.FindAsync(itemReq.PartId)
                    ?? throw new InvalidOperationException($"Part with ID {itemReq.PartId} not found.");

                var lineTotal = itemReq.Quantity * itemReq.UnitPrice;
                total += lineTotal;

                invoice.Items.Add(new PurchaseInvoiceItem
                {
                    PartId = itemReq.PartId,
                    Quantity = itemReq.Quantity,
                    UnitPrice = itemReq.UnitPrice,
                });

                // Automatically update stock quantity
                part.StockQuantity += itemReq.Quantity;
                part.UpdatedAt = DateTime.UtcNow;
            }

            invoice.TotalAmount = total;

            _db.PurchaseInvoices.Add(invoice);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Purchase invoice '{InvoiceNumber}' created. Total: {Total}.", invoiceNumber, total);

            return await GetByIdAsync(invoice.Id) ?? MapToResponse(invoice);
        }

        // ── Feature 1: Financial Reports ──────────────────────────────────────

        public async Task<FinancialReportResponse> GetDailyReportAsync(DateTime date)
        {
            var start = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var end = start.AddDays(1);
            var invoices = await GetInvoicesInRange(start, end);
            return BuildReport($"Daily — {date:dd MMM yyyy}", invoices);
        }

        public async Task<FinancialReportResponse> GetMonthlyReportAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);
            var invoices = await GetInvoicesInRange(start, end);
            return BuildReport($"Monthly — {start:MMMM yyyy}", invoices);
        }

        public async Task<FinancialReportResponse> GetYearlyReportAsync(int year)
        {
            var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddYears(1);
            var invoices = await GetInvoicesInRange(start, end);
            return BuildReport($"Yearly — {year}", invoices);
        }

        private async Task<List<PurchaseInvoice>> GetInvoicesInRange(DateTime start, DateTime end)
        {
            return await _db.PurchaseInvoices
                .Include(i => i.Vendor)
                .Include(i => i.Items).ThenInclude(item => item.Part)
                .Where(i => i.InvoiceDate >= start && i.InvoiceDate < end)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        private static FinancialReportResponse BuildReport(string period, List<PurchaseInvoice> invoices)
        {
            return new FinancialReportResponse
            {
                Period = period,
                TotalPurchases = invoices.Sum(i => i.TotalAmount),
                TotalInvoices = invoices.Count,
                TotalItemsBought = invoices.SelectMany(i => i.Items).Sum(item => item.Quantity),
                Invoices = invoices.Select(MapToResponse).ToList()
            };
        }

        private static PurchaseInvoiceResponse MapToResponse(PurchaseInvoice i) => new()
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            VendorId = i.VendorId,
            VendorName = i.Vendor?.Name ?? "",
            InvoiceDate = i.InvoiceDate,
            Notes = i.Notes,
            TotalAmount = i.TotalAmount,
            CreatedAt = i.CreatedAt,
            Items = i.Items.Select(item => new PurchaseInvoiceItemResponse
            {
                Id = item.Id,
                PartId = item.PartId,
                PartName = item.Part?.Name ?? "",
                PartNumber = item.Part?.PartNumber ?? "",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice,
            }).ToList()
        };
    }
}