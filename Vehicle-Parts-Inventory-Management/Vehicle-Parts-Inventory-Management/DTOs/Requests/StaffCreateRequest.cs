namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class StaffCreateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Staff";
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
