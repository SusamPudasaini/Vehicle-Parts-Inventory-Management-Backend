using System.ComponentModel.DataAnnotations;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class UpdatePartRequestStatusRequest
    {
        [Required]
        public PartRequestStatus Status { get; set; }
    }
}
