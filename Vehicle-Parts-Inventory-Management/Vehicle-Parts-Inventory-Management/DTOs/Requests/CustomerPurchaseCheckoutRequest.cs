using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class CustomerPurchaseCheckoutRequest
    {
        [Required, MinLength(1)]
        public List<CustomerPurchaseCheckoutItemRequest> Items { get; set; } = new();
    }

    public class CustomerPurchaseCheckoutItemRequest
    {
        [Required]
        public int PartId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
