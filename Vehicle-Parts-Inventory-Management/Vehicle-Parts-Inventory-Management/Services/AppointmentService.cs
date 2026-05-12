using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _db;
        public AppointmentService(AppDbContext db) => _db = db;

        public async Task<Appointment> CreateAsync(CreateAppointmentRequest req)
        {
            if (req.AppointmentDateTime < DateTime.Now)
                throw new ArgumentException("Appointment date/time cannot be in the past.");

            var appt = new Appointment
            {
                CustomerId = req.CustomerId,
                VehicleId = req.VehicleId,
                AppointmentDateTime = req.AppointmentDateTime,
                ServiceType = req.ServiceType,
                Notes = req.Notes,
                Status = AppointmentStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.Appointments.Add(appt);
            await _db.SaveChangesAsync();
            return appt;
        }

        public Task<List<Appointment>> GetByCustomerAsync(int customerId)
        {
            return _db.Appointments
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }
    }
}