namespace Vehicle_Parts_Inventory_Management.Entities
{
    /// <summary>Represents a vehicle part in the inventory.</summary>
    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; } = 10;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
