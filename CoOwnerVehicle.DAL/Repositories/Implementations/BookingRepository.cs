using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<BookingSchedule> _dbSet;

        public BookingRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<BookingSchedule>();
        }

        public IQueryable<BookingSchedule> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<BookingSchedule?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .FirstOrDefaultAsync(bs => bs.BookingScheduleId == bookingId, cancellationToken);
        }

        public async Task<List<BookingSchedule>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .OrderByDescending(bs => bs.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BookingSchedule>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.UserId == userId)
                .OrderByDescending(bs => bs.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BookingSchedule>> GetByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.VehicleId == vehicleId)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BookingSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.StartTime >= startDate && bs.EndTime <= endDate)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BookingSchedule>> GetPendingAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.Status == BookingStatus.Pending)
                .OrderBy(bs => bs.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BookingSchedule>> GetConfirmedAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Include(bs => bs.ConfirmedByUser)
                .Where(bs => bs.Status == BookingStatus.Confirmed)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(BookingSchedule booking, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(booking, cancellationToken);
        }

        public void Update(BookingSchedule booking)
        {
            _dbSet.Update(booking);
        }

        public void Remove(BookingSchedule booking)
        {
            _dbSet.Remove(booking);
        }

        public async Task<bool> ConfirmAsync(int bookingId, int confirmedByUserId, CancellationToken cancellationToken = default)
        {
            var booking = await _dbSet.FindAsync([bookingId], cancellationToken);
            if (booking == null) return false;
            booking.Status = BookingStatus.Confirmed;
            booking.ConfirmedAt = DateTime.UtcNow;
            booking.ConfirmedBy = confirmedByUserId;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> CancelAsync(int bookingId, string reason, CancellationToken cancellationToken = default)
        {
            var booking = await _dbSet.FindAsync([bookingId], cancellationToken);
            if (booking == null) return false;
            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancellationReason = reason;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> CompleteAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            var booking = await _dbSet.FindAsync([bookingId], cancellationToken);
            if (booking == null) return false;
            booking.Status = BookingStatus.Completed;
            booking.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime, int? excludeBookingId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(bs => bs.VehicleId == vehicleId && (bs.Status == BookingStatus.Confirmed || bs.Status == BookingStatus.InUse) && bs.StartTime < endTime && bs.EndTime > startTime);
            if (excludeBookingId.HasValue)
                query = query.Where(bs => bs.BookingScheduleId != excludeBookingId.Value);
            var count = await query.CountAsync(cancellationToken);
            return count == 0;
        }

        public async Task<List<BookingSchedule>> GetConflictingBookingsAsync(int vehicleId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.User)
                .Where(bs => bs.VehicleId == vehicleId && (bs.Status == BookingStatus.Confirmed || bs.Status == BookingStatus.InUse) && bs.StartTime < endTime && bs.EndTime > startTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DateTime>> GetAvailableTimeSlotsAsync(int vehicleId, DateTime date, int durationMinutes = 60, CancellationToken cancellationToken = default)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            var duration = TimeSpan.FromMinutes(durationMinutes);

            var existingBookings = await _dbSet
                .Where(bs => bs.VehicleId == vehicleId && (bs.Status == BookingStatus.Confirmed || bs.Status == BookingStatus.InUse) && bs.StartTime >= startOfDay && bs.EndTime <= endOfDay)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync(cancellationToken);

            var availableSlots = new List<DateTime>();
            var currentTime = startOfDay.AddHours(8);
            var endTime = startOfDay.AddHours(22);

            foreach (var booking in existingBookings)
            {
                if (currentTime.Add(duration) <= booking.StartTime)
                {
                    availableSlots.Add(currentTime);
                }
                currentTime = booking.EndTime;
            }

            if (currentTime.Add(duration) <= endTime)
            {
                availableSlots.Add(currentTime);
            }

            return availableSlots;
        }

        public async Task<List<BookingSchedule>> GetByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(bs => bs.Vehicle)
                .Include(bs => bs.User)
                .Where(bs => bs.Priority == priority)
                .OrderBy(bs => bs.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CanUserBookAsync(int userId, int vehicleId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
        {
            var userGroups = await _context.GroupMembers
                .Where(gm => gm.UserId == userId && gm.Status == MemberStatus.Active)
                .Select(gm => gm.CoOwnerGroupId)
                .ToListAsync(cancellationToken);

            var vehicleGroups = await _context.CoOwnerGroups
                .Where(g => g.VehicleId == vehicleId && g.Status == GroupStatus.Active)
                .Select(g => g.CoOwnerGroupId)
                .ToListAsync(cancellationToken);

            return userGroups.Any(ug => vehicleGroups.Contains(ug));
        }

        public async Task<decimal> GetUserBookingScoreAsync(int userId, int groupId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var userBookings = await _dbSet
                .Include(bs => bs.Vehicle)
                .ThenInclude(v => v.CoOwnerGroups)
                .Where(bs => bs.UserId == userId && bs.Vehicle.CoOwnerGroups.Any(g => g.CoOwnerGroupId == groupId) && bs.StartTime >= startDate && bs.EndTime <= endDate)
                .ToListAsync(cancellationToken);

            var totalHours = userBookings.Sum(bs => (bs.EndTime - bs.StartTime).TotalHours);
            var userOwnership = await _context.OwnershipShares
                .Where(os => os.CoOwnerGroupId == groupId && os.UserId == userId && os.IsActive)
                .SumAsync(os => os.Percentage, cancellationToken);

            return userOwnership > 0 ? (decimal)totalHours / (userOwnership / 100) : 0;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}


