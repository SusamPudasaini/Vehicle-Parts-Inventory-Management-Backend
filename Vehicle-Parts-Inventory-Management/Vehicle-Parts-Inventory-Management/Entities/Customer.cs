
namespace Vehicle_Parts_Inventory_Management.Entities
{
    /// Represents a customer registered in the system.
    public class Customer
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
