using System.ComponentModel.DataAnnotations;

namespace Vehicle_Parts_Inventory_Management.DTOs.Requests
{
    public class SendInvoiceEmailRequest
    {
        public int? CustomerId { get; set; }  // helps in getting lookup email from DB

        [EmailAddress]
        public string? ToEmail { get; set; }  // for direct email

        [Required]
        public string Subject { get; set; } = "Invoice";

        [Required]
        public string Body { get; set; } = ""; // plain text for now
    }
}