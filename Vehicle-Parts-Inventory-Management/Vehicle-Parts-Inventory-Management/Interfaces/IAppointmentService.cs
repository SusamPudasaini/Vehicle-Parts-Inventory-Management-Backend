using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAsync(CreateAppointmentRequest req);
        Task<List<Appointment>> GetByCustomerAsync(int customerId);
    }
}