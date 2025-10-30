using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Co_owner_Vehicle.Pages.Bookings
{
    public class MyBookingsModel : PageModel
    {
        public void OnGet()
        {
            // TODO: Load user's bookings from BookingService
            // - Get current user's bookings only
            // - Group by status (upcoming, pending, history)
            // - Calculate statistics
        }
    }
}

