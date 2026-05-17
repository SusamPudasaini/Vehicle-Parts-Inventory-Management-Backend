using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiController]
    [Route("api/part-requests")]
    public class PartRequestsController : ControllerBase
    {
        //private readonly IPartRequestService _service;
        //public PartRequestsController(IPartRequestService service) => _service = service;

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreatePartRequestRequest req)
        //{
        //    var pr = await _service.CreateAsync(req);
        //    return Ok(pr);
        //}

        //[HttpGet("customer/{customerId:int}")]
        //public async Task<IActionResult> GetByCustomer(int customerId)
        //{
        //    var list = await _service.GetByCustomerAsync(customerId);
        //    return Ok(list);
        //}
    }
}