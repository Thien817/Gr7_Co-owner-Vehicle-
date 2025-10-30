using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly CoOwnerVehicleDbContext _context;

        public PaymentService(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Expense)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Expense)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Expense)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByExpenseIdAsync(int expenseId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Expense)
                .Where(p => p.ExpenseId == expenseId)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Expense)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByMethodAsync(PaymentMethod method)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Expense)
                .Where(p => p.PaymentMethod == method)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Expense)
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            payment.Status = PaymentStatus.Pending;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProcessPaymentAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            payment.Status = PaymentStatus.Processing;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompletePaymentAsync(int paymentId, string transactionId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            payment.Status = PaymentStatus.Completed;
            payment.CompletedAt = DateTime.UtcNow;
            payment.TransactionId = transactionId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FailPaymentAsync(int paymentId, string failureReason)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = failureReason;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RefundPaymentAsync(int paymentId, string reason)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            payment.Status = PaymentStatus.Refunded;
            payment.Notes = reason;
            await _context.SaveChangesAsync();
            return true;
        }

        // Payment Processing
        public async Task<Payment> CreatePaymentForExpenseAsync(int expenseId, int userId, decimal amount, PaymentMethod method)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null) throw new ArgumentException("Expense not found");

            var payment = new Payment
            {
                UserId = userId,
                ExpenseId = expenseId,
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = DateTime.UtcNow,
                Description = $"Payment for expense: {expense.ExpenseTitle}",
                Status = PaymentStatus.Pending
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<List<Payment>> CreatePaymentsForExpenseSplitAsync(int expenseId)
        {
            var expense = await _context.Expenses
                .Include(e => e.CoOwnerGroup)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId);

            if (expense == null) return new List<Payment>();

            var expenseService = new ExpenseService(_context);
            var split = await expenseService.CalculateExpenseSplitAsync(expenseId);

            var payments = new List<Payment>();
            foreach (var kvp in split)
            {
                var payment = new Payment
                {
                    UserId = kvp.Key,
                    ExpenseId = expenseId,
                    Amount = kvp.Value,
                    PaymentMethod = PaymentMethod.BankTransfer, // Default method
                    PaymentDate = DateTime.UtcNow,
                    Description = $"Split payment for expense: {expense.ExpenseTitle}",
                    Status = PaymentStatus.Pending
                };
                payments.Add(payment);
            }

            _context.Payments.AddRange(payments);
            await _context.SaveChangesAsync();
            return payments;
        }

        public async Task<bool> ValidatePaymentAmountAsync(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Expense)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null) return false;

            // Check if payment amount matches the expense split
            var expenseService = new ExpenseService(_context);
            var split = await expenseService.CalculateExpenseSplitAsync(payment.ExpenseId!.Value);
            
            return payment.ExpenseId.HasValue && split.ContainsKey(payment.UserId) && 
                   Math.Abs(split[payment.UserId] - payment.Amount) < 0.01m;
        }

        public Task<bool> CanUserMakePaymentAsync(int userId, decimal amount)
        {
            // This could be extended with credit checks, balance validation, etc.
            // For now, just return true
            return Task.FromResult(true);
        }

        // Outstanding & Settlements
        public async Task<List<Payment>> GetOutstandingPaymentsAsync(int userId)
        {
            return await _context.Payments
                .Include(p => p.Expense)
                .Where(p => p.UserId == userId && 
                           (p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing))
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetOutstandingAmountAsync(int userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId && 
                           (p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing))
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetOutstandingAmountByGroupAsync(int groupId)
        {
            return await _context.Payments
                .Include(p => p.Expense)
                .Where(p => p.Expense!.CoOwnerGroupId == groupId &&
                           (p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing))
                .SumAsync(p => p.Amount);
        }

        public async Task<List<Payment>> GetPendingPaymentsAsync(int groupId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Expense)
                .Where(p => p.Expense!.CoOwnerGroupId == groupId && p.Status == PaymentStatus.Pending)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<bool> MarkPaymentAsSettledAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            payment.Status = PaymentStatus.Completed;
            payment.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // Reports
        public async Task<decimal> GetTotalPaymentsByUserAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Where(p => p.UserId == userId && p.Status == PaymentStatus.Completed);

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query.SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetTotalPaymentsByGroupAsync(int groupId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Include(p => p.Expense)
                .Where(p => p.Expense!.CoOwnerGroupId == groupId && p.Status == PaymentStatus.Completed);

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query.SumAsync(p => p.Amount);
        }

        public async Task<Dictionary<PaymentMethod, decimal>> GetPaymentsByMethodAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Where(p => p.UserId == userId && p.Status == PaymentStatus.Completed);

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query
                .GroupBy(p => p.PaymentMethod)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(p => p.Amount));
        }

        public async Task<List<Payment>> GetRecentPaymentsAsync(int userId, int count = 10)
        {
            return await _context.Payments
                .Include(p => p.Expense)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
