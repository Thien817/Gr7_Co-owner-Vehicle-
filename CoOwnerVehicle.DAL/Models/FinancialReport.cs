using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum ReportType
    {
        Financial = 1,      // Báo cáo tài chính
        Usage = 2,         // Báo cáo sử dụng
        Maintenance = 3,    // Báo cáo bảo dưỡng
        Ownership = 4,     // Báo cáo sở hữu
        Voting = 5         // Báo cáo bỏ phiếu
    }

    public enum ReportPeriod
    {
        Daily = 1,         // Hàng ngày
        Weekly = 2,        // Hàng tuần
        Monthly = 3,       // Hàng tháng
        Quarterly = 4,     // Hàng quý
        Yearly = 5,        // Hàng năm
        Custom = 6         // Tùy chỉnh
    }

    public class FinancialReport
    {
        [Key]
        public int FinancialReportId { get; set; }

        [Required]
        public int CoOwnerGroupId { get; set; }

        [Required]
        [StringLength(200)]
        public string ReportTitle { get; set; } = string.Empty;

        [Required]
        public ReportType ReportType { get; set; }

        [Required]
        public ReportPeriod ReportPeriod { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalExpenses { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPayments { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OutstandingAmount { get; set; } = 0;

        [StringLength(5000)]
        public string? ReportData { get; set; } // JSON data của báo cáo

        [StringLength(500)]
        public string? ReportFilePath { get; set; } // Đường dẫn file PDF/Excel

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public int? GeneratedBy { get; set; } // UserId của người tạo báo cáo

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey("CoOwnerGroupId")]
        public virtual CoOwnerGroup CoOwnerGroup { get; set; } = null!;

        [ForeignKey("GeneratedBy")]
        public virtual User? GeneratedByUser { get; set; }
    }

    public class UsageAnalytics
    {
        [Key]
        public int UsageAnalyticsId { get; set; }

        [Required]
        public int CoOwnerGroupId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;

        public int TotalUsageHours { get; set; } = 0;

        public int TotalMileage { get; set; } = 0;

        public int BookingCount { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal UsagePercentage { get; set; } = 0; // % sử dụng so với tổng

        [Column(TypeName = "decimal(5,2)")]
        public decimal OwnershipPercentage { get; set; } = 0; // % sở hữu

        [Column(TypeName = "decimal(5,2)")]
        public decimal FairnessScore { get; set; } = 0; // Điểm công bằng (0-100)

        [StringLength(2000)]
        public string? AIRecommendations { get; set; } // Gợi ý từ AI

        [StringLength(1000)]
        public string? AnalysisNotes { get; set; }

        // Navigation Properties
        [ForeignKey("CoOwnerGroupId")]
        public virtual CoOwnerGroup CoOwnerGroup { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
