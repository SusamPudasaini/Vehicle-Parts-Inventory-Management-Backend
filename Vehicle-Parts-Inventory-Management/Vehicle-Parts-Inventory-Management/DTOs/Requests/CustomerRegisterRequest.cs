using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class CustomerRegisterRequest
    {
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Phone must be 10 to 15 digits.")]
        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
