using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly AppDbContext _db;

        public CustomerAuthService(AppDbContext db)
        {
            _db = db;
        }

        // REGISTER 
        public async Task<(bool Success, string Message)> RegisterAsync(CustomerRegisterRequest request)
        {
            try
            {
                bool emailExists = await _db.Customers
                    .AnyAsync(c => c.Email == request.Email.ToLower().Trim());

                if (emailExists)
                    return (false, "An account with this email already exists.");

                var customer = new Customer
                {
                    FullName = request.FullName.Trim(),
                    Email = request.Email.ToLower().Trim(),
                    Phone = request.Phone.Trim(),
                    Address = request.Address.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    RegisteredAt = DateTime.UtcNow
                };

                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();

                return (true, "Registration successful. You can now log in.");
            }
            catch (Exception ex)
            {
                return (false, "Registration failed: " + ex.Message);
            }
        }

        // LOGIN 
        public async Task<Customer?> LoginAsync(LoginRequest request)
        {
            try
            {
                var customer = await _db.Customers
                    .FirstOrDefaultAsync(c => c.Email == request.Email.ToLower().Trim());

                if (customer == null) return null;

                bool valid = BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash);
                return valid ? customer : null;
            }
            catch
            {
                return null;
            }
        }

        // GET PROFILE 
        public async Task<Customer?> GetProfileAsync(int customerId)
        {
            return await _db.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        //  UPDATE PROFILE 
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

        //  CHANGE PASSWORD 
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

        //  GET VEHICLES 
        public async Task<List<Vehicle>> GetVehiclesAsync(int customerId)
        {
            return await _db.Vehicles
                .Where(v => v.CustomerId == customerId)
                .ToListAsync();
        }

        //  ADD VEHICLE 
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

        //  DELETE VEHICLE 
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
    }
}