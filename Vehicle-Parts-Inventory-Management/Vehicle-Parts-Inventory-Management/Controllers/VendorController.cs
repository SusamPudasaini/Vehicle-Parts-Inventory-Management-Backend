
using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    /// Manages vendor CRUD operations.
    /// Feature 5: Admin can manage vendor details.
    [ApiExplorerSettings(GroupName = "03-Vendors")] // This puts all endpoints in this controller into the "03-Vendors" dropdown in Swagger UI
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _service;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorService service, ILogger<VendorController> logger)
        {
            _service = service;
            _logger = logger;
        }

        ///  Get all vendors.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        ///  Get a specific vendor by ID.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = $"Vendor with ID {id} not found." }) : Ok(result);
        }

        /// Create a new vendor.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VendorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// Update an existing vendor's details.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VendorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, request);
            return result == null ? NotFound(new { message = $"Vendor with ID {id} not found." }) : Ok(result);
        }

        /// Delete a vendor.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound(new { message = $"Vendor with ID {id} not found." });
        }
    }
}
