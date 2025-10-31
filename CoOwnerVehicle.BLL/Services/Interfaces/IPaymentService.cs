using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<List<Payment>> GetPaymentsByUserIdAsync(int userId);
        Task<List<Payment>> GetPaymentsByExpenseIdAsync(int expenseId);
        Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
        Task<List<Payment>> GetPaymentsByMethodAsync(PaymentMethod method);
        Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> UpdatePaymentAsync(Payment payment);
        Task<bool> DeletePaymentAsync(int paymentId);
        Task<bool> ProcessPaymentAsync(int paymentId);
        Task<bool> CompletePaymentAsync(int paymentId, string transactionId);
        Task<bool> FailPaymentAsync(int paymentId, string failureReason);
        Task<bool> RefundPaymentAsync(int paymentId, string reason);
        
        // Payment Processing
        Task<Payment> CreatePaymentForExpenseAsync(int expenseId, int userId, decimal amount, PaymentMethod method);
        Task<List<Payment>> CreatePaymentsForExpenseSplitAsync(int expenseId);
        Task<bool> ValidatePaymentAmountAsync(int paymentId);
        Task<bool> CanUserMakePaymentAsync(int userId, decimal amount);
        
        // Outstanding & Settlements
        Task<List<Payment>> GetOutstandingPaymentsAsync(int userId);
        Task<decimal> GetOutstandingAmountAsync(int userId);
        Task<decimal> GetOutstandingAmountByGroupAsync(int groupId);
        Task<List<Payment>> GetPendingPaymentsAsync(int groupId);
        Task<bool> MarkPaymentAsSettledAsync(int paymentId);
        
        // Reports
        Task<decimal> GetTotalPaymentsByUserAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalPaymentsByGroupAsync(int groupId, DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<PaymentMethod, decimal>> GetPaymentsByMethodAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<Payment>> GetRecentPaymentsAsync(int userId, int count = 10);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
