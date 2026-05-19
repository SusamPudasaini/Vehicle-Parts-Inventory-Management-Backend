using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiExplorerSettings(GroupName = "06-Appointments")] // This puts all endpoints in this controller into the "06-Appointments" dropdown in Swagger UI
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;
        public AppointmentsController(IAppointmentService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest req)
        {
            var appt = await _service.CreateAsync(req);
            return Ok(appt);
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var list = await _service.GetByCustomerAsync(customerId);
            return Ok(list);
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // PUT: api/appointments/{appointmentId}/status
        [HttpPut("{appointmentId:int}/status")]
        public async Task<IActionResult> UpdateStatus(int appointmentId, [FromBody] UpdateAppointmentStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateStatusAsync(appointmentId, request.Status);
            if (result == null)
                return NotFound(new { message = "Appointment not found." });

            return Ok(new { message = "Appointment status updated." });
        }

        // PUT: api/appointments/{appointmentId}/schedule
        [HttpPut("{appointmentId:int}/schedule")]
        public async Task<IActionResult> Reschedule(int appointmentId, [FromBody] UpdateAppointmentScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.RescheduleAsync(appointmentId, request.AppointmentDateTime);
                if (result == null)
                    return NotFound(new { message = "Appointment not found." });

                return Ok(new { message = "Appointment rescheduled." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}