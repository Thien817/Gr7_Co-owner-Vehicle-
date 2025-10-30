using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Expenses
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IExpenseService _expenseService;
        public DetailsModel(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public Expense? Expense { get; set; }
        public Dictionary<int, decimal> Split { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            Expense = await _expenseService.GetExpenseByIdAsync(id.Value);
            if (Expense == null) return NotFound();
            Split = await _expenseService.CalculateExpenseSplitAsync(id.Value);
            return Page();
        }
    }
}

