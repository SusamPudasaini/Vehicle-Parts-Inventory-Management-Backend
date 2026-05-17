namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class PurchaseInvoiceResponse
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PurchaseInvoiceItemResponse> Items { get; set; } = new();
    }

    public class PurchaseInvoiceItemResponse
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    // ── Feature 1: Financial Report Responses ─────────────────────────────────
    public class FinancialReportResponse
    {
        public string Period { get; set; } = string.Empty;
        public decimal TotalPurchases { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalItemsBought { get; set; }
        public List<PurchaseInvoiceResponse> Invoices { get; set; } = new();
    }
}
