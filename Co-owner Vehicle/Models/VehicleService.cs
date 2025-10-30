using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum ServiceType
    {
        Maintenance = 1,   // Bảo dưỡng định kỳ
        Repair = 2,        // Sửa chữa
        Inspection = 3,    // Kiểm tra
        Cleaning = 4,      // Vệ sinh
        Charging = 5,      // Sạc điện
        Other = 6          // Khác
    }

    public enum ServiceStatus
    {
        Scheduled = 1,     // Đã lên lịch
        InProgress = 2,    // Đang thực hiện
        Completed = 3,     // Hoàn thành
        Cancelled = 4      // Hủy bỏ
    }

    public class VehicleService
    {
        [Key]
        public int VehicleServiceId { get; set; }

        [Required]
        [StringLength(200)]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public ServiceType ServiceType { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedCost { get; set; }

        public int? EstimatedDurationMinutes { get; set; } // Thời gian dự kiến (phút)

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
    }

    public class ServiceRecord
    {
        [Key]
        public int ServiceRecordId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int VehicleServiceId { get; set; }

        [Required]
        public ServiceStatus Status { get; set; } = ServiceStatus.Scheduled;

        public DateTime ScheduledDate { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualCost { get; set; }

        [StringLength(1000)]
        public string? ServiceNotes { get; set; }

        [StringLength(500)]
        public string? VendorName { get; set; } // Tên nhà cung cấp dịch vụ

        [StringLength(1000)]
        public string? IssuesFound { get; set; } // Vấn đề phát hiện

        [StringLength(1000)]
        public string? Recommendations { get; set; } // Khuyến nghị

        public int? PerformedBy { get; set; } // UserId của người thực hiện

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey("VehicleServiceId")]
        public virtual VehicleService VehicleService { get; set; } = null!;

        [ForeignKey("PerformedBy")]
        public virtual User? PerformedByUser { get; set; }
    }
}
