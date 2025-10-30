using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Vehicles
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        public EditModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public Vehicle? Vehicle { get; set; }
        public Array VehicleTypes => Enum.GetValues(typeof(VehicleType));
        public Array StatusOptions => Enum.GetValues(typeof(VehicleStatus));

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            Vehicle = await _vehicleService.GetVehicleByIdAsync(id.Value);
            if (Vehicle == null) return NotFound();
            return Page();
        }
    }
}

