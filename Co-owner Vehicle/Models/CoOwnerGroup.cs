using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum GroupStatus
    {
        Active = 1,         // Hoạt động
        Suspended = 2,      // Tạm dừng
        Dissolved = 3,      // Giải thể
        Pending = 4         // Chờ duyệt
    }

    public class CoOwnerGroup
    {
        [Key]
        public int CoOwnerGroupId { get; set; }

        [Required]
        [StringLength(200)]
        public string GroupName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int CreatedBy { get; set; } // UserId của người tạo nhóm

        [Required]
        public GroupStatus Status { get; set; } = GroupStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ActivatedAt { get; set; }

        public DateTime? DissolvedAt { get; set; }

        [StringLength(500)]
        public string? DissolutionReason { get; set; }

        // Navigation Properties
        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;

        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public virtual ICollection<OwnershipShare> OwnershipShares { get; set; } = new List<OwnershipShare>();
        public virtual ICollection<EContract> EContracts { get; set; } = new List<EContract>();
        public virtual ICollection<VotingSession> VotingSessions { get; set; } = new List<VotingSession>();
        public virtual ICollection<CommonFund> CommonFunds { get; set; } = new List<CommonFund>();
        public virtual ICollection<FinancialReport> FinancialReports { get; set; } = new List<FinancialReport>();
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
