using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Co_owner_Vehicle.Models
{
    public enum ExpenseCategoryType
    {
        Charging = 1,        // Phí sạc điện
        Maintenance = 2,    // Bảo dưỡng
        Insurance = 3,      // Bảo hiểm
        Registration = 4,   // Đăng kiểm
        Cleaning = 5,       // Vệ sinh xe
        Repair = 6,         // Sửa chữa
        Fuel = 7,           // Nhiên liệu
        Parking = 8,        // Phí đỗ xe
        Toll = 9,           // Phí cầu đường
        Other = 10          // Khác
    }

    public class ExpenseCategory
    {
        [Key]
        public int ExpenseCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public ExpenseCategoryType CategoryType { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
