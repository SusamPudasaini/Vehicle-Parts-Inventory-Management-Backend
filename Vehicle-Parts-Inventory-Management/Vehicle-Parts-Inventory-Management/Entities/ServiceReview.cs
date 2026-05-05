using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.Entities
{
    public class ServiceReview
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? AppointmentId { get; set; } // optional but useful

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}