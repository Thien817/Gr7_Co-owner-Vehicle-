using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Reports
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class UsageModel : PageModel
    {
        private readonly IReportsService _reportsService;

        public UsageModel(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [BindProperty(SupportsGet = true)]
        public int GroupId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? From { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? To { get; set; }

        public Dictionary<int, double> UsageHoursByUser { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (GroupId > 0)
            {
                UsageHoursByUser = await _reportsService.GetUsageHoursByUserAsync(GroupId, From, To);
            }
        }
    }
}

