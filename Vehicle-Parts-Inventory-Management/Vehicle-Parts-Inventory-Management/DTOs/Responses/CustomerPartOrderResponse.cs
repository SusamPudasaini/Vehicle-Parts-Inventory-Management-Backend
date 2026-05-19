namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class CustomerPartOrderResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? InvoiceNumber { get; set; }
        public string? StaffNotes { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime RequestedAtUtc { get; set; }
        public DateTime? ProcessedAtUtc { get; set; }
        public List<CustomerPartOrderItemResponse> Items { get; set; } = new();
    }

    public class CustomerPartOrderItemResponse
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
