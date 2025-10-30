using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Groups
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IGroupService _groupService;
        private readonly IVehicleService _vehicleService;
        public CreateModel(IGroupService groupService, IVehicleService vehicleService)
        {
            _groupService = groupService;
            _vehicleService = vehicleService;
        }

        public List<Vehicle> Vehicles { get; set; } = new();

        public async Task OnGetAsync()
        {
            Vehicles = await _vehicleService.GetAllVehiclesAsync();
        }
    }
}

