
using System.Text.Json.Serialization;

namespace Vehicle_Parts_Inventory_Management.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;    // e.g. Toyota
        public string Model { get; set; } = string.Empty;  // e.g. Corolla
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        [JsonIgnore]
        public Customer Customer { get; set; } = null!;
    }
}
