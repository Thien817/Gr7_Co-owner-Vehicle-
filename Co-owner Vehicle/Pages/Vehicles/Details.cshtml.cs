using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Co_owner_Vehicle.Pages.Vehicles
{
    public class DetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        public void OnGet()
        {
            // TODO: Load vehicle details from VehicleService
            // - Get vehicle by ID
            // - Get ownership shares
            // - Get usage history
            // - Get expenses
            // - Get bookings
        }
    }
}

