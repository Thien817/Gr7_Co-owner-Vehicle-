using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum PaymentMethod
    {
        BankTransfer = 1,   // Chuyển khoản ngân hàng
        EWallet = 2,        // Ví điện tử
        Cash = 3,          // Tiền mặt
        CreditCard = 4,    // Thẻ tín dụng
        Other = 5          // Khác
    }

    public enum PaymentStatus
    {
        Pending = 1,        // Chờ thanh toán
        Processing = 2,     // Đang xử lý
        Completed = 3,      // Hoàn thành
        Failed = 4,         // Thất bại
        Refunded = 5        // Đã hoàn tiền
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? ExpenseId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [StringLength(200)]
        public string? TransactionId { get; set; } // Mã giao dịch

        [StringLength(500)]
        public string? PaymentReference { get; set; } // Tham chiếu thanh toán

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        [StringLength(500)]
        public string? FailureReason { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ExpenseId")]
        public virtual Expense? Expense { get; set; }
    }
}
