using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiExplorerSettings(GroupName = "05-Inventory")]
    [ApiController]
    [Route("api/customer-parts")]
    public class CustomerPartsController : ControllerBase
    {
        private readonly ICustomerPartPurchaseService _service;

        public CustomerPartsController(ICustomerPartPurchaseService service)
        {
            _service = service;
        }

        private int? GetCustomerId()
        {
            var idStr = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(idStr)) return null;
            return int.TryParse(idStr, out var id) ? id : null;
        }

        [HttpGet]
        public async Task<IActionResult> GetCatalog()
        {
            var customerId = GetCustomerId();
            if (customerId == null)
                return Unauthorized(new { message = "Please log in." });

            var parts = await _service.GetCatalogAsync();
            return Ok(parts);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CustomerPurchaseCheckoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = GetCustomerId();
            if (customerId == null)
                return Unauthorized(new { message = "Please log in." });

            try
            {
                var result = await _service.SubmitOrderAsync(customerId.Value, request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var customerId = GetCustomerId();
            if (customerId == null)
                return Unauthorized(new { message = "Please log in." });

            var orders = await _service.GetCustomerOrdersAsync(customerId.Value);
            return Ok(orders);
        }
    }
}
