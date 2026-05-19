using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
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
            var appointmentTime = req.AppointmentDateTime.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(req.AppointmentDateTime, DateTimeKind.Local)
                : req.AppointmentDateTime;
            var appointmentUtc = appointmentTime.ToUniversalTime();

            if (appointmentUtc < DateTime.UtcNow)
                throw new ArgumentException("Appointment date/time cannot be in the past.");

            var appt = new Appointment
            {
                CustomerId = req.CustomerId,
                VehicleId = req.VehicleId,
                AppointmentDateTime = appointmentUtc,
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

        public Task<List<AppointmentSummaryResponse>> GetAllAsync()
        {
            return (from a in _db.Appointments
                    join c in _db.Customers on a.CustomerId equals c.Id
                    join v in _db.Vehicles on a.VehicleId equals v.Id into vehicleGroup
                    from v in vehicleGroup.DefaultIfEmpty()
                    orderby a.AppointmentDateTime descending
                    select new AppointmentSummaryResponse
                    {
                        AppointmentId = a.AppointmentId,
                        CustomerId = c.Id,
                        CustomerName = c.FullName,
                        CustomerEmail = c.Email,
                        CustomerPhone = c.Phone,
                        VehicleId = v != null ? v.Id : null,
                        VehicleNumber = v != null ? v.VehicleNumber : null,
                        VehicleMake = v != null ? v.Make : null,
                        VehicleModel = v != null ? v.Model : null,
                        AppointmentDateTime = a.AppointmentDateTime,
                        ServiceType = a.ServiceType,
                        Notes = a.Notes,
                        Status = a.Status
                    }).ToListAsync();
        }

        public async Task<Appointment?> UpdateStatusAsync(int appointmentId, AppointmentStatus status)
        {
            var appt = await _db.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            if (appt == null)
                return null;

            appt.Status = status;
            await _db.SaveChangesAsync();
            return appt;
        }

        public async Task<Appointment?> RescheduleAsync(int appointmentId, DateTime appointmentDateTime)
        {
            var appt = await _db.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            if (appt == null)
                return null;

            var appointmentTime = appointmentDateTime.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(appointmentDateTime, DateTimeKind.Local)
                : appointmentDateTime;
            var appointmentUtc = appointmentTime.ToUniversalTime();

            if (appointmentUtc < DateTime.UtcNow)
                throw new ArgumentException("Appointment date/time cannot be in the past.");

            appt.AppointmentDateTime = appointmentUtc;
            await _db.SaveChangesAsync();
            return appt;
        }
    }
}