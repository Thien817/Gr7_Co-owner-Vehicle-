using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly CoOwnerVehicleDbContext _context;

        public VehicleService(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int vehicleId)
        {
            return await _context.Vehicles
                .Include(v => v.CoOwnerGroups)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }

        public async Task<Vehicle?> GetVehicleByLicensePlateAsync(string licensePlate)
        {
            return await _context.Vehicles
                .Include(v => v.CoOwnerGroups)
                .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.Status != VehicleStatus.Sold)
                .OrderBy(v => v.Brand)
                .ThenBy(v => v.Model)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByTypeAsync(VehicleType vehicleType)
        {
            return await _context.Vehicles
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.VehicleType == vehicleType && v.Status != VehicleStatus.Sold)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status)
        {
            return await _context.Vehicles
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.Status == status)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetAvailableVehiclesAsync(DateTime startTime, DateTime endTime)
        {
            var conflictingVehicleIds = await _context.BookingSchedules
                .Where(bs => bs.Status == BookingStatus.Confirmed || bs.Status == BookingStatus.InUse)
                .Where(bs => (bs.StartTime < endTime && bs.EndTime > startTime))
                .Select(bs => bs.VehicleId)
                .Distinct()
                .ToListAsync();

            return await _context.Vehicles
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.Status == VehicleStatus.Active && !conflictingVehicleIds.Contains(v.VehicleId))
                .ToListAsync();
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.Status = VehicleStatus.Active;

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
        {
            vehicle.UpdatedAt = DateTime.UtcNow;
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> DeleteVehicleAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null) return false;

            vehicle.Status = VehicleStatus.Sold;
            vehicle.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVehicleMileageAsync(int vehicleId, int newMileage)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null) return false;

            vehicle.CurrentMileage = newMileage;
            vehicle.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVehicleStatusAsync(int vehicleId, VehicleStatus status)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null) return false;

            vehicle.Status = status;
            vehicle.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Vehicle>> SearchVehiclesAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _context.Vehicles
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.Status != VehicleStatus.Sold &&
                           (v.Brand.ToLower().Contains(term) ||
                            v.Model.ToLower().Contains(term) ||
                            v.LicensePlate.ToLower().Contains(term) ||
                            (v.Color != null && v.Color.ToLower().Contains(term))))
                .ToListAsync();
        }

        public async Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime)
        {
            var conflictingBookings = await _context.BookingSchedules
                .Where(bs => bs.VehicleId == vehicleId &&
                           (bs.Status == BookingStatus.Confirmed || bs.Status == BookingStatus.InUse) &&
                           bs.StartTime < endTime && bs.EndTime > startTime)
                .CountAsync();

            return conflictingBookings == 0;
        }

        public async Task<List<BookingSchedule>> GetVehicleBookingsAsync(int vehicleId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.BookingSchedules
                .Include(bs => bs.User)
                .Where(bs => bs.VehicleId == vehicleId);

            if (startDate.HasValue)
                query = query.Where(bs => bs.StartTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(bs => bs.EndTime <= endDate.Value);

            return await query
                .OrderBy(bs => bs.StartTime)
                .ToListAsync();
        }
    }
}
