namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class CustomerPartCatalogResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
    }
}
