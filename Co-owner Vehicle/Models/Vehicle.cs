using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum VehicleType
    {
        ElectricCar = 1,    // Xe điện
        HybridCar = 2,      // Xe hybrid
        GasolineCar = 3,    // Xe xăng
        Motorcycle = 4,     // Xe máy
        Bicycle = 5         // Xe đạp điện
    }

    public enum VehicleStatus
    {
        Active = 1,         // Hoạt động
        Maintenance = 2,    // Bảo dưỡng
        Repair = 3,         // Sửa chữa
        Inactive = 4,       // Không hoạt động
        Sold = 5            // Đã bán
    }

    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        public VehicleStatus Status { get; set; } = VehicleStatus.Active;

        [StringLength(50)]
        public string? Color { get; set; }

        public int? Year { get; set; }

        [StringLength(17)]
        public string? VIN { get; set; } // Vehicle Identification Number

        [StringLength(100)]
        public string? EngineNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PurchasePrice { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int? CurrentMileage { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<CoOwnerGroup> CoOwnerGroups { get; set; } = new List<CoOwnerGroup>();
        public virtual ICollection<BookingSchedule> BookingSchedules { get; set; } = new List<BookingSchedule>();
        public virtual ICollection<VehicleUsageHistory> VehicleUsageHistories { get; set; } = new List<VehicleUsageHistory>();
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public virtual ICollection<CheckInOutRecord> CheckInOutRecords { get; set; } = new List<CheckInOutRecord>();
        public virtual ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();

        // Computed Properties
        [NotMapped]
        public string FullName => $"{Brand} {Model} ({LicensePlate})";
    }
}
