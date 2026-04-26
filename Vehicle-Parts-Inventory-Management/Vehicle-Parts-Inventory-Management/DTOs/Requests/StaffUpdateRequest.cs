namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class StaffUpdateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
