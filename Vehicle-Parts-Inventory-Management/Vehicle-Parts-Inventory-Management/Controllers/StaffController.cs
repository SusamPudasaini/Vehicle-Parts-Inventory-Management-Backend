using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    /// Manages staff registration and role management.
    /// Feature 2: Admin can manage staff registration and roles.

    [ApiExplorerSettings(GroupName = "02-Staff")] // This puts all endpoints in this controller into the "02-Staff" dropdown in Swagger UI
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;
        private readonly ILogger<StaffController> _logger;

        public StaffController(IStaffService service, ILogger<StaffController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// Get all staff members.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        /// Get a specific staff member by ID.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Staff member with ID {id} not found." })
                : Ok(result);
        }

        /// Create a new staff member.
        /// Staff will receive an email verification link. Staff cannot log in until verified.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                // Used for known business errors (e.g., duplicate email)
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create staff member.");
                return StatusCode(500, new { message = "Failed to create staff member. " + ex.Message });
            }
        }

        /// Update an existing staff member's details or role.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] StaffUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, request);
            return result == null
                ? NotFound(new { message = $"Staff member with ID {id} not found." })
                : Ok(result);
        }

        /// Delete a staff member.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success
                ? NoContent()
                : NotFound(new { message = $"Staff member with ID {id} not found." });
        }
    }
}