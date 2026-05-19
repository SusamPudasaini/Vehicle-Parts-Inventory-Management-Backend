using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.Interfaces
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAsync(CreateAppointmentRequest req);
        Task<List<Appointment>> GetByCustomerAsync(int customerId);
        Task<List<AppointmentSummaryResponse>> GetAllAsync();
        Task<Appointment?> UpdateStatusAsync(int appointmentId, AppointmentStatus status);
        Task<Appointment?> RescheduleAsync(int appointmentId, DateTime appointmentDateTime);
    }
}