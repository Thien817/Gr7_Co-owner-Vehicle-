using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Services
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class IndexModel : PageModel
    {
        private readonly IServiceRecordService _serviceRecordService;
        private readonly IGroupService _groupService;

        public IndexModel(IServiceRecordService serviceRecordService, IGroupService groupService)
        {
            _serviceRecordService = serviceRecordService;
            _groupService = groupService;
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
            var isAdminOrStaff = isAdmin || isStaff;

            List<ServiceRecord> records;
            if (StatusFilter.HasValue)
            {
                // Get by status from service
                records = await _serviceRecordService.GetByStatusAsync(StatusFilter.Value);
            }
            else
            {
                // Aggregate multiple statuses for a broader list
                var scheduled = await _serviceRecordService.GetByStatusAsync(ServiceStatus.Scheduled);
                var inProgress = await _serviceRecordService.GetByStatusAsync(ServiceStatus.InProgress);
                var completed = await _serviceRecordService.GetByStatusAsync(ServiceStatus.Completed);
                records = scheduled
                    .Concat(inProgress)
                    .Concat(completed)
                    .ToList();
            }

            if (!isAdminOrStaff)
            {
                // Filter records to vehicles in user groups
                var userGroups = await _groupService.GetGroupsByUserIdAsync(currentUserId);
                var userVehicleIds = userGroups.Select(g => g.VehicleId).Distinct().ToHashSet();
                records = records.Where(r => userVehicleIds.Contains(r.VehicleId)).ToList();
            }

            if (VehicleFilter.HasValue)
            {
                records = records.Where(sr => sr.VehicleId == VehicleFilter.Value).ToList();
            }

            if (TypeFilter.HasValue)
            {
                records = records.Where(sr => sr.VehicleService?.ServiceType == TypeFilter.Value).ToList();
            }

            records = records
                .OrderByDescending(sr => sr.ScheduledDate)
                .Take(100)
                .ToList();

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

            // Statistics (in-memory from filtered domain list; for exact global stats add service method later)
            var allForStats = records; // using current filtered scope
            Statistics.TotalServices = allForStats.Count;
            Statistics.ScheduledServices = allForStats.Count(sr => sr.Status == ServiceStatus.Scheduled);
            Statistics.InProgressServices = allForStats.Count(sr => sr.Status == ServiceStatus.InProgress);
            Statistics.CompletedServices = allForStats.Count(sr => sr.Status == ServiceStatus.Completed);
            Statistics.TotalSpent = allForStats.Where(sr => sr.ActualCost.HasValue).Sum(sr => sr.ActualCost ?? 0);
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

