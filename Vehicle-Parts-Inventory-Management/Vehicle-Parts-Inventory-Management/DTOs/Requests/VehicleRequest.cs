using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class VehicleRequest
    {
        [Required(ErrorMessage = "Vehicle number is required.")]
        [MaxLength(50, ErrorMessage = "Vehicle number cannot exceed 50 characters.")]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Make is required.")]
        [MaxLength(100, ErrorMessage = "Make cannot exceed 100 characters.")]
        public string Make { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required.")]
        [MaxLength(100, ErrorMessage = "Model cannot exceed 100 characters.")]
        public string Model { get; set; } = string.Empty;

        [Range(1886, 2100, ErrorMessage = "Enter a valid vehicle year.")]
        public int Year { get; set; }

        [MaxLength(50, ErrorMessage = "Color cannot exceed 50 characters.")]
        public string Color { get; set; } = string.Empty;
    }
}
