using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.Entities
{
    public enum CustomerPartOrderStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class CustomerPartOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        [Required]
        public CustomerPartOrderStatus Status { get; set; } = CustomerPartOrderStatus.Pending;

        [MaxLength(50)]
        public string? InvoiceNumber { get; set; }

        [MaxLength(500)]
        public string? StaffNotes { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAtUtc { get; set; }

        public List<CustomerPartOrderItem> Items { get; set; } = new();
    }

    public class CustomerPartOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerPartOrderId { get; set; }

        public CustomerPartOrder CustomerPartOrder { get; set; } = null!;

        [Required]
        public int PartId { get; set; }

        public Part Part { get; set; } = null!;

        [Required, MaxLength(150)]
        public string PartName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string PartNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}
