using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Payments
{
    [Authorize]
    public class PayModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IExpenseService _expenseService;

        public PayModel(IPaymentService paymentService, IExpenseService expenseService)
        {
            _paymentService = paymentService;
            _expenseService = expenseService;
        }

        public Expense? Expense { get; set; }
        public decimal OutstandingForUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int? expenseId)
        {
            if (expenseId == null) return NotFound();
            Expense = await _expenseService.GetExpenseByIdAsync(expenseId.Value);
            if (Expense == null) return NotFound();

            var userIdStr = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                OutstandingForUser = await _paymentService.GetOutstandingAmountAsync(userId);
            }
            return Page();
        }
    }
}

