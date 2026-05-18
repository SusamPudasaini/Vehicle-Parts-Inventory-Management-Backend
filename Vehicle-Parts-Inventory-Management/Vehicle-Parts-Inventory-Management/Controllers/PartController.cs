using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    /// <summary>
    /// Feature 3: Admin can perform parts management (add, edit, delete).
    /// Feature 15: Low stock detection endpoint.
    /// </summary>

    [ApiExplorerSettings(GroupName = "05-Inventory")] // This puts all endpoints in this controller into the "05-Inventory/Parts APIs" dropdown in Swagger UI
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PartController : ControllerBase
    {
        private readonly IPartService _service;
        private readonly ILogger<PartController> _logger;

        public PartController(IPartService service, ILogger<PartController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>Get all parts in inventory.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        /// <summary>Get a specific part by ID.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Part with ID {id} not found." })
                : Ok(result);
        }

        /// <summary>Feature 15: Get all parts with stock at or below their low stock threshold.</summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var result = await _service.GetLowStockAsync();
            return Ok(result);
        }

        /// <summary>Feature 3: Add a new part to inventory.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PartRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>Feature 3: Update an existing part's details or stock quantity.</summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PartRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.UpdateAsync(id, request);
                return result == null
                    ? NotFound(new { message = $"Part with ID {id} not found." })
                    : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>Feature 3: Delete a part from inventory.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success
                ? NoContent()
                : NotFound(new { message = $"Part with ID {id} not found." });
        }
    }
}
