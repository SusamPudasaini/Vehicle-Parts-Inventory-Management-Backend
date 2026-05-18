namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}