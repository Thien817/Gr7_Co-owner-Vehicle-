using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Co_owner_Vehicle.Pages.Bookings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IBookingService _bookingService;
        public IndexModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public List<BookingSchedule> Bookings { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public BookingStatus? StatusFilter { get; set; }

        public async Task OnGetAsync()
        {
            var allBookings = await _bookingService.GetAllBookingsAsync();
            if (StatusFilter.HasValue)
                allBookings = allBookings.Where(b => b.Status == StatusFilter.Value).ToList();
            Bookings = allBookings.OrderByDescending(b => b.StartTime).ToList();
        }
    }
}

