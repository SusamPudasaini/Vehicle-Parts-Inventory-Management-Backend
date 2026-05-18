using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    /// Handles staff-facing vehicle registration and lookup.
    /// 
    [ApiExplorerSettings(GroupName = "04-Vehicles")] // This puts all endpoints in this controller into the "04-Vehicles" dropdown in Swagger UI
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _service;

        public VehicleController(IVehicleService service)
        {
            _service = service;
        }

        /// Get all registered vehicles.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        /// Get a specific vehicle by ID.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = $"Vehicle with ID {id} not found." }) : Ok(result);
        }

        /// Get all vehicles registered to a customer.
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var result = await _service.GetByCustomerIdAsync(customerId);
            return Ok(result);
        }

        /// Register a vehicle for a customer.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// Register a vehicle for a customer using the customer ID from the route.
        [HttpPost("customer/{customerId:int}")]
        public async Task<IActionResult> CreateForCustomer(int customerId, [FromBody] VehicleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateForCustomerAsync(customerId, request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// Update an existing vehicle.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdateAsync(id, request);
                return result == null ? NotFound(new { message = $"Vehicle with ID {id} not found." }) : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// Delete a vehicle.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound(new { message = $"Vehicle with ID {id} not found." });
        }
    }
}
