using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public class OwnershipShare
    {
        [Key]
        public int OwnershipShareId { get; set; }

        [Required]
        public int CoOwnerGroupId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Percentage { get; set; } // Ví dụ: 40.00 cho 40%

        [Column(TypeName = "decimal(18,2)")]
        public decimal? InvestmentAmount { get; set; } // Số tiền đầu tư

        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;

        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey("CoOwnerGroupId")]
        public virtual CoOwnerGroup CoOwnerGroup { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
