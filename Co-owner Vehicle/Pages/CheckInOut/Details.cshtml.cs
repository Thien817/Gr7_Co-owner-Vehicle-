using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Co_owner_Vehicle.Pages.CheckInOut
{
    [Authorize(Roles = "Staff,Admin")]
    public class DetailsModel : PageModel
    {
        private readonly CoOwnerVehicleDbContext _context;

        public DetailsModel(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public CheckInOutRecordViewModel? Record { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var record = await _context.CheckInOutRecords
                .Include(c => c.Vehicle)
                .Include(c => c.User)
                .Include(c => c.ProcessedByUser)
                .FirstOrDefaultAsync(c => c.CheckInOutRecordId == id);

            if (record == null)
                return NotFound();

            Record = new CheckInOutRecordViewModel
            {
                CheckInOutRecordId = record.CheckInOutRecordId,
                VehicleId = record.VehicleId,
                Type = record.Type,
                Status = record.Status,
                CheckTime = record.CheckTime,
                Location = record.Location,
                Notes = record.Notes,
                Mileage = record.Mileage,
                VehicleCondition = record.VehicleCondition,
                Vehicle = record.Vehicle,
                User = record.User,
                ProcessedByUser = record.ProcessedByUser,
                ProcessedAt = record.ProcessedAt
            };

            return Page();
        }
    }
}

