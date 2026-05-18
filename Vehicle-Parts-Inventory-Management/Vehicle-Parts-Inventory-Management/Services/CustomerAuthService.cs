using Microsoft.EntityFrameworkCore;
using Npgsql;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Helpers;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;

        public CustomerAuthService(AppDbContext db, IEmailService email, IConfiguration config)
        {
            _db = db;
            _email = email;
            _config = config;
        }

        // REGISTER (sending verification email; login only after verified)
        public async Task<(bool Success, string Message)> RegisterAsync(CustomerRegisterRequest request)
        {
            try
            {
                var email = request.Email.ToLower().Trim();

                bool emailExists = await _db.Customers.AnyAsync(c => c.Email == email);
                if (emailExists)
                    return (false, "An account with this email already exists.");

                // Generate verification token (store only HASH)
                var token = EmailVerificationToken.GenerateToken();
                var tokenHash = EmailVerificationToken.HashToken(token);

                var customer = new Customer
                {
                    FullName = request.FullName.Trim(),
                    Email = email,
                    Phone = request.Phone.Trim(),
                    Address = request.Address.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    RegisteredAt = DateTime.UtcNow,

                    // Email verification fields
                    IsEmailVerified = false,
                    EmailVerificationTokenHash = tokenHash,
                    EmailVerificationTokenExpiresUtc = DateTime.UtcNow.AddHours(24)
                };

                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();

                // Build verification link
                var baseUrl = _config["AppUrls:BackendBaseUrl"] ?? "https://localhost:7041";
                var link = $"{baseUrl}/api/customer-auth/verify-email?token={token}";

                // Send verification email
                await _email.SendAsync(
                    customer.Email,
                    "Verify your email address",
                    $"Hi {customer.FullName},\n\nPlease verify your email by clicking the link below:\n{link}\n\nThis link expires in 24 hours.\n"
                );

                return (true, "Registration successful. Please verify your email before logging in.");
            }
            catch (Exception ex)
            {
                return (false, "Registration failed: " + ex.Message);
            }
        }

        // LOGIN (blocked if not verified)
        public async Task<(bool Success, Customer? Customer, string Message)> LoginAsync(LoginRequest request)
        {
            try
            {
                var email = request.Email.ToLower().Trim();

                var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == email);
                if (customer == null)
                    return (false, null, "Invalid email or password.");

                if (string.IsNullOrWhiteSpace(customer.PasswordHash))
                    return (false, null, "This customer account does not have portal access yet. Please register for customer portal access first.");

                bool valid = BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash);
                if (!valid)
                    return (false, null, "Invalid email or password.");

                return (true, customer, "Login successful.");
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UndefinedColumn)
            {
                return (false, null, "Database is missing the customer email verification columns. Please run the latest EF Core migration and try again.");
            }
            catch (Exception)
            {
                return (false, null, "Customer login failed because of a server error. Please try again later.");
            }
        }

        // VERIFY EMAIL 
        public async Task<(bool Success, string Message)> VerifyEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return (false, "Token is required.");

            var tokenHash = EmailVerificationToken.HashToken(token);

            var customer = await _db.Customers.FirstOrDefaultAsync(c =>
                c.EmailVerificationTokenHash == tokenHash &&
                c.EmailVerificationTokenExpiresUtc != null &&
                c.EmailVerificationTokenExpiresUtc > DateTime.UtcNow);

            if (customer == null)
                return (false, "Invalid or expired verification link.");

            customer.IsEmailVerified = true;
            customer.EmailVerificationTokenHash = null;
            customer.EmailVerificationTokenExpiresUtc = null;

            await _db.SaveChangesAsync();
            return (true, "Email verified successfully. You can now log in.");
        }

        // GET PROFILE
        public async Task<Customer?> GetProfileAsync(int customerId)
        {
            return await _db.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        // UPDATE PROFILE
        public async Task<(bool Success, string Message)> UpdateProfileAsync(int customerId, UpdateProfileRequest request)
        {
            try
            {
                var customer = await _db.Customers.FindAsync(customerId);
                if (customer == null)
                    return (false, "Customer not found.");

                customer.FullName = request.FullName.Trim();
                customer.Phone = request.Phone.Trim();
                customer.Address = request.Address.Trim();

                await _db.SaveChangesAsync();
                return (true, "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, "Update failed: " + ex.Message);
            }
        }

        // CHANGE PASSWORD
        public async Task<(bool Success, string Message)> ChangePasswordAsync(int customerId, ChangePasswordRequest request)
        {
            try
            {
                var customer = await _db.Customers.FindAsync(customerId);
                if (customer == null)
                    return (false, "Customer not found.");

                bool valid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, customer.PasswordHash);
                if (!valid)
                    return (false, "Current password is incorrect.");

                customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _db.SaveChangesAsync();

                return (true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                return (false, "Password change failed: " + ex.Message);
            }
        }

        // GET VEHICLES
        public async Task<List<Vehicle>> GetVehiclesAsync(int customerId)
        {
            return await _db.Vehicles
                .Where(v => v.CustomerId == customerId)
                .ToListAsync();
        }

        // ADD VEHICLE
        public async Task<(bool Success, string Message)> AddVehicleAsync(int customerId, VehicleRequest request)
        {
            try
            {
                bool numberExists = await _db.Vehicles
                    .AnyAsync(v => v.VehicleNumber == request.VehicleNumber.ToUpper().Trim());

                if (numberExists)
                    return (false, "A vehicle with this number already exists.");

                var vehicle = new Vehicle
                {
                    CustomerId = customerId,
                    VehicleNumber = request.VehicleNumber.ToUpper().Trim(),
                    Make = request.Make.Trim(),
                    Model = request.Model.Trim(),
                    Year = request.Year,
                    Color = request.Color.Trim()
                };

                _db.Vehicles.Add(vehicle);
                await _db.SaveChangesAsync();

                return (true, "Vehicle added successfully.");
            }
            catch (Exception ex)
            {
                return (false, "Failed to add vehicle: " + ex.Message);
            }
        }

        // UPDATE VEHICLE
        public async Task<(bool Success, string Message)> UpdateVehicleAsync(int customerId, int vehicleId, VehicleRequest request)
        {
            try
            {
                var vehicle = await _db.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == vehicleId && v.CustomerId == customerId);

                if (vehicle == null)
                    return (false, "Vehicle not found or you do not have permission to edit it.");

                bool numberExists = await _db.Vehicles
                    .AnyAsync(v => v.VehicleNumber == request.VehicleNumber.ToUpper().Trim()
                               && v.Id != vehicleId);

                if (numberExists)
                    return (false, "A vehicle with this number already exists.");

                vehicle.VehicleNumber = request.VehicleNumber.ToUpper().Trim();
                vehicle.Make = request.Make.Trim();
                vehicle.Model = request.Model.Trim();
                vehicle.Year = request.Year;
                vehicle.Color = request.Color.Trim();

                await _db.SaveChangesAsync();
                return (true, "Vehicle updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, "Failed to update vehicle: " + ex.Message);
            }
        }

        // DELETE VEHICLE
        public async Task<(bool Success, string Message)> DeleteVehicleAsync(int customerId, int vehicleId)
        {
            try
            {
                var vehicle = await _db.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == vehicleId && v.CustomerId == customerId);

                if (vehicle == null)
                    return (false, "Vehicle not found or you do not have permission to delete it.");

                _db.Vehicles.Remove(vehicle);
                await _db.SaveChangesAsync();

                return (true, "Vehicle deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, "Failed to delete vehicle: " + ex.Message);
            }
        }

        // GET PURCHASE HISTORY
        public async Task<List<PurchaseHistory>> GetPurchaseHistoryAsync(int customerId)
        {
            return await _db.PurchaseHistories
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.PurchasedAt)
                .ToListAsync();
        }

        // GET SERVICE HISTORY
        public async Task<List<ServiceHistory>> GetServiceHistoryAsync(int customerId)
        {
            return await _db.ServiceHistories
                .Where(s => s.CustomerId == customerId)
                .OrderByDescending(s => s.ServiceDate)
                .ToListAsync();
        }
    }
}
