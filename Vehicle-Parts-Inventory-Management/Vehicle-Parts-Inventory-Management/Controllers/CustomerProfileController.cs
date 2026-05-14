using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Services;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiController]
    [Route("api/customer-profile")]
    public class CustomerProfileController : ControllerBase
    {
        private readonly ICustomerAuthService _service;

        public CustomerProfileController(ICustomerAuthService service)
        {
            _service = service;
        }

        private int? GetCustomerId()
        {
            var idStr = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(idStr)) return null;
            return int.TryParse(idStr, out int id) ? id : null;
        }

        // GET: api/customer-profile/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var profile = await _service.GetProfileAsync(id.Value);
            if (profile == null)
                return NotFound(new { message = "Customer not found." });

            return Ok(new
            {
                profile.Id,
                profile.FullName,
                profile.Email,
                profile.Phone,
                profile.Address,
                profile.RegisteredAt,
                vehicles = profile.Vehicles
            });
        }

        // PUT: api/customer-profile/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var result = await _service.UpdateProfileAsync(id.Value, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            HttpContext.Session.SetString("CustomerName", request.FullName);
            return Ok(new { message = result.Message });
        }

        // PUT: api/customer-profile/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var result = await _service.ChangePasswordAsync(id.Value, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // GET: api/customer-profile/vehicles
        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var vehicles = await _service.GetVehiclesAsync(id.Value);
            return Ok(vehicles);
        }

        // POST: api/customer-profile/vehicles
        [HttpPost("vehicles")]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var result = await _service.AddVehicleAsync(id.Value, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // PUT: api/customer-profile/vehicles/{vehicleId}
        [HttpPut("vehicles/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] VehicleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var result = await _service.UpdateVehicleAsync(id.Value, vehicleId, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // DELETE: api/customer-profile/vehicles/{vehicleId}
        [HttpDelete("vehicles/{vehicleId}")]
        public async Task<IActionResult> DeleteVehicle(int vehicleId)
        {
            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var result = await _service.DeleteVehicleAsync(id.Value, vehicleId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // GET: api/customer-profile/purchase-history
        [HttpGet("purchase-history")]
        public async Task<IActionResult> GetPurchaseHistory()
        {
            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var history = await _service.GetPurchaseHistoryAsync(id.Value);
            return Ok(history);
        }

        // GET: api/customer-profile/service-history
        [HttpGet("service-history")]
        public async Task<IActionResult> GetServiceHistory()
        {
            var id = GetCustomerId();
            if (id == null)
                return Unauthorized(new { message = "Please log in." });

            var history = await _service.GetServiceHistoryAsync(id.Value);
            return Ok(history);
        }


    }
}