using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class PartRequest
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string PartNumber { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(0, double.MaxValue)] public decimal SellingPrice { get; set; }
        [Range(0, double.MaxValue)] public decimal PurchasePrice { get; set; }
        [Range(0, int.MaxValue)] public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; } = 10;
        public bool IsActive { get; set; } = true;
    }
}
