using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum NotificationType
    {
        BookingApproved = 1,       // Lịch đặt được duyệt
        BookingRejected = 2,       // Lịch đặt bị từ chối
        BookingReminder = 3,       // Nhắc nhở lịch đặt
        NewExpense = 4,            // Chi phí mới
        ExpenseApproved = 5,       // Chi phí được duyệt
        ExpenseRejected = 6,       // Chi phí bị từ chối
        PaymentReminder = 7,       // Nhắc nhở thanh toán
        PaymentReceived = 8,       // Đã nhận thanh toán
        VehicleMaintenance = 9,    // Bảo dưỡng xe
        VotingSession = 10,        // Cuộc bỏ phiếu mới
        GroupInvitation = 11,      // Lời mời vào nhóm
        Other = 99                 // Khác
    }

    public enum NotificationStatus
    {
        Unread = 1,               // Chưa đọc
        Read = 2                  // Đã đọc
    }

    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        // Optional: Link to related entity
        public int? RelatedEntityId { get; set; } // BookingScheduleId, ExpenseId, etc.
        
        [StringLength(50)]
        public string? RelatedEntityType { get; set; } // "Booking", "Expense", etc.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}

