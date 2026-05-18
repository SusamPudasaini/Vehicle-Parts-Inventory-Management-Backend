using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiExplorerSettings(GroupName = "07-Reviews")] //  This puts all endpoints in this controller into the "07-Review APIs" dropdown in Swagger UI
    [ApiController]
    [Route("api/reviews")]
    public class ServiceReviewsController : ControllerBase
    {
        private readonly IServiceReviewService _service;
        public ServiceReviewsController(IServiceReviewService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceReviewRequest req)
        {
            var review = await _service.CreateAsync(req);
            return Ok(review);
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var list = await _service.GetByCustomerAsync(customerId);
            return Ok(list);
        }
    }
}