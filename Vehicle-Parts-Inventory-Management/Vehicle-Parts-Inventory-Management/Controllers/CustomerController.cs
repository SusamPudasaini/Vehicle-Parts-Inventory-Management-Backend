using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    /// Manages customer registration, retrieval, and search.
    /// Feature 6: Staff can register new customers with vehicle details.
    /// Feature 8: Staff can view customer details, history, and vehicle info.
    /// Feature 10: Staff can search customers by vehicle number, phone, ID, or name.
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService service, ILogger<CustomerController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// Get all customers.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        ///Feature 8: Get full customer detail including vehicles and history.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = $"Customer with ID {id} not found." }) : Ok(result);
        }

        ///Feature 10: Search customers by name, phone, ID, or vehicle number.
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Search query cannot be empty." });

            var results = await _service.SearchAsync(q);
            return Ok(results);
        }

        ///Feature 6: Register a new customer with vehicle details. 
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CustomerCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.RegisterAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
