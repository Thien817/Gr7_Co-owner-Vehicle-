using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.CommonFunds
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class DetailsModel : PageModel
    {
        private readonly ICommonFundService _fundService;

        public DetailsModel(ICommonFundService fundService)
        {
            _fundService = fundService;
        }

        public CommonFund? Fund { get; set; }
        public List<FundTransaction> RecentTransactions { get; set; } = new();
        public decimal Balance { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Fund = await _fundService.GetFundByIdAsync(id.Value);
            if (Fund == null)
            {
                return NotFound();
            }

            Balance = await _fundService.GetFundBalanceAsync(Fund.CommonFundId);
            RecentTransactions = (await _fundService.GetTransactionsAsync(Fund.CommonFundId))
                .Take(10)
                .ToList();

            return Page();
        }
    }
}

