using Microsoft.AspNetCore.Mvc;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    /// <summary>
    /// Feature 4: Admin can create purchase invoices for stock updates.
    /// Feature 1: Admin can generate daily, monthly, and yearly financial reports.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PurchaseInvoiceController : ControllerBase
    {
        private readonly IPurchaseInvoiceService _service;
        private readonly ILogger<PurchaseInvoiceController> _logger;

        public PurchaseInvoiceController(IPurchaseInvoiceService service, ILogger<PurchaseInvoiceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>Get all purchase invoices.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        /// <summary>Get a specific purchase invoice by ID.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = $"Invoice with ID {id} not found." })
                : Ok(result);
        }

        /// <summary>Feature 4: Create a purchase invoice. Automatically updates part stock quantities.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseInvoiceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ── Feature 1: Financial Reports ──────────────────────────────────────

        /// <summary>Feature 1: Get daily financial report for a specific date. Format: yyyy-MM-dd</summary>
        [HttpGet("report/daily")]
        public async Task<IActionResult> DailyReport([FromQuery] string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd." });

            var result = await _service.GetDailyReportAsync(parsedDate);
            return Ok(result);
        }

        /// <summary>Feature 1: Get monthly financial report.</summary>
        [HttpGet("report/monthly")]
        public async Task<IActionResult> MonthlyReport([FromQuery] int year, [FromQuery] int month)
        {
            if (year < 2000 || month < 1 || month > 12)
                return BadRequest(new { message = "Invalid year or month." });

            var result = await _service.GetMonthlyReportAsync(year, month);
            return Ok(result);
        }

        /// <summary>Feature 1: Get yearly financial report.</summary>
        [HttpGet("report/yearly")]
        public async Task<IActionResult> YearlyReport([FromQuery] int year)
        {
            if (year < 2000)
                return BadRequest(new { message = "Invalid year." });

            var result = await _service.GetYearlyReportAsync(year);
            return Ok(result);
        }
    }
}
