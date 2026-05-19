using Microsoft.EntityFrameworkCore;
using Npgsql;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Helpers;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;
        private readonly ILogger<StaffService> _logger;

        public StaffService(AppDbContext db, IEmailService email, IConfiguration config, ILogger<StaffService> logger)
        {
            _db = db;
            _email = email;
            _config = config;
            _logger = logger;
        }

        public async Task<List<StaffResponse>> GetAllAsync()
        {
            return await _db.Staff
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => MapToResponse(s))
                .ToListAsync();
        }

        public async Task<StaffResponse?> GetByIdAsync(int id)
        {
            var s = await _db.Staff.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return s == null ? null : MapToResponse(s);
        }

        public async Task<StaffResponse> CreateAsync(StaffCreateRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Check duplicate email
            var exists = await _db.Staff.AnyAsync(s => s.Email == normalizedEmail);
            if (exists)
                throw new InvalidOperationException("A staff account with this email already exists.");

            // Generate verification token (store hash, not raw token)
            var rawToken = EmailVerificationToken.GenerateToken();
            var tokenHash = EmailVerificationToken.HashToken(rawToken);
            var expires = DateTime.UtcNow.AddHours(24);

            // Create staff with verification pending
            var staff = new Staff
            {
                FullName = request.FullName.Trim(),
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                IsActive = true,

                // Email verification fields
                IsEmailVerified = false,
                EmailVerificationTokenHash = tokenHash,
                EmailVerificationTokenExpiresUtc = expires,
                CreatedAt = DateTime.UtcNow
            };

            await using var tx = await _db.Database.BeginTransactionAsync();

            _db.Staff.Add(staff);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                throw new InvalidOperationException("A staff account with this email already exists.");
            }

            // Send staff to the frontend so the app can show a branded verification result.
            var frontendBaseUrl = _config["AppUrls:FrontendBaseUrl"] ?? "http://localhost:5173";
            var verifyLink = $"{frontendBaseUrl.TrimEnd('/')}/verify-email/staff?token={Uri.EscapeDataString(rawToken)}";

            try
            {
                await _email.SendAsync(
                    staff.Email,
                    "Verify your staff account email",
                    $"Hello {staff.FullName},\n\n" +
                    $"Your staff account has been created. Please verify your email using the link below:\n{verifyLink}\n\n" +
                    $"This link expires in 24 hours.\n"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send staff verification email. Rolling back staff creation.");
                await tx.RollbackAsync();
                throw new InvalidOperationException("Staff creation failed because verification email could not be sent.");
            }

            await tx.CommitAsync();

            return MapToResponse(staff);
        }

        public async Task<StaffResponse?> UpdateAsync(int id, StaffUpdateRequest request)
        {
            var staff = await _db.Staff.FindAsync(id);
            if (staff == null) return null;

            staff.FullName = request.FullName.Trim();
            staff.Role = request.Role.Trim();
            staff.PhoneNumber = request.PhoneNumber.Trim();
            staff.IsActive = request.IsActive;

            await _db.SaveChangesAsync();
            return MapToResponse(staff);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var staff = await _db.Staff.FindAsync(id);
            if (staff == null) return false;

            _db.Staff.Remove(staff);
            await _db.SaveChangesAsync();
            return true;
        }

        private static StaffResponse MapToResponse(Staff s) => new StaffResponse
        {
            Id = s.Id,
            FullName = s.FullName,
            Email = s.Email,
            Role = s.Role,
            PhoneNumber = s.PhoneNumber,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
            // StaffResponse has IsEmailVerified, then we can add this line hai else yo comment hataideu
            // IsEmailVerified = s.IsEmailVerified
        };
    }
}
