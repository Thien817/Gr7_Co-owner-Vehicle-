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

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (Vehicle == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy xe để cập nhật";
                return RedirectToPage("/Vehicles/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _vehicleService.UpdateVehicleAsync(Vehicle);
                TempData["SuccessMessage"] = $"Đã cập nhật xe {Vehicle.Brand} {Vehicle.Model} thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật xe. Vui lòng thử lại.";
            }

            return RedirectToPage("/Vehicles/Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (vehicle == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy xe để xóa";
                    return RedirectToPage("/Vehicles/Index");
                }

                await _vehicleService.DeleteVehicleAsync(id);
                TempData["SuccessMessage"] = $"Đã xóa xe {vehicle.Brand} {vehicle.Model} thành công!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa xe. Vui lòng thử lại.";
            }

            return RedirectToPage("/Vehicles/Index");
        }
    }
}

