namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public List<VehicleResponse> Vehicles { get; set; } = new();
        public List<PurchaseHistoryResponse> PurchaseHistory { get; set; } = new();
        public List<ServiceHistoryResponse> ServiceHistory { get; set; } = new();
    }
}
