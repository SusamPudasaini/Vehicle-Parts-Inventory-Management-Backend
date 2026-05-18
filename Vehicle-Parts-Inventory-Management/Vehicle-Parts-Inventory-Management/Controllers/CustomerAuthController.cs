using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Services;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiExplorerSettings(GroupName = "00-Auth")] // This puts all endpoints in this controller into the "00-Auth" dropdown in Swagger UI
    [ApiController]
    [Route("api/customer-auth")]
    public class CustomerAuthController : ControllerBase
    {
        private readonly ICustomerAuthService _service;

        public CustomerAuthController(ICustomerAuthService service)
        {
            _service = service;
        }

        // POST: api/customer-auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomerRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // POST: api/customer-auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _service.LoginAsync(request);

            if (customer == null)
                return Unauthorized(new { message = "Invalid email or password." });

            HttpContext.Session.SetString("CustomerId", customer.Id.ToString());
            HttpContext.Session.SetString("CustomerName", customer.FullName);
            HttpContext.Session.SetString("CustomerEmail", customer.Email);
            HttpContext.Session.SetString("Role", "Customer");

            return Ok(new
            {
                id = customer.Id,
                fullName = customer.FullName,
                email = customer.Email,
                phone = customer.Phone,
                address = customer.Address,
                role = "Customer"
            });
        }

        // POST: api/customer-auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }

        // GET: api/customer-auth/check-session
        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");

            if (string.IsNullOrEmpty(customerId))
                return Unauthorized(new { message = "Not logged in." });

            return Ok(new
            {
                id = customerId,
                name = HttpContext.Session.GetString("CustomerName"),
                role = HttpContext.Session.GetString("Role")
            });
        }
    }
}