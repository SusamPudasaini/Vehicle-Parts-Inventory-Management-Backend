using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class PartRequestResponse
    {
        public int PartRequestId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Description { get; set; }
        public PartRequestStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
