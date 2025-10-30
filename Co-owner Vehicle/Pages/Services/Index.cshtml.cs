using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Co_owner_Vehicle.Pages.Services
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class IndexModel : PageModel
    {
        private readonly CoOwnerVehicleDbContext _context;

        public IndexModel(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public List<ServiceRecordViewModel> ServiceRecords { get; set; } = new();
        public ServiceStatistics Statistics { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public ServiceStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VehicleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public ServiceType? TypeFilter { get; set; }

        public async Task OnGetAsync()
        {
            var currentUserId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            IQueryable<ServiceRecord> recordsQuery;

            if (isAdmin || isStaff)
            {
                // Admin and Staff can see all service records
                recordsQuery = _context.ServiceRecords
                    .Include(sr => sr.Vehicle)
                    .Include(sr => sr.VehicleService)
                    .Include(sr => sr.PerformedByUser);
            }
            else
            {
                // Co-owners can only see services of vehicles in their groups
                var userGroups = await _context.GroupMembers
                    .Where(gm => gm.UserId == currentUserId && gm.Status == MemberStatus.Active)
                    .Select(gm => gm.CoOwnerGroupId)
                    .ToListAsync();

                var userVehicles = await _context.CoOwnerGroups
                    .Where(g => userGroups.Contains(g.CoOwnerGroupId))
                    .Select(g => g.VehicleId)
                    .Distinct()
                    .ToListAsync();

                recordsQuery = _context.ServiceRecords
                    .Include(sr => sr.Vehicle)
                    .Include(sr => sr.VehicleService)
                    .Include(sr => sr.PerformedByUser)
                    .Where(sr => userVehicles.Contains(sr.VehicleId));
            }

            // Apply filters
            if (StatusFilter.HasValue)
            {
                recordsQuery = recordsQuery.Where(sr => sr.Status == StatusFilter.Value);
            }

            if (VehicleFilter.HasValue)
            {
                recordsQuery = recordsQuery.Where(sr => sr.VehicleId == VehicleFilter.Value);
            }

            if (TypeFilter.HasValue)
            {
                recordsQuery = recordsQuery.Where(sr => sr.VehicleService.ServiceType == TypeFilter.Value);
            }

            var records = await recordsQuery
                .OrderByDescending(sr => sr.ScheduledDate)
                .Take(100) // Limit to 100 records for performance
                .ToListAsync();

            ServiceRecords = records.Select(sr => new ServiceRecordViewModel
            {
                ServiceRecordId = sr.ServiceRecordId,
                VehicleId = sr.VehicleId,
                VehicleName = sr.Vehicle != null 
                    ? $"{sr.Vehicle.Brand} {sr.Vehicle.Model} ({sr.Vehicle.LicensePlate})" 
                    : "Unknown",
                ServiceName = sr.VehicleService?.ServiceName ?? "Unknown Service",
                ServiceType = sr.VehicleService?.ServiceType ?? ServiceType.Other,
                Status = sr.Status,
                ScheduledDate = sr.ScheduledDate,
                StartedAt = sr.StartedAt,
                CompletedAt = sr.CompletedAt,
                ActualCost = sr.ActualCost,
                EstimatedCost = sr.VehicleService?.EstimatedCost,
                ServiceNotes = sr.ServiceNotes,
                VendorName = sr.VendorName,
                IssuesFound = sr.IssuesFound,
                Recommendations = sr.Recommendations,
                PerformedBy = sr.PerformedByUser?.FullName,
                CreatedAt = sr.CreatedAt
            }).ToList();

            // Calculate statistics
            Statistics.TotalServices = await _context.ServiceRecords.CountAsync();
            Statistics.ScheduledServices = await _context.ServiceRecords.CountAsync(sr => sr.Status == ServiceStatus.Scheduled);
            Statistics.InProgressServices = await _context.ServiceRecords.CountAsync(sr => sr.Status == ServiceStatus.InProgress);
            Statistics.CompletedServices = await _context.ServiceRecords.CountAsync(sr => sr.Status == ServiceStatus.Completed);
            
            // Calculate total spent
            Statistics.TotalSpent = await _context.ServiceRecords
                .Where(sr => sr.ActualCost.HasValue)
                .SumAsync(sr => sr.ActualCost ?? 0);
        }
    }

    public class ServiceRecordViewModel
    {
        public int ServiceRecordId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public ServiceType ServiceType { get; set; }
        public ServiceStatus Status { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal? ActualCost { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string? ServiceNotes { get; set; }
        public string? VendorName { get; set; }
        public string? IssuesFound { get; set; }
        public string? Recommendations { get; set; }
        public string? PerformedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ServiceStatistics
    {
        public int TotalServices { get; set; }
        public int ScheduledServices { get; set; }
        public int InProgressServices { get; set; }
        public int CompletedServices { get; set; }
        public decimal TotalSpent { get; set; }
    }
}

