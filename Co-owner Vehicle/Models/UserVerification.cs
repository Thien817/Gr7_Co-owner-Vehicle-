using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum VerificationType
    {
        CitizenId = 1,      // CMND/CCCD
        DriverLicense = 2,  // Giấy phép lái xe
        Email = 3,          // Xác thực email
        Phone = 4           // Xác thực số điện thoại
    }

    public enum VerificationStatus
    {
        Pending = 1,        // Chờ xử lý
        Approved = 2,       // Đã duyệt
        Rejected = 3,       // Từ chối
        Expired = 4         // Hết hạn
    }

    public class UserVerification
    {
        [Key]
        public int UserVerificationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public VerificationType VerificationType { get; set; }

        [Required]
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

        [StringLength(500)]
        public string? DocumentPath { get; set; } // Đường dẫn file tài liệu

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(200)]
        public string? RejectionReason { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedBy { get; set; } // UserId của người review

        public DateTime? ExpiresAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ReviewedBy")]
        public virtual User? ReviewedByUser { get; set; }
    }
}
