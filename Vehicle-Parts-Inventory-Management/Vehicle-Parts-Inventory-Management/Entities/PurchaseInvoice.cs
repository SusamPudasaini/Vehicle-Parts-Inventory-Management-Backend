namespace Vehicle_Parts_Inventory_Management.Entities
{
    /// <summary>
    /// Represents a purchase invoice when admin buys stock from a vendor.
    /// Feature 4: Admin can create purchase invoices for stock updates.
    /// </summary>
    public class PurchaseInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; } = null!;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<PurchaseInvoiceItem> Items { get; set; } = new();
    }

    /// <summary>Individual line item on a purchase invoice.</summary>
    public class PurchaseInvoiceItem
    {
        public int Id { get; set; }
        public int PurchaseInvoiceId { get; set; }
        public PurchaseInvoice PurchaseInvoice { get; set; } = null!;
        public int PartId { get; set; }
        public Part Part { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
