using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.Services
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class DetailsModel : PageModel
    {
        private readonly IServiceRecordService _serviceRecordService;

        public DetailsModel(IServiceRecordService serviceRecordService)
        {
            _serviceRecordService = serviceRecordService;
        }

        public ServiceRecord? Record { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Record = await _serviceRecordService.GetByIdAsync(id.Value);
            if (Record == null)
                return NotFound();

            return Page();
        }
    }
}

