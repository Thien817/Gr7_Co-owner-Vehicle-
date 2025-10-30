using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Payments
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        public IndexModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public List<Payment> Payments { get; set; } = new();
        public decimal Outstanding { get; set; }
        public decimal PaidTotal { get; set; }
        public int ProcessingCount { get; set; }
        public int TotalTransactions { get; set; }
        public int OutstandingCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public PaymentStatus? Status { get; set; }

        public async Task OnGetAsync()
        {
            var userIdStr = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return;
            Payments = await _paymentService.GetPaymentsByUserIdAsync(userId);
            if (Status.HasValue)
                Payments = Payments.Where(p => p.Status == Status.Value).ToList();
            Outstanding = await _paymentService.GetOutstandingAmountAsync(userId);

            PaidTotal = Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            ProcessingCount = Payments.Count(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing);
            TotalTransactions = Payments.Count;
            OutstandingCount = Payments.Count(p => p.Status == PaymentStatus.Pending);

            // Provide simple notifications to the layout
            var notifications = new List<object>();
            if (Payments.Any())
            {
                var last = Payments.OrderByDescending(p => p.PaymentDate).First();
                notifications.Add(new { Icon="bi-credit-card", Color="bg-success", Title="Thanh toán gần nhất", Sub=$"{last.Amount:#,##0}₫ - {last.Description}" });
            }
            if (Outstanding > 0)
            {
                notifications.Add(new { Icon="bi-exclamation-circle", Color="bg-warning", Title="Công nợ chưa thanh toán", Sub=$"Tổng {Outstanding:#,##0}₫" });
            }
            ViewData["Notifications"] = notifications;
        }
    }
}

