using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class UpdateAppointmentScheduleRequest
    {
        [Required]
        public DateTime AppointmentDateTime { get; set; }
    }
}
