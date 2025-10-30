using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum CheckInOutType
    {
        CheckIn = 1,       // Nhận xe
        CheckOut = 2       // Trả xe
    }

    public enum CheckInOutStatus
    {
        Pending = 1,        // Chờ xử lý
        Completed = 2,      // Hoàn thành
        Failed = 3          // Thất bại
    }

    public class CheckInOutRecord
    {
        [Key]
        public int CheckInOutRecordId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? BookingScheduleId { get; set; }

        [Required]
        public CheckInOutType Type { get; set; }

        [Required]
        public CheckInOutStatus Status { get; set; } = CheckInOutStatus.Pending;

        public DateTime CheckTime { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? QRCodeData { get; set; } // Dữ liệu QR code

        [StringLength(1000)]
        public string? DigitalSignature { get; set; } // Chữ ký số

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? PhotoPath { get; set; } // Đường dẫn ảnh xe

        public int? Mileage { get; set; }

        [StringLength(500)]
        public string? VehicleCondition { get; set; } // Tình trạng xe

        public int? ProcessedBy { get; set; } // UserId của staff xử lý

        public DateTime? ProcessedAt { get; set; }

        // Navigation Properties
        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("BookingScheduleId")]
        public virtual BookingSchedule? BookingSchedule { get; set; }

        [ForeignKey("ProcessedBy")]
        public virtual User? ProcessedByUser { get; set; }
    }
}
