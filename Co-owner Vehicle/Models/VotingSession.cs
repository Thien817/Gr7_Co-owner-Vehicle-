using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum VotingStatus
    {
        Draft = 1,          // Nháp
        Active = 2,         // Đang bỏ phiếu
        Completed = 3,      // Hoàn thành
        Cancelled = 4       // Hủy bỏ
    }

    public enum DecisionType
    {
        BatteryUpgrade = 1,  // Nâng cấp pin
        Insurance = 2,      // Bảo hiểm
        SellVehicle = 3,    // Bán xe
        Maintenance = 4,    // Bảo dưỡng
        AddMember = 5,      // Thêm thành viên
        RemoveMember = 6,   // Xóa thành viên
        ChangeOwnership = 7, // Thay đổi tỷ lệ sở hữu
        Other = 8           // Khác
    }

    public class VotingSession
    {
        [Key]
        public int VotingSessionId { get; set; }

        [Required]
        public int CoOwnerGroupId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DecisionType DecisionType { get; set; }

        [Required]
        public VotingStatus Status { get; set; } = VotingStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? CreatedBy { get; set; } // UserId của người tạo

        public int? RequiredVotes { get; set; } // Số phiếu tối thiểu để thông qua

        public int? YesVotes { get; set; } = 0;

        public int? NoVotes { get; set; } = 0;

        public int? AbstainVotes { get; set; } = 0;

        public bool IsPassed { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        [StringLength(1000)]
        public string? ResultNotes { get; set; }

        // Navigation Properties
        [ForeignKey("CoOwnerGroupId")]
        public virtual CoOwnerGroup CoOwnerGroup { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }

    public enum VoteChoice
    {
        Yes = 1,           // Đồng ý
        No = 2,            // Không đồng ý
        Abstain = 3        // Không bỏ phiếu
    }

    public class Vote
    {
        [Key]
        public int VoteId { get; set; }

        [Required]
        public int VotingSessionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public VoteChoice Choice { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public DateTime VotedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("VotingSessionId")]
        public virtual VotingSession VotingSession { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
