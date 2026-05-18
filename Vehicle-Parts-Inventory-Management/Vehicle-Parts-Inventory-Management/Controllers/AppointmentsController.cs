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
    }
}