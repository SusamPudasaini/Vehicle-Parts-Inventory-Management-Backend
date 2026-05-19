using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.DTOs.Responses
{
    public class AppointmentSummaryResponse
    {
        public int AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public int? VehicleId { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
