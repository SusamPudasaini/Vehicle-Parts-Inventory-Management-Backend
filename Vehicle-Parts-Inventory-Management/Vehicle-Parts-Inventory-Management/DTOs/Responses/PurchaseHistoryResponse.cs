namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class PurchaseHistoryResponse
    {
        public int Id { get; set; }
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime PurchasedAt { get; set; }
    }
}
