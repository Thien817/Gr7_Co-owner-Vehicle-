using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Claims;

namespace Co_owner_Vehicle.Pages.Bookings
{
    [Authorize]
    public class MyBookingsModel : PageModel
    {
        private readonly IBookingService _bookingService;
        public MyBookingsModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public List<BookingSchedule> UpcomingBookings { get; set; } = new();
        public List<BookingSchedule> PendingBookings { get; set; } = new();
        public List<BookingSchedule> HistoryBookings { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get user id (sample: from claims)
            var userIdStr = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return;
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            UpcomingBookings = bookings.Where(b => b.Status == BookingStatus.Confirmed && b.StartTime > DateTime.UtcNow).ToList();
            PendingBookings = bookings.Where(b => b.Status == BookingStatus.Pending).ToList();
            HistoryBookings = bookings.Where(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Cancelled).ToList();
        }
    }
}

