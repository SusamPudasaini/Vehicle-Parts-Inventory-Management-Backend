using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class PurchaseInvoiceRequest
    {
        [Required] public int VendorId { get; set; }
        public string Notes { get; set; } = string.Empty;
        [Required, MinLength(1)] public List<PurchaseInvoiceItemRequest> Items { get; set; } = new();
    }

    public class PurchaseInvoiceItemRequest
    {
        [Required] public int PartId { get; set; }
        [Range(1, int.MaxValue)] public int Quantity { get; set; }
        [Range(0, double.MaxValue)] public decimal UnitPrice { get; set; }
    }
}
