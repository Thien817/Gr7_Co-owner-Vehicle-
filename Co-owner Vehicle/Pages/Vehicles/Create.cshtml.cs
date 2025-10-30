using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Vehicles
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly IGroupService _groupService;
        public CreateModel(IVehicleService vehicleService, IGroupService groupService)
        {
            _vehicleService = vehicleService;
            _groupService = groupService;
        }

        public Array VehicleTypes => Enum.GetValues(typeof(VehicleType));
        public Array StatusOptions => Enum.GetValues(typeof(VehicleStatus));

        [BindProperty]
        public Vehicle Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Default sensible values
            if (Input.Status == 0) Input.Status = VehicleStatus.Active;

            var vehicle = await _vehicleService.CreateVehicleAsync(Input);

            // Tự động tạo nhóm đồng sở hữu cho người tạo để họ nhìn thấy xe
            var currentUserId = this.GetCurrentUserId();
            var group = new CoOwnerGroup
            {
                VehicleId = vehicle.VehicleId,
                GroupName = $"Nhóm {vehicle.Brand} {vehicle.Model}",
                Status = GroupStatus.Active,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            };

            group = await _groupService.CreateGroupAsync(group);
            await _groupService.AddMemberToGroupAsync(group.CoOwnerGroupId, currentUserId, MemberRole.Owner);
            await _groupService.UpdateOwnershipShareAsync(group.CoOwnerGroupId, currentUserId, 100m, vehicle.PurchasePrice);

            TempData["SuccessMessage"] = "Thêm xe thành công và đã tạo nhóm đồng sở hữu cho bạn.";
            return RedirectToPage("/Vehicles/Index");
        }
    }
}

