using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
    public class BookingService : IBookingService
    {
        private readonly CoOwnerVehicleDbContext _context;

        public BookingService(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public async Task<BookingSchedule?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .FirstOrDefaultAsync(bs => bs.BookingScheduleId == bookingId);
        }

        public async Task<List<BookingSchedule>> GetAllBookingsAsync()
        {
            return await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .OrderByDescending(bs => bs.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BookingSchedule>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.UserId == userId)
                .OrderByDescending(bs => bs.StartTime)
                .ToListAsync();
        }

        public async Task<List<BookingSchedule>> GetBookingsByVehicleIdAsync(int vehicleId)
        {
            return await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.VehicleId == vehicleId)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync();
        }

        public async Task<List<BookingSchedule>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.StartTime >= startDate && bs.EndTime <= endDate)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync();
        }

        public async Task<List<BookingSchedule>> GetPendingBookingsAsync()
        {
            return await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.Status == BookingStatus.Pending)
                .OrderBy(bs => bs.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BookingSchedule>> GetConfirmedBookingsAsync()
        {
            return await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.Status == BookingStatus.Confirmed)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync();
        }

        public async Task<BookingSchedule> CreateBookingAsync(BookingSchedule booking)
        {
            booking.CreatedAt = DateTime.UtcNow;
            booking.Status = BookingStatus.Pending;

            _context.BookingSchedules.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<BookingSchedule> UpdateBookingAsync(BookingSchedule booking)
        {
            _context.BookingSchedules.Update(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.BookingSchedules.FindAsync(bookingId);
            if (booking == null) return false;

            _context.BookingSchedules.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmBookingAsync(int bookingId, int confirmedByUserId)
        {
            var booking = await _context.BookingSchedules.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = BookingStatus.Confirmed;
            booking.ConfirmedAt = DateTime.UtcNow;
            booking.ConfirmedBy = confirmedByUserId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBookingAsync(int bookingId, string reason)
        {
            var booking = await _context.BookingSchedules.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancellationReason = reason;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteBookingAsync(int bookingId)
        {
            var booking = await _context.BookingSchedules.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = BookingStatus.Completed;
            booking.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // Availability & Conflicts
        public async Task<bool> IsTimeSlotAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            var query = _context.BookingSchedules
                .Where(bs => bs.VehicleId == vehicleId &&
                           (bs.Status == BookingStatus.Confirmed || bs.Status == BookingStatus.InUse) &&
                           bs.StartTime < endTime && bs.EndTime > startTime);

            if (excludeBookingId.HasValue)
                query = query.Where(bs => bs.BookingScheduleId != excludeBookingId.Value);

            var conflictingBookings = await query.CountAsync();
            return conflictingBookings == 0;
        }

        public async Task<List<BookingSchedule>> GetConflictingBookingsAsync(int vehicleId, DateTime startTime, DateTime endTime)
        {
            return await _context.BookingSchedules
                .Include(bs => bs.User)
                .Where(bs => bs.VehicleId == vehicleId &&
                           (bs.Status == BookingStatus.Confirmed || bs.Status == BookingStatus.InUse) &&
                           bs.StartTime < endTime && bs.EndTime > startTime)
                .ToListAsync();
        }

        public async Task<List<DateTime>> GetAvailableTimeSlotsAsync(int vehicleId, DateTime date, int durationMinutes = 60)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            var duration = TimeSpan.FromMinutes(durationMinutes);

            var existingBookings = await _context.BookingSchedules
                .Where(bs => bs.VehicleId == vehicleId &&
                           (bs.Status == BookingStatus.Confirmed || bs.Status == BookingStatus.InUse) &&
                           bs.StartTime >= startOfDay && bs.EndTime <= endOfDay)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync();

            var availableSlots = new List<DateTime>();
            var currentTime = startOfDay.AddHours(8); // Start from 8 AM
            var endTime = startOfDay.AddHours(22); // End at 10 PM

            foreach (var booking in existingBookings)
            {
                // Add available slot before this booking
                if (currentTime.Add(duration) <= booking.StartTime)
                {
                    availableSlots.Add(currentTime);
                }
                currentTime = booking.EndTime;
            }

            // Add available slot after last booking
            if (currentTime.Add(duration) <= endTime)
            {
                availableSlots.Add(currentTime);
            }

            return availableSlots;
        }

        // Priority & Fairness
        public async Task<List<BookingSchedule>> GetBookingsByPriorityAsync(PriorityLevel priority)
        {
            return await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Where(bs => bs.Priority == priority)
                .OrderBy(bs => bs.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CanUserBookAsync(int userId, int vehicleId, DateTime startTime, DateTime endTime)
        {
            // Check if user is in any group that owns this vehicle
            var userGroups = await _context.GroupMembers
                .Where(gm => gm.UserId == userId && gm.Status == MemberStatus.Active)
                .Select(gm => gm.CoOwnerGroupId)
                .ToListAsync();

            var vehicleGroups = await _context.CoOwnerGroups
                .Where(g => g.VehicleId == vehicleId && g.Status == GroupStatus.Active)
                .Select(g => g.CoOwnerGroupId)
                .ToListAsync();

            return userGroups.Any(ug => vehicleGroups.Contains(ug));
        }

        public async Task<decimal> GetUserBookingScoreAsync(int userId, int groupId, DateTime startDate, DateTime endDate)
        {
            var userBookings = await _context.BookingSchedules
                .Include(bs => bs.Vehicle)
                .ThenInclude(v => v.CoOwnerGroups)
                .Where(bs => bs.UserId == userId &&
                           bs.Vehicle.CoOwnerGroups.Any(g => g.CoOwnerGroupId == groupId) &&
                           bs.StartTime >= startDate && bs.EndTime <= endDate)
                .ToListAsync();

            var totalHours = userBookings.Sum(bs => (bs.EndTime - bs.StartTime).TotalHours);
            var userOwnership = await _context.OwnershipShares
                .Where(os => os.CoOwnerGroupId == groupId && os.UserId == userId && os.IsActive)
                .SumAsync(os => os.Percentage);

            return userOwnership > 0 ? (decimal)totalHours / (userOwnership / 100) : 0;
        }
    }
}
