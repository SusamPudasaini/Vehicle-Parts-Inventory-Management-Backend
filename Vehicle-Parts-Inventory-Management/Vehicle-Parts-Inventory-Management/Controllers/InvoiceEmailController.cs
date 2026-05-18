using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceEmailController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _email;

        public InvoiceEmailController(AppDbContext db, IEmailService email)
        {
            _db = db;
            _email = email;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendInvoiceEmail([FromBody] SendInvoiceEmailRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? toEmail = req.ToEmail;

            // If customerId provided, lookup email from DB
            if (toEmail == null && req.CustomerId.HasValue)
            {
                var customer = await _db.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == req.CustomerId.Value);

                if (customer == null) return NotFound(new { message = "Customer not found." });

                toEmail = customer.Email;
            }

            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest(new { message = "Provide ToEmail or CustomerId." });

            await _email.SendAsync(toEmail, req.Subject, req.Body);

            return Ok(new { message = "Invoice email sent successfully." });
        }
    }
}