
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _db;

        public StaffService(AppDbContext db) => _db = db;

        public async Task<List<StaffResponse>> GetAllAsync()
        {
            return await _db.Staff
                .Select(s => new StaffResponse
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    Role = s.Role,
                    PhoneNumber = s.PhoneNumber,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt
                }).ToListAsync();
        }

        public async Task<StaffResponse?> GetByIdAsync(int id)
        {
            var s = await _db.Staff.FindAsync(id);
            if (s == null) return null;
            return new StaffResponse { Id = s.Id, FullName = s.FullName, /* ... */ };
        }

        public async Task<StaffResponse> CreateAsync(StaffCreateRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var exists = await _db.Staff.AnyAsync(s => s.Email.ToLower() == normalizedEmail);
            if (exists)
            {
                throw new InvalidOperationException("A staff account with this email already exists.");
            }

            var staff = new Staff
            {
                FullName = request.FullName,
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                PhoneNumber = request.PhoneNumber
            };
            _db.Staff.Add(staff);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                // Handles concurrent create requests that bypassed the pre-check.
                throw new InvalidOperationException("A staff account with this email already exists.");
            }
            return new StaffResponse { Id = staff.Id, FullName = staff.FullName, /* ... */ };
        }

        public async Task<StaffResponse?> UpdateAsync(int id, StaffUpdateRequest request)
        {
            var staff = await _db.Staff.FindAsync(id);
            if (staff == null) return null;
            staff.FullName = request.FullName;
            staff.Role = request.Role;
            staff.PhoneNumber = request.PhoneNumber;
            staff.IsActive = request.IsActive;
            await _db.SaveChangesAsync();
            return new StaffResponse { Id = staff.Id, FullName = staff.FullName, /* ... */ };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var staff = await _db.Staff.FindAsync(id);
            if (staff == null) return false;
            _db.Staff.Remove(staff);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
