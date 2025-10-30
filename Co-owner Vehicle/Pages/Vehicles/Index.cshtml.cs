using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.Vehicles
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly IGroupService _groupService;

        public IndexModel(
            IVehicleService vehicleService,
            IGroupService groupService)
        {
            _vehicleService = vehicleService;
            _groupService = groupService;
        }

        public List<VehicleViewModel> Vehicles { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public VehicleType? VehicleTypeFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public VehicleStatus? StatusFilter { get; set; }

        public class VehicleViewModel
        {
            public Vehicle Vehicle { get; set; } = null!;
            public decimal OwnershipPercentage { get; set; }
            public int MemberCount { get; set; }
            public List<CoOwnerGroup> Groups { get; set; } = new();
        }

        public async Task OnGetAsync()
        {
            var userId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            List<Vehicle> vehicles;

            if (isAdmin || isStaff)
            {
                // Admin/Staff: See all vehicles
                vehicles = await _vehicleService.GetAllVehiclesAsync();
            }
            else
            {
                // Co-owner: Only see vehicles they own
                var userGroups = await _groupService.GetGroupsByUserIdAsync(userId);
                vehicles = userGroups
                    .Where(g => g.Vehicle != null)
                    .Select(g => g.Vehicle!)
                    .Distinct()
                    .ToList();
            }

            // Apply filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                vehicles = vehicles.Where(v =>
                    (v.Brand?.ToLower().Contains(searchLower) ?? false) ||
                    (v.Model?.ToLower().Contains(searchLower) ?? false) ||
                    (v.LicensePlate?.ToLower().Contains(searchLower) ?? false)
                ).ToList();
            }

            if (VehicleTypeFilter.HasValue)
            {
                vehicles = vehicles.Where(v => v.VehicleType == VehicleTypeFilter.Value).ToList();
            }

            if (StatusFilter.HasValue)
            {
                vehicles = vehicles.Where(v => v.Status == StatusFilter.Value).ToList();
            }

            // Build view models with ownership info
            foreach (var vehicle in vehicles)
            {
                var groups = await _groupService.GetGroupsByVehicleIdAsync(vehicle.VehicleId);
                var userGroups = groups.Where(g => g.GroupMembers.Any(m => m.UserId == userId && m.Status == MemberStatus.Active));

                decimal ownershipPercentage = 0;
                if (userGroups.Any())
                {
                    foreach (var group in userGroups)
                    {
                        var shares = await _groupService.GetOwnershipSharesAsync(group.CoOwnerGroupId);
                        var userShare = shares.FirstOrDefault(s => s.UserId == userId);
                        if (userShare != null)
                        {
                            ownershipPercentage += userShare.Percentage;
                        }
                    }
                }

                var memberCount = groups
                    .SelectMany(g => g.GroupMembers)
                    .Where(m => m.Status == MemberStatus.Active)
                    .Select(m => m.UserId)
                    .Distinct()
                    .Count();

                Vehicles.Add(new VehicleViewModel
                {
                    Vehicle = vehicle,
                    OwnershipPercentage = ownershipPercentage,
                    MemberCount = memberCount,
                    Groups = groups
                });
            }

            // Sort by ownership percentage (descending) for co-owners
            if (!isAdmin && !isStaff)
            {
                Vehicles = Vehicles.OrderByDescending(v => v.OwnershipPercentage).ToList();
            }
        }
    }
}

