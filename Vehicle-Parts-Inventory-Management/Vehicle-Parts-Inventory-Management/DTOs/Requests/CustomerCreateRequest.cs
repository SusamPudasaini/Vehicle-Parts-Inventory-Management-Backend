
namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class CustomerCreateRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public VehicleRequest Vehicle { get; set; } = null!;
    }
}
