using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.CommonFunds
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class TransactionsModel : PageModel
    {
        private readonly ICommonFundService _fundService;

        public TransactionsModel(ICommonFundService fundService)
        {
            _fundService = fundService;
        }

        public CommonFund? Fund { get; set; }
        public List<FundTransaction> Transactions { get; set; } = new();
        public decimal IncomeTotal { get; set; }
        public decimal ExpenseTotal { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? From { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? To { get; set; }

        public async Task<IActionResult> OnGetAsync(int? fundId)
        {
            if (fundId == null)
            {
                return NotFound();
            }

            Fund = await _fundService.GetFundByIdAsync(fundId.Value);
            if (Fund == null)
            {
                return NotFound();
            }

            var all = await _fundService.GetTransactionsAsync(Fund.CommonFundId);
            var query = all.AsQueryable();
            if (From.HasValue)
                query = query.Where(t => t.TransactionDate >= From.Value);
            if (To.HasValue)
                query = query.Where(t => t.TransactionDate <= To.Value);

            Transactions = query.OrderByDescending(t => t.TransactionDate).ToList();
            IncomeTotal = Transactions
                .Where(t => t.TransactionType == TransactionType.Deposit 
                         || t.TransactionType == TransactionType.Interest 
                         || t.TransactionType == TransactionType.Refund)
                .Sum(t => t.Amount);
            ExpenseTotal = Transactions
                .Where(t => t.TransactionType == TransactionType.Withdrawal 
                         || t.TransactionType == TransactionType.Expense)
                .Sum(t => t.Amount);

            return Page();
        }
    }
}

