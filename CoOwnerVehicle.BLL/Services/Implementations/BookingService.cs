using Microsoft.EntityFrameworkCore;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingSchedule?> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingRepository.GetByIdAsync(bookingId);
        }

        public async Task<List<BookingSchedule>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<List<BookingSchedule>> GetBookingsByUserIdAsync(int userId)
        {
            return await _bookingRepository.GetByUserIdAsync(userId);
        }

        public async Task<List<BookingSchedule>> GetBookingsByVehicleIdAsync(int vehicleId)
        {
            return await _bookingRepository.GetByVehicleIdAsync(vehicleId);
        }

        public async Task<List<BookingSchedule>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _bookingRepository.GetByDateRangeAsync(startDate, endDate);
        }

        public async Task<List<BookingSchedule>> GetPendingBookingsAsync()
        {
            return await _bookingRepository.GetPendingAsync();
        }

        public async Task<List<BookingSchedule>> GetConfirmedBookingsAsync()
        {
            return await _bookingRepository.GetConfirmedAsync();
        }

        public async Task<BookingSchedule> CreateBookingAsync(BookingSchedule booking)
        {
            booking.CreatedAt = DateTime.UtcNow;
            booking.Status = BookingStatus.Pending;
            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();
            return booking;
        }

        public async Task<BookingSchedule> UpdateBookingAsync(BookingSchedule booking)
        {
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;
            _bookingRepository.Remove(booking);
            await _bookingRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmBookingAsync(int bookingId, int confirmedByUserId)
        {
            return await _bookingRepository.ConfirmAsync(bookingId, confirmedByUserId);
        }

        public async Task<bool> CancelBookingAsync(int bookingId, string reason)
        {
            return await _bookingRepository.CancelAsync(bookingId, reason);
        }

        public async Task<bool> CompleteBookingAsync(int bookingId)
        {
            return await _bookingRepository.CompleteAsync(bookingId);
        }

        // Availability & Conflicts
        public async Task<bool> IsTimeSlotAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            return await _bookingRepository.IsTimeSlotAvailableAsync(vehicleId, startTime, endTime, excludeBookingId);
        }

        public async Task<List<BookingSchedule>> GetConflictingBookingsAsync(int vehicleId, DateTime startTime, DateTime endTime)
        {
            return await _bookingRepository.GetConflictingBookingsAsync(vehicleId, startTime, endTime);
        }

        public async Task<List<DateTime>> GetAvailableTimeSlotsAsync(int vehicleId, DateTime date, int durationMinutes = 60)
        {
            return await _bookingRepository.GetAvailableTimeSlotsAsync(vehicleId, date, durationMinutes);
        }

        // Priority & Fairness
        public async Task<List<BookingSchedule>> GetBookingsByPriorityAsync(PriorityLevel priority)
        {
            return await _bookingRepository.GetByPriorityAsync(priority);
        }

        public async Task<bool> CanUserBookAsync(int userId, int vehicleId, DateTime startTime, DateTime endTime)
        {
            return await _bookingRepository.CanUserBookAsync(userId, vehicleId, startTime, endTime);
        }

        public async Task<decimal> GetUserBookingScoreAsync(int userId, int groupId, DateTime startDate, DateTime endDate)
        {
            return await _bookingRepository.GetUserBookingScoreAsync(userId, groupId, startDate, endDate);
        }
    }
}
