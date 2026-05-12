using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vehicle_Parts_Inventory_Management.Entities
{
    public enum AppointmentStatus
    {
        Pending = 0,
        Confirmed = 1,
        Completed = 2,
        Cancelled = 3
    }

    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public int CustomerId { get; set; }   // from request (sessionStorage in frontend)

        public int? VehicleId { get; set; }   // full optional only if we have Vehicle table usage

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required, MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}