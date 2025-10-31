using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum ExpenseStatus
    {
        Pending = 1,        // Chờ duyệt
        Approved = 2,       // Đã duyệt
        Rejected = 3,       // Từ chối
        Paid = 4           // Đã thanh toán
    }

    public enum SplitMethod
    {
        ByOwnership = 1,    // Theo tỷ lệ sở hữu
        ByUsage = 2,        // Theo mức độ sử dụng
        Equal = 3,          // Chia đều
        Custom = 4          // Tùy chỉnh
    }

    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int ExpenseCategoryId { get; set; }

        [Required]
        public int CoOwnerGroupId { get; set; }

        [Required]
        [StringLength(200)]
        public string ExpenseTitle { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

        [Required]
        public SplitMethod SplitMethod { get; set; } = SplitMethod.ByOwnership;

        public DateTime ExpenseDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedBy { get; set; } // UserId của người duyệt

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        [StringLength(500)]
        public string? ReceiptPath { get; set; } // Đường dẫn hóa đơn

        [StringLength(200)]
        public string? VendorName { get; set; } // Tên nhà cung cấp

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey("ExpenseCategoryId")]
        public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;

        [ForeignKey("CoOwnerGroupId")]
        public virtual CoOwnerGroup CoOwnerGroup { get; set; } = null!;

        [ForeignKey("ApprovedBy")]
        public virtual User? ApprovedByUser { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
