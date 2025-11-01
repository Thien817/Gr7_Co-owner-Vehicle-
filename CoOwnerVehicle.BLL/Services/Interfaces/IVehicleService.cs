using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<Vehicle?> GetVehicleByIdAsync(int vehicleId);
        Task<Vehicle?> GetVehicleByLicensePlateAsync(string licensePlate);
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<List<Vehicle>> GetVehiclesByTypeAsync(VehicleType vehicleType);
        Task<List<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status);
        Task<List<Vehicle>> GetAvailableVehiclesAsync(DateTime startTime, DateTime endTime);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);
        Task<bool> DeleteVehicleAsync(int vehicleId);
        Task<bool> UpdateVehicleMileageAsync(int vehicleId, int newMileage);
        Task<bool> UpdateVehicleStatusAsync(int vehicleId, VehicleStatus status);
        Task<List<Vehicle>> SearchVehiclesAsync(string searchTerm);
        Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime);
        Task<List<BookingSchedule>> GetVehicleBookingsAsync(int vehicleId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<Vehicle>> GetRecentVehiclesAsync(int count = 10);
    }
}
