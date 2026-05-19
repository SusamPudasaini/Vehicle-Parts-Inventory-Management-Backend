using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.Helpers;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiExplorerSettings(GroupName = "00-Auth")]
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

            // Blocking staff/admin login until verified
            if (!staff.IsEmailVerified)
                return StatusCode(403, new { message = "Staff email not verified. Please verify before logging in." });

            return Ok(new LoginResponse
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Email = staff.Email,
                Role = staff.Role,
            });
        }

        // GET: api/auth/verify-staff-email?token=XXXX
        [HttpGet("verify-staff-email")]
        public async Task<IActionResult> VerifyStaffEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token is required." });

            var tokenHash = EmailVerificationToken.HashToken(token);

            var staff = await _db.Staff.FirstOrDefaultAsync(s =>
                s.EmailVerificationTokenHash == tokenHash &&
                s.EmailVerificationTokenExpiresUtc != null &&
                s.EmailVerificationTokenExpiresUtc > DateTime.UtcNow);

            if (staff == null)
                return BadRequest(new { message = "Invalid or expired verification link." });

            staff.IsEmailVerified = true;
            staff.EmailVerificationTokenHash = null;
            staff.EmailVerificationTokenExpiresUtc = null;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Staff email verified successfully." });
        }
    }
}