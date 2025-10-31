using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;
using CoOwnerVehicle.DAL.Repositories.Interfaces;

namespace Co_owner_Vehicle.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicles;
        private readonly IBookingRepository _bookings;

        public VehicleService(IVehicleRepository vehicles, IBookingRepository bookings)
        {
            _vehicles = vehicles;
            _bookings = bookings;
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int vehicleId)
        {
            return await _vehicles.GetQueryable()
                .Include(v => v.CoOwnerGroups)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }

        public async Task<Vehicle?> GetVehicleByLicensePlateAsync(string licensePlate)
        {
            return await _vehicles.GetQueryable()
                .Include(v => v.CoOwnerGroups)
                .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehicles.GetQueryable()
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.Status != VehicleStatus.Sold)
                .OrderBy(v => v.Brand)
                .ThenBy(v => v.Model)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByTypeAsync(VehicleType vehicleType)
        {
            return await _vehicles.GetQueryable()
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.VehicleType == vehicleType && v.Status != VehicleStatus.Sold)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status)
        {
            return await _vehicles.GetQueryable()
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.Status == status)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetAvailableVehiclesAsync(DateTime startTime, DateTime endTime)
        {
            var conflictingVehicleIds = await _bookings.GetConflictingBookingsAsync(0, startTime, endTime);
            var conflictingIds = conflictingVehicleIds.Select(b => b.VehicleId).Distinct().ToList();

            return await _vehicles.GetQueryable()
                .Include(v => v.CoOwnerGroups)
                .Where(v => v.Status == VehicleStatus.Active && !conflictingIds.Contains(v.VehicleId))
                .ToListAsync();
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.Status = VehicleStatus.Active;

            await _vehicles.AddAsync(vehicle);
            await _vehicles.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
        {
            vehicle.UpdatedAt = DateTime.UtcNow;
            _vehicles.Update(vehicle);
            await _vehicles.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> DeleteVehicleAsync(int vehicleId)
        {
            var vehicle = await _vehicles.GetByIdAsync(vehicleId);
            if (vehicle == null) return false;

            vehicle.Status = VehicleStatus.Sold;
            vehicle.UpdatedAt = DateTime.UtcNow;
            _vehicles.Update(vehicle);
            await _vehicles.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVehicleMileageAsync(int vehicleId, int newMileage)
        {
            var vehicle = await _vehicles.GetByIdAsync(vehicleId);
            if (vehicle == null) return false;

            vehicle.CurrentMileage = newMileage;
            vehicle.UpdatedAt = DateTime.UtcNow;
            _vehicles.Update(vehicle);
            await _vehicles.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVehicleStatusAsync(int vehicleId, VehicleStatus status)
        {
            var vehicle = await _vehicles.GetByIdAsync(vehicleId);
            if (vehicle == null) return false;

            vehicle.Status = status;
            vehicle.UpdatedAt = DateTime.UtcNow;
            _vehicles.Update(vehicle);
            await _vehicles.SaveChangesAsync();
            return true;
        }

        public async Task<List<Vehicle>> SearchVehiclesAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _vehicles.GetQueryable()
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
            var available = await _bookings.IsTimeSlotAvailableAsync(vehicleId, startTime, endTime, null);
            return available;
        }

        public async Task<List<BookingSchedule>> GetVehicleBookingsAsync(int vehicleId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var list = await _bookings.GetByVehicleIdAsync(vehicleId);
            if (startDate.HasValue) list = list.Where(bs => bs.StartTime >= startDate.Value).ToList();
            if (endDate.HasValue) list = list.Where(bs => bs.EndTime <= endDate.Value).ToList();
            return list.OrderBy(bs => bs.StartTime).ToList();
        }

        public async Task<List<Vehicle>> GetRecentVehiclesAsync(int count = 10)
        {
            return await _vehicles.GetQueryable()
                .OrderByDescending(v => v.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
