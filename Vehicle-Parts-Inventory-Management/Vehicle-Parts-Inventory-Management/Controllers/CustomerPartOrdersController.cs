using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiExplorerSettings(GroupName = "05-Inventory")]
    [ApiController]
    [Route("api/customer-part-orders")]
    public class CustomerPartOrdersController : ControllerBase
    {
        private readonly ICustomerPartPurchaseService _service;

        public CustomerPartOrdersController(ICustomerPartPurchaseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPut("{orderId:int}/approve")]
        public async Task<IActionResult> Approve(int orderId, [FromBody] UpdateCustomerPartOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.ApproveOrderAsync(orderId, request.StaffNotes);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{orderId:int}/reject")]
        public async Task<IActionResult> Reject(int orderId, [FromBody] UpdateCustomerPartOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.RejectOrderAsync(orderId, request.StaffNotes);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
