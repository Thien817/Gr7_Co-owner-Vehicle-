using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Co_owner_Vehicle.Pages.CheckInOut
{
    [Authorize(Roles = "Staff,Admin")]
    public class IndexModel : PageModel
    {
        private readonly CoOwnerVehicleDbContext _context;

        public IndexModel(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public CheckInOutStats Stats { get; set; } = new();
        public List<CheckInOutRecordViewModel> Records { get; set; } = new();
        public List<Vehicle> Vehicles { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? VehicleIdFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public CheckInOutType? TypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; } = DateTime.UtcNow.AddDays(-30);

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; } = DateTime.UtcNow;

        public async Task OnGetAsync()
        {
            await LoadVehiclesAsync();
            await LoadStatisticsAsync();
            await LoadRecordsAsync();
        }

        private async Task LoadVehiclesAsync()
        {
            Vehicles = await _context.Vehicles
                .Where(v => v.Status == VehicleStatus.Active)
                .OrderBy(v => v.Brand)
                .ThenBy(v => v.Model)
                .ToListAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // Đang sử dụng (có check-in nhưng chưa check-out)
            var activeCheckIns = await _context.CheckInOutRecords
                .Where(c => c.Type == CheckInOutType.CheckIn && 
                           c.Status == CheckInOutStatus.Completed &&
                           c.CheckTime.Date <= today)
                .Select(c => c.VehicleId)
                .ToListAsync();

            var activeCheckOuts = await _context.CheckInOutRecords
                .Where(c => c.Type == CheckInOutType.CheckOut && 
                           c.Status == CheckInOutStatus.Completed &&
                           c.CheckTime.Date <= today)
                .Select(c => c.VehicleId)
                .ToListAsync();

            var currentlyInUse = activeCheckIns.Except(activeCheckOuts).Count();

            // Check-in hôm nay
            var checkInsToday = await _context.CheckInOutRecords
                .CountAsync(c => c.Type == CheckInOutType.CheckIn && 
                               c.CheckTime.Date == today &&
                               c.Status == CheckInOutStatus.Completed);

            // Check-out hôm nay
            var checkOutsToday = await _context.CheckInOutRecords
                .CountAsync(c => c.Type == CheckInOutType.CheckOut && 
                               c.CheckTime.Date == today &&
                               c.Status == CheckInOutStatus.Completed);

            // Tổng lượt tháng này
            var totalThisMonth = await _context.CheckInOutRecords
                .CountAsync(c => c.CheckTime >= thisMonth &&
                               c.Status == CheckInOutStatus.Completed);

            Stats = new CheckInOutStats
            {
                CurrentlyInUse = currentlyInUse,
                CheckInsToday = checkInsToday,
                CheckOutsToday = checkOutsToday,
                TotalThisMonth = totalThisMonth
            };
        }

        private async Task LoadRecordsAsync()
        {
            var query = _context.CheckInOutRecords
                .Include(c => c.Vehicle)
                .Include(c => c.User)
                .Include(c => c.ProcessedByUser)
                .AsQueryable();

            // Apply filters
            if (VehicleIdFilter.HasValue)
            {
                query = query.Where(c => c.VehicleId == VehicleIdFilter.Value);
            }

            if (TypeFilter.HasValue)
            {
                query = query.Where(c => c.Type == TypeFilter.Value);
            }

            if (FromDate.HasValue)
            {
                query = query.Where(c => c.CheckTime.Date >= FromDate.Value.Date);
            }

            if (ToDate.HasValue)
            {
                query = query.Where(c => c.CheckTime.Date <= ToDate.Value.Date);
            }

            var records = await query
                .OrderByDescending(c => c.CheckTime)
                .Take(50) // Limit to 50 records for performance
                .ToListAsync();

            Records = records.Select(r => new CheckInOutRecordViewModel
            {
                CheckInOutRecordId = r.CheckInOutRecordId,
                Type = r.Type,
                Status = r.Status,
                CheckTime = r.CheckTime,
                Location = r.Location,
                Notes = r.Notes,
                Mileage = r.Mileage,
                VehicleCondition = r.VehicleCondition,
                Vehicle = r.Vehicle,
                User = r.User,
                ProcessedByUser = r.ProcessedByUser,
                ProcessedAt = r.ProcessedAt
            }).ToList();
        }
    }

    public class CheckInOutStats
    {
        public int CurrentlyInUse { get; set; }
        public int CheckInsToday { get; set; }
        public int CheckOutsToday { get; set; }
        public int TotalThisMonth { get; set; }
    }

    public class CheckInOutRecordViewModel
    {
        public int CheckInOutRecordId { get; set; }
        public int VehicleId { get; set; }
        public CheckInOutType Type { get; set; }
        public CheckInOutStatus Status { get; set; }
        public DateTime CheckTime { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public int? Mileage { get; set; }
        public string? VehicleCondition { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public User User { get; set; } = null!;
        public User? ProcessedByUser { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}

