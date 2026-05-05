using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class ServiceReviewService : IServiceReviewService
    {
        private readonly AppDbContext _db;
        public ServiceReviewService(AppDbContext db) => _db = db;

        public async Task<ServiceReview> CreateAsync(CreateServiceReviewRequest req)
        {
            // Optional: ensure appointment exists if AppointmentId provided
            if (req.AppointmentId.HasValue)
            {
                var exists = await _db.Appointments.AnyAsync(a => a.AppointmentId == req.AppointmentId.Value);
                if (!exists) throw new ArgumentException("Appointment not found.");
            }

            var review = new ServiceReview
            {
                CustomerId = req.CustomerId,
                AppointmentId = req.AppointmentId,
                Rating = req.Rating,
                Comment = req.Comment,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.ServiceReviews.Add(review);
            await _db.SaveChangesAsync();
            return review;
        }

        public Task<List<ServiceReview>> GetByCustomerAsync(int customerId)
        {
            return _db.ServiceReviews
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync();
        }
    }
}