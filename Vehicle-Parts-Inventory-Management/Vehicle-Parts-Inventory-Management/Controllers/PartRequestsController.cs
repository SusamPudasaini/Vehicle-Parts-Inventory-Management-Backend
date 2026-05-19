using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiController]
    [Route("api/part-requests")]
    public class PartRequestsController : ControllerBase
    {
        private readonly IPartRequestService _service;
        public PartRequestsController(IPartRequestService service) => _service = service;

        private int? GetCustomerId()
        {
            var idStr = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(idStr)) return null;
            return int.TryParse(idStr, out int id) ? id : null;
        }

        // POST: api/part-requests
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePartRequestRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = req.CustomerId ?? GetCustomerId();
            if (customerId == null)
                return Unauthorized(new { message = "Please log in." });

            var pr = await _service.CreateAsync(customerId.Value, req);
            return Ok(new { message = "Part request submitted successfully.", id = pr.PartRequestId });
        }

        // GET: api/part-requests/customer
        [HttpGet("customer")]
        public async Task<IActionResult> GetByCustomer()
        {
            var customerId = GetCustomerId();
            if (customerId == null)
                return Unauthorized(new { message = "Please log in." });

            var list = await _service.GetByCustomerAsync(customerId.Value);
            return Ok(list);
        }

        // GET: api/part-requests
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // PUT: api/part-requests/{partRequestId}/status
        [HttpPut("{partRequestId:int}/status")]
        public async Task<IActionResult> UpdateStatus(int partRequestId, [FromBody] UpdatePartRequestStatusRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pr = await _service.UpdateStatusAsync(partRequestId, req.Status);
            if (pr == null)
                return NotFound(new { message = "Part request not found." });

            return Ok(new { message = "Part request status updated." });
        }
    }
}