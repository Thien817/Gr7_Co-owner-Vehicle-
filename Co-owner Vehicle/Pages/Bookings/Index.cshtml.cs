using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Co_owner_Vehicle.Pages.Bookings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // TODO: Load all bookings from BookingService
            // - Get all bookings (admin/staff view)
            // - Apply filters
            // - Group by status
            // - Pagination
        }
    }
}

