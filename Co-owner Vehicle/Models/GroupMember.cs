using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum MemberRole
    {
        Owner = 1,          // Chủ sở hữu
        Admin = 2,          // Quản trị viên nhóm
        Member = 3          // Thành viên
    }

    public enum MemberStatus
    {
        Active = 1,        // Hoạt động
        Pending = 2,       // Chờ duyệt
        Suspended = 3,     // Tạm dừng
        Removed = 4        // Đã rời nhóm
    }

    public class GroupMember
    {
        [Key]
        public int GroupMemberId { get; set; }

        [Required]
        public int CoOwnerGroupId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public MemberRole Role { get; set; } = MemberRole.Member;

        [Required]
        public MemberStatus Status { get; set; } = MemberStatus.Pending;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LeftAt { get; set; }

        [StringLength(500)]
        public string? LeaveReason { get; set; }

        public int? InvitedBy { get; set; } // UserId của người mời

        public DateTime? InvitedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CoOwnerGroupId")]
        public virtual CoOwnerGroup CoOwnerGroup { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("InvitedBy")]
        public virtual User? InvitedByUser { get; set; }
    }
}
