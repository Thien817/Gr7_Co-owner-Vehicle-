using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum BookingStatus
    {
        Pending = 1,        // Chờ duyệt
        Confirmed = 2,      // Đã xác nhận
        InUse = 3,          // Đang sử dụng
        Completed = 4,      // Hoàn thành
        Cancelled = 5,      // Đã hủy
        NoShow = 6          // Không đến
    }

    public enum PriorityLevel
    {
        Low = 1,           // Thấp
        Normal = 2,        // Bình thường
        High = 3,          // Cao
        Urgent = 4         // Khẩn cấp
    }

    public class BookingSchedule
    {
        [Key]
        public int BookingScheduleId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Required]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

        [StringLength(500)]
        public string? Purpose { get; set; } // Mục đích sử dụng

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(200)]
        public string? PickupLocation { get; set; }

        [StringLength(200)]
        public string? ReturnLocation { get; set; }

        public int? EstimatedMileage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        public int? ConfirmedBy { get; set; } // UserId của người xác nhận

        // Navigation Properties
        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ConfirmedBy")]
        public virtual User? ConfirmedByUser { get; set; }

        public virtual ICollection<VehicleUsageHistory> VehicleUsageHistories { get; set; } = new List<VehicleUsageHistory>();
    }
}
