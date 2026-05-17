using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.Entities
{
    public enum PartRequestStatus
    {
        Pending = 0,
        Ordered = 1,
        Available = 2,
        Cancelled = 3
    }

    public class PartRequestEntity
    {
        [Key]
        public int PartRequestId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required, MaxLength(150)]
        public string PartName { get; set; } = string.Empty;

        [Range(1, 9999)]
        public int Quantity { get; set; } = 1;

        [MaxLength(50)]
        public string? VehicleNumber { get; set; } // helpful for matching

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public PartRequestStatus Status { get; set; } = PartRequestStatus.Pending;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}