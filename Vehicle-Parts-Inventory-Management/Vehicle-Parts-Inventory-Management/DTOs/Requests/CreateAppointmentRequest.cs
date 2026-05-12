using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class CreateAppointmentRequest
    {
        [Required]
        public int CustomerId { get; set; }

        public int? VehicleId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required, MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}