using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Expenses
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IExpenseService _expenseService;
        public CreateModel(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public List<ExpenseCategory> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            Categories = await _expenseService.GetAllExpenseCategoriesAsync();
        }
    }
}

