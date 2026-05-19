using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class UpdateCustomerPartOrderRequest
    {
        [MaxLength(500)]
        public string? StaffNotes { get; set; }
    }
}
