namespace Vehicle_Parts_Inventory_Management.Entities
{
    public class ServiceHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime ServiceDate { get; set; } = DateTime.UtcNow;

        public Customer Customer { get; set; } = null!;
    }
}
