namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class ServiceHistoryResponse
    {
        public int Id { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
    }
}
