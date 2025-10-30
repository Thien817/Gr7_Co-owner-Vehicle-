using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.Contracts
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class DetailsModel : PageModel
    {
        private readonly IContractService _contractService;

        public DetailsModel(IContractService contractService)
        {
            _contractService = contractService;
        }

        public EContract? Contract { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Contract = await _contractService.GetByIdAsync(id.Value);
            if (Contract == null)
                return NotFound();

            return Page();
        }
    }
}

