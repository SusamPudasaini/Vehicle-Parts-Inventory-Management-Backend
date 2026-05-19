namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class CustomerPurchaseCheckoutResponse
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CustomerPurchaseCheckoutItemResponse> Items { get; set; } = new();
    }

    public class CustomerPurchaseCheckoutItemResponse
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
