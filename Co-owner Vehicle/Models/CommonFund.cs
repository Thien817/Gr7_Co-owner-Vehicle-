using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum FundType
    {
        Maintenance = 1,   // Quỹ bảo dưỡng
        Emergency = 2,     // Quỹ dự phòng
        Insurance = 3,     // Quỹ bảo hiểm
        Upgrade = 4,       // Quỹ nâng cấp
        Other = 5          // Quỹ khác
    }

    public class CommonFund
    {
        [Key]
        public int CommonFundId { get; set; }

        [Required]
        public int CoOwnerGroupId { get; set; }

        [Required]
        [StringLength(200)]
        public string FundName { get; set; } = string.Empty;

        [Required]
        public FundType FundType { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TargetAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("CoOwnerGroupId")]
        public virtual CoOwnerGroup CoOwnerGroup { get; set; } = null!;

        public virtual ICollection<FundTransaction> FundTransactions { get; set; } = new List<FundTransaction>();
    }

    public enum TransactionType
    {
        Deposit = 1,        // Nạp tiền
        Withdrawal = 2,     // Rút tiền
        Expense = 3,        // Chi phí
        Interest = 4,      // Lãi suất
        Refund = 5         // Hoàn tiền
    }

    public class FundTransaction
    {
        [Key]
        public int FundTransactionId { get; set; }

        [Required]
        public int CommonFundId { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int? RelatedExpenseId { get; set; } // Liên kết với Expense

        public int? ProcessedBy { get; set; } // UserId của người xử lý

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey("CommonFundId")]
        public virtual CommonFund CommonFund { get; set; } = null!;

        [ForeignKey("RelatedExpenseId")]
        public virtual Expense? RelatedExpense { get; set; }

        [ForeignKey("ProcessedBy")]
        public virtual User? ProcessedByUser { get; set; }
    }
}
