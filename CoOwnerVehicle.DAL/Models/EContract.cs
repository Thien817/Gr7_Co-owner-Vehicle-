using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum ContractStatus
    {
        Draft = 1,          // Nháp
        Pending = 2,        // Chờ ký
        Active = 3,         // Có hiệu lực
        Expired = 4,        // Hết hạn
        Terminated = 5      // Chấm dứt
    }

    public class EContract
    {
        [Key]
        public int EContractId { get; set; }

        [Required]
        public int CoOwnerGroupId { get; set; }

        [Required]
        [StringLength(200)]
        public string ContractTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string ContractContent { get; set; } = string.Empty;

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? SignedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [StringLength(500)]
        public string? ContractFilePath { get; set; } // Đường dẫn file hợp đồng PDF

        [StringLength(1000)]
        public string? DigitalSignature { get; set; } // Chữ ký số

        public int? CreatedBy { get; set; } // UserId của người tạo

        public int? SignedBy { get; set; } // UserId của người ký

        [StringLength(500)]
        public string? TerminationReason { get; set; }

        public DateTime? TerminatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CoOwnerGroupId")]
        public virtual CoOwnerGroup CoOwnerGroup { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        [ForeignKey("SignedBy")]
        public virtual User? SignedByUser { get; set; }
    }
}
