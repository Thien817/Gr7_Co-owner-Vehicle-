using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Bookings
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly ICheckInOutService _checkInOutService;
        public DetailsModel(IBookingService bookingService, ICheckInOutService checkInOutService)
        {
            _bookingService = bookingService;
            _checkInOutService = checkInOutService;
        }

        public BookingSchedule? Booking { get; set; }
        public List<CheckInOutRecord> CheckInOutRecords { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();
            Booking = await _bookingService.GetBookingByIdAsync(id.Value);
            if (Booking == null) return NotFound();
            CheckInOutRecords = await _checkInOutService.GetTodayRecordsAsync(Booking.VehicleId);
            return Page();
        }
    }
}

