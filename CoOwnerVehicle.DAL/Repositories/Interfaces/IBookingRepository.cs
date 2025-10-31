using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        IQueryable<BookingSchedule> GetQueryable();
        Task<BookingSchedule?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default);
        Task<List<BookingSchedule>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<BookingSchedule>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<List<BookingSchedule>> GetByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken = default);
        Task<List<BookingSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<List<BookingSchedule>> GetPendingAsync(CancellationToken cancellationToken = default);
        Task<List<BookingSchedule>> GetConfirmedAsync(CancellationToken cancellationToken = default);

        Task AddAsync(BookingSchedule booking, CancellationToken cancellationToken = default);
        void Update(BookingSchedule booking);
        void Remove(BookingSchedule booking);

        Task<bool> ConfirmAsync(int bookingId, int confirmedByUserId, CancellationToken cancellationToken = default);
        Task<bool> CancelAsync(int bookingId, string reason, CancellationToken cancellationToken = default);
        Task<bool> CompleteAsync(int bookingId, CancellationToken cancellationToken = default);

        Task<bool> IsTimeSlotAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime, int? excludeBookingId = null, CancellationToken cancellationToken = default);
        Task<List<BookingSchedule>> GetConflictingBookingsAsync(int vehicleId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
        Task<List<DateTime>> GetAvailableTimeSlotsAsync(int vehicleId, DateTime date, int durationMinutes = 60, CancellationToken cancellationToken = default);

        Task<List<BookingSchedule>> GetByPriorityAsync(PriorityLevel priority, CancellationToken cancellationToken = default);
        Task<bool> CanUserBookAsync(int userId, int vehicleId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
        Task<decimal> GetUserBookingScoreAsync(int userId, int groupId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}


