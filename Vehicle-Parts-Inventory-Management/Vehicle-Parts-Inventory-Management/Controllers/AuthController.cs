using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiExplorerSettings(GroupName = "00-Auth")] // This puts all endpoints in this controller into the "00-Auth" dropdown in Swagger UI
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db) => _db = db;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required." });

            var staff = await _db.Staff
                .FirstOrDefaultAsync(s => s.Email.ToLower() == request.Email.ToLower());

            if (staff == null || !BCrypt.Net.BCrypt.Verify(request.Password, staff.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            if (!staff.IsActive)
                return Unauthorized(new { message = "Your account has been deactivated. Contact an admin." });

            return Ok(new LoginResponse
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Email = staff.Email,
                Role = staff.Role,
            });
        }
    }
}