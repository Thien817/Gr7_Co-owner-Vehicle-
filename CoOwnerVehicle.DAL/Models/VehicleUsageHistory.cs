using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public class VehicleUsageHistory
    {
        [Key]
        public int VehicleUsageHistoryId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? BookingScheduleId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public int? StartMileage { get; set; }

        public int? EndMileage { get; set; }

        [NotMapped]
        public int? TotalMileage => EndMileage - StartMileage;

        [StringLength(200)]
        public string? StartLocation { get; set; }

        [StringLength(200)]
        public string? EndLocation { get; set; }

        [StringLength(1000)]
        public string? UsageNotes { get; set; }

        [StringLength(500)]
        public string? IssuesReported { get; set; } // Vấn đề phát hiện

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("BookingScheduleId")]
        public virtual BookingSchedule? BookingSchedule { get; set; }
    }
}
