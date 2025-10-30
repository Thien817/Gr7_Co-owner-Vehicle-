using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Reports
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class FinancialModel : PageModel
    {
        private readonly IReportsService _reportsService;

        public FinancialModel(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [BindProperty(SupportsGet = true)]
        public int GroupId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? From { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? To { get; set; }

        public decimal TotalExpenses { get; set; }
        public decimal TotalPayments { get; set; }
        public Dictionary<string, decimal> ExpensesByCategory { get; set; } = new();
        public Dictionary<string, decimal> ExpensesByVehicle { get; set; } = new();
        public List<Co_owner_Vehicle.Models.Expense> TopExpenses { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (GroupId <= 0)
                return Page();

            TotalExpenses = await _reportsService.GetTotalExpensesAsync(GroupId, From, To);
            TotalPayments = await _reportsService.GetTotalPaymentsAsync(GroupId, From, To);
            ExpensesByCategory = await _reportsService.GetExpensesByCategoryAsync(GroupId, From, To);
            ExpensesByVehicle = await _reportsService.GetExpensesByVehicleAsync(GroupId, From, To);
            TopExpenses = await _reportsService.GetTopExpensesAsync(GroupId, From, To, 5);
            return Page();
        }
    }
}

