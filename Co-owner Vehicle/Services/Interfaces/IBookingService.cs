using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingSchedule?> GetBookingByIdAsync(int bookingId);
        Task<List<BookingSchedule>> GetAllBookingsAsync();
        Task<List<BookingSchedule>> GetBookingsByUserIdAsync(int userId);
        Task<List<BookingSchedule>> GetBookingsByVehicleIdAsync(int vehicleId);
        Task<List<BookingSchedule>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<BookingSchedule>> GetPendingBookingsAsync();
        Task<List<BookingSchedule>> GetConfirmedBookingsAsync();
        Task<BookingSchedule> CreateBookingAsync(BookingSchedule booking);
        Task<BookingSchedule> UpdateBookingAsync(BookingSchedule booking);
        Task<bool> DeleteBookingAsync(int bookingId);
        Task<bool> ConfirmBookingAsync(int bookingId, int confirmedByUserId);
        Task<bool> CancelBookingAsync(int bookingId, string reason);
        Task<bool> CompleteBookingAsync(int bookingId);
        
        // Availability & Conflicts
        Task<bool> IsTimeSlotAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
        Task<List<BookingSchedule>> GetConflictingBookingsAsync(int vehicleId, DateTime startTime, DateTime endTime);
        Task<List<DateTime>> GetAvailableTimeSlotsAsync(int vehicleId, DateTime date, int durationMinutes = 60);
        
        // Priority & Fairness
        Task<List<BookingSchedule>> GetBookingsByPriorityAsync(PriorityLevel priority);
        Task<bool> CanUserBookAsync(int userId, int vehicleId, DateTime startTime, DateTime endTime);
        Task<decimal> GetUserBookingScoreAsync(int userId, int groupId, DateTime startDate, DateTime endDate);
    }
}
