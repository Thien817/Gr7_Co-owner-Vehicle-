using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Data
{
    public class CoOwnerVehicleDbContext : DbContext
    {
        public CoOwnerVehicleDbContext(DbContextOptions<CoOwnerVehicleDbContext> options) : base(options)
        {
        }

        // User Management
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Vehicle & Group Management
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<CoOwnerGroup> CoOwnerGroups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<OwnershipShare> OwnershipShares { get; set; }
        public DbSet<EContract> EContracts { get; set; }

        // Scheduling & Usage
        public DbSet<BookingSchedule> BookingSchedules { get; set; }
        public DbSet<VehicleUsageHistory> VehicleUsageHistories { get; set; }

        // Costs & Payments
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CommonFund> CommonFunds { get; set; }
        public DbSet<FundTransaction> FundTransactions { get; set; }

        // Group Management & Decisions
        public DbSet<VotingSession> VotingSessions { get; set; }
        public DbSet<Vote> Votes { get; set; }

        // Check-in/Check-out & Services
        public DbSet<CheckInOutRecord> CheckInOutRecords { get; set; }
        public DbSet<VehicleService> VehicleServices { get; set; }
        public DbSet<ServiceRecord> ServiceRecords { get; set; }

        // Reports & Analytics
        public DbSet<FinancialReport> FinancialReports { get; set; }
        public DbSet<UsageAnalytics> UsageAnalytics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Role many-to-many relationship
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => ur.UserRoleId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User-Verification relationship
            modelBuilder.Entity<UserVerification>()
                .HasOne(uv => uv.User)
                .WithMany(u => u.UserVerifications)
                .HasForeignKey(uv => uv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserVerification>()
                .HasOne(uv => uv.ReviewedByUser)
                .WithMany()
                .HasForeignKey(uv => uv.ReviewedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Notification relationship
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Vehicle-Group relationship
            modelBuilder.Entity<CoOwnerGroup>()
                .HasOne(cog => cog.Vehicle)
                .WithMany(v => v.CoOwnerGroups)
                .HasForeignKey(cog => cog.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CoOwnerGroup>()
                .HasOne(cog => cog.CreatedByUser)
                .WithMany()
                .HasForeignKey(cog => cog.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Group Member relationships
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.CoOwnerGroup)
                .WithMany(cog => cog.GroupMembers)
                .HasForeignKey(gm => gm.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.InvitedByUser)
                .WithMany()
                .HasForeignKey(gm => gm.InvitedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Ownership Share relationships
            modelBuilder.Entity<OwnershipShare>()
                .HasOne(os => os.CoOwnerGroup)
                .WithMany(cog => cog.OwnershipShares)
                .HasForeignKey(os => os.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OwnershipShare>()
                .HasOne(os => os.User)
                .WithMany()
                .HasForeignKey(os => os.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure E-Contract relationships
            modelBuilder.Entity<EContract>()
                .HasOne(ec => ec.CoOwnerGroup)
                .WithMany(cog => cog.EContracts)
                .HasForeignKey(ec => ec.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EContract>()
                .HasOne(ec => ec.CreatedByUser)
                .WithMany()
                .HasForeignKey(ec => ec.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EContract>()
                .HasOne(ec => ec.SignedByUser)
                .WithMany()
                .HasForeignKey(ec => ec.SignedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Booking Schedule relationships
            modelBuilder.Entity<BookingSchedule>()
                .HasOne(bs => bs.Vehicle)
                .WithMany(v => v.BookingSchedules)
                .HasForeignKey(bs => bs.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookingSchedule>()
                .HasOne(bs => bs.User)
                .WithMany(u => u.BookingSchedules)
                .HasForeignKey(bs => bs.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookingSchedule>()
                .HasOne(bs => bs.ConfirmedByUser)
                .WithMany()
                .HasForeignKey(bs => bs.ConfirmedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Vehicle Usage History relationships
            modelBuilder.Entity<VehicleUsageHistory>()
                .HasOne(vuh => vuh.Vehicle)
                .WithMany(v => v.VehicleUsageHistories)
                .HasForeignKey(vuh => vuh.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VehicleUsageHistory>()
                .HasOne(vuh => vuh.User)
                .WithMany()
                .HasForeignKey(vuh => vuh.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VehicleUsageHistory>()
                .HasOne(vuh => vuh.BookingSchedule)
                .WithMany(bs => bs.VehicleUsageHistories)
                .HasForeignKey(vuh => vuh.BookingScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Expense relationships
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Vehicle)
                .WithMany(v => v.Expenses)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.ExpenseCategory)
                .WithMany(ec => ec.Expenses)
                .HasForeignKey(e => e.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.CoOwnerGroup)
                .WithMany(cog => cog.Expenses)
                .HasForeignKey(e => e.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.ApprovedByUser)
                .WithMany()
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Payment relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Expense)
                .WithMany(e => e.Payments)
                .HasForeignKey(p => p.ExpenseId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Common Fund relationships
            modelBuilder.Entity<CommonFund>()
                .HasOne(cf => cf.CoOwnerGroup)
                .WithMany(cog => cog.CommonFunds)
                .HasForeignKey(cf => cf.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FundTransaction>()
                .HasOne(ft => ft.CommonFund)
                .WithMany(cf => cf.FundTransactions)
                .HasForeignKey(ft => ft.CommonFundId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FundTransaction>()
                .HasOne(ft => ft.RelatedExpense)
                .WithMany()
                .HasForeignKey(ft => ft.RelatedExpenseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FundTransaction>()
                .HasOne(ft => ft.ProcessedByUser)
                .WithMany()
                .HasForeignKey(ft => ft.ProcessedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Voting relationships
            modelBuilder.Entity<VotingSession>()
                .HasOne(vs => vs.CoOwnerGroup)
                .WithMany(cog => cog.VotingSessions)
                .HasForeignKey(vs => vs.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VotingSession>()
                .HasOne(vs => vs.CreatedByUser)
                .WithMany()
                .HasForeignKey(vs => vs.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.VotingSession)
                .WithMany(vs => vs.Votes)
                .HasForeignKey(v => v.VotingSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Check-in/Check-out relationships
            modelBuilder.Entity<CheckInOutRecord>()
                .HasOne(cior => cior.Vehicle)
                .WithMany(v => v.CheckInOutRecords)
                .HasForeignKey(cior => cior.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CheckInOutRecord>()
                .HasOne(cior => cior.User)
                .WithMany(u => u.CheckInOutRecords)
                .HasForeignKey(cior => cior.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CheckInOutRecord>()
                .HasOne(cior => cior.BookingSchedule)
                .WithMany()
                .HasForeignKey(cior => cior.BookingScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CheckInOutRecord>()
                .HasOne(cior => cior.ProcessedByUser)
                .WithMany()
                .HasForeignKey(cior => cior.ProcessedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Service relationships
            modelBuilder.Entity<ServiceRecord>()
                .HasOne(sr => sr.Vehicle)
                .WithMany(v => v.ServiceRecords)
                .HasForeignKey(sr => sr.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ServiceRecord>()
                .HasOne(sr => sr.VehicleService)
                .WithMany(vs => vs.ServiceRecords)
                .HasForeignKey(sr => sr.VehicleServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRecord>()
                .HasOne(sr => sr.PerformedByUser)
                .WithMany()
                .HasForeignKey(sr => sr.PerformedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Report relationships
            modelBuilder.Entity<FinancialReport>()
                .HasOne(fr => fr.CoOwnerGroup)
                .WithMany(cog => cog.FinancialReports)
                .HasForeignKey(fr => fr.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FinancialReport>()
                .HasOne(fr => fr.GeneratedByUser)
                .WithMany()
                .HasForeignKey(fr => fr.GeneratedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UsageAnalytics>()
                .HasOne(ua => ua.CoOwnerGroup)
                .WithMany()
                .HasForeignKey(ua => ua.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UsageAnalytics>()
                .HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for better performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.CitizenId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.DriverLicenseNumber);

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.LicensePlate)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VIN);

            modelBuilder.Entity<BookingSchedule>()
                .HasIndex(bs => new { bs.VehicleId, bs.StartTime, bs.EndTime });

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.TransactionId);

            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.VotingSessionId, v.UserId })
                .IsUnique();

            // Configure decimal precision
            modelBuilder.Entity<OwnershipShare>()
                .Property(os => os.Percentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CommonFund>()
                .Property(cf => cf.CurrentBalance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CommonFund>()
                .Property(cf => cf.TargetAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FundTransaction>()
                .Property(ft => ft.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FinancialReport>()
                .Property(fr => fr.TotalExpenses)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FinancialReport>()
                .Property(fr => fr.TotalPayments)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FinancialReport>()
                .Property(fr => fr.OutstandingAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UsageAnalytics>()
                .Property(ua => ua.UsagePercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<UsageAnalytics>()
                .Property(ua => ua.OwnershipPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<UsageAnalytics>()
                .Property(ua => ua.FairnessScore)
                .HasPrecision(5, 2);

            // ============================================
            // SEED DATA - Dữ liệu mẫu ban đầu
            // ============================================

            SeedRoles(modelBuilder);
            SeedExpenseCategories(modelBuilder);
            SeedUsers(modelBuilder);
            SeedVehicles(modelBuilder);
            SeedVehicleServices(modelBuilder);
            SeedUserRoles(modelBuilder);
            SeedCoOwnerGroups(modelBuilder);
            SeedGroupMembers(modelBuilder);
            SeedOwnershipShares(modelBuilder);
            SeedCommonFunds(modelBuilder);
            // SeedBookingSchedules(modelBuilder); // Commented out to avoid hardcoded bookings
            SeedExpenses(modelBuilder);
            SeedVotingSessions(modelBuilder);
        }

        // ============================================
        // SEED DATA METHODS
        // ============================================

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "Quản trị viên hệ thống", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 2, RoleName = "Staff", Description = "Nhân viên vận hành", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Role { RoleId = 3, RoleName = "Co-owner", Description = "Chủ xe đồng sở hữu", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedExpenseCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseCategory>().HasData(
                new ExpenseCategory { ExpenseCategoryId = 1, CategoryName = "Phí sạc điện", CategoryType = ExpenseCategoryType.Charging, Description = "Chi phí sạc điện cho xe điện", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 2, CategoryName = "Bảo dưỡng định kỳ", CategoryType = ExpenseCategoryType.Maintenance, Description = "Chi phí bảo dưỡng định kỳ", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 3, CategoryName = "Bảo hiểm xe", CategoryType = ExpenseCategoryType.Insurance, Description = "Phí bảo hiểm phương tiện", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 4, CategoryName = "Đăng kiểm", CategoryType = ExpenseCategoryType.Registration, Description = "Phí đăng kiểm phương tiện", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 5, CategoryName = "Vệ sinh xe", CategoryType = ExpenseCategoryType.Cleaning, Description = "Chi phí vệ sinh và làm sạch xe", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 6, CategoryName = "Sửa chữa", CategoryType = ExpenseCategoryType.Repair, Description = "Chi phí sửa chữa khi có hỏng hóc", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 7, CategoryName = "Nhiên liệu", CategoryType = ExpenseCategoryType.Fuel, Description = "Chi phí nhiên liệu (xăng, dầu)", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 8, CategoryName = "Phí đỗ xe", CategoryType = ExpenseCategoryType.Parking, Description = "Chi phí đỗ xe tại các bãi đỗ", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 9, CategoryName = "Phí cầu đường", CategoryType = ExpenseCategoryType.Toll, Description = "Phí cầu đường và cao tốc", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ExpenseCategory { ExpenseCategoryId = 10, CategoryName = "Khác", CategoryType = ExpenseCategoryType.Other, Description = "Các chi phí khác", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, FirstName = "Admin", LastName = "System", Email = "admin@coowner.com", PhoneNumber = "0901234567", PasswordHash = "admin123", IsActive = true, IsVerified = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { UserId = 2, FirstName = "Staff", LastName = "Operator", Email = "staff@coowner.com", PhoneNumber = "0905555555", PasswordHash = "staff123", IsActive = true, IsVerified = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { UserId = 3, FirstName = "Nguyễn Văn", LastName = "A", Email = "nguyenvana@email.com", PhoneNumber = "0901111111", PasswordHash = "user123", CitizenId = "123456789", DriverLicenseNumber = "DL123456", DateOfBirth = new DateTime(1990, 5, 15), Address = "123 Đường ABC, Quận 1, TP.HCM", IsActive = true, IsVerified = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { UserId = 4, FirstName = "Trần Thị", LastName = "B", Email = "tranthib@email.com", PhoneNumber = "0902222222", PasswordHash = "user123", CitizenId = "987654321", DriverLicenseNumber = "DL789012", DateOfBirth = new DateTime(1985, 8, 20), Address = "456 Đường XYZ, Quận 2, TP.HCM", IsActive = true, IsVerified = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { UserId = 5, FirstName = "Lê Văn", LastName = "C", Email = "levanc@email.com", PhoneNumber = "0903333333", PasswordHash = "user123", CitizenId = "456789123", DriverLicenseNumber = "DL345678", DateOfBirth = new DateTime(1992, 12, 10), Address = "789 Đường DEF, Quận 3, TP.HCM", IsActive = true, IsVerified = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { UserId = 6, FirstName = "Phạm Thị", LastName = "D", Email = "phamthid@email.com", PhoneNumber = "0904444444", PasswordHash = "user123", CitizenId = "789123456", DriverLicenseNumber = "DL901234", DateOfBirth = new DateTime(1988, 3, 25), Address = "321 Đường GHI, Quận 4, TP.HCM", IsActive = true, IsVerified = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { UserId = 7, FirstName = "Hoàng Văn", LastName = "E", Email = "hoangvane@email.com", PhoneNumber = "0906666666", PasswordHash = "user123", CitizenId = "111222333", DriverLicenseNumber = "DL567890", DateOfBirth = new DateTime(1987, 7, 12), Address = "555 Đường JKL, Quận 5, TP.HCM", IsActive = true, IsVerified = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { UserId = 8, FirstName = "Võ Thị", LastName = "F", Email = "vothif@email.com", PhoneNumber = "0907777777", PasswordHash = "user123", CitizenId = "444555666", DriverLicenseNumber = "DL123789", DateOfBirth = new DateTime(1991, 11, 8), Address = "777 Đường MNO, Quận 6, TP.HCM", IsActive = true, IsVerified = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedVehicles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { VehicleId = 1, Brand = "Tesla", Model = "Model 3", LicensePlate = "30A-12345", VehicleType = VehicleType.ElectricCar, Status = VehicleStatus.Active, Color = "Đen", Year = 2023, VIN = "1HGBH41JXMN109186", PurchasePrice = 1200000000, PurchaseDate = new DateTime(2023, 1, 15), CurrentMileage = 15000, Description = "Tesla Model 3 màu đen, xe điện cao cấp", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Vehicle { VehicleId = 2, Brand = "Toyota", Model = "Prius", LicensePlate = "30B-67890", VehicleType = VehicleType.HybridCar, Status = VehicleStatus.Active, Color = "Trắng", Year = 2022, VIN = "1HGBH41JXMN109187", PurchasePrice = 800000000, PurchaseDate = new DateTime(2022, 6, 10), CurrentMileage = 25000, Description = "Toyota Prius hybrid, tiết kiệm nhiên liệu", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Vehicle { VehicleId = 3, Brand = "Honda", Model = "Civic", LicensePlate = "30C-11111", VehicleType = VehicleType.GasolineCar, Status = VehicleStatus.Active, Color = "Xám", Year = 2021, VIN = "1HGBH41JXMN109188", PurchasePrice = 600000000, PurchaseDate = new DateTime(2021, 3, 20), CurrentMileage = 35000, Description = "Honda Civic sedan, động cơ xăng", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Vehicle { VehicleId = 4, Brand = "VinFast", Model = "VF8", LicensePlate = "30D-22222", VehicleType = VehicleType.ElectricCar, Status = VehicleStatus.Active, Color = "Xanh", Year = 2023, VIN = "1HGBH41JXMN109189", PurchasePrice = 900000000, PurchaseDate = new DateTime(2023, 5, 8), CurrentMileage = 8000, Description = "VinFast VF8 SUV điện, thương hiệu Việt Nam", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedVehicleServices(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VehicleService>().HasData(
                new VehicleService { VehicleServiceId = 1, ServiceName = "Bảo dưỡng định kỳ 10,000km", ServiceType = ServiceType.Maintenance, Description = "Thay dầu, lọc gió, kiểm tra hệ thống", EstimatedCost = 2000000, EstimatedDurationMinutes = 120, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new VehicleService { VehicleServiceId = 2, ServiceName = "Bảo dưỡng định kỳ 20,000km", ServiceType = ServiceType.Maintenance, Description = "Thay dầu, lọc gió, kiểm tra phanh, hệ thống điện", EstimatedCost = 3500000, EstimatedDurationMinutes = 180, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new VehicleService { VehicleServiceId = 3, ServiceName = "Kiểm tra pin xe điện", ServiceType = ServiceType.Inspection, Description = "Kiểm tra tình trạng pin, hệ thống sạc", EstimatedCost = 1500000, EstimatedDurationMinutes = 90, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new VehicleService { VehicleServiceId = 4, ServiceName = "Vệ sinh nội thất", ServiceType = ServiceType.Cleaning, Description = "Vệ sinh ghế, dashboard, sàn xe", EstimatedCost = 500000, EstimatedDurationMinutes = 60, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new VehicleService { VehicleServiceId = 5, ServiceName = "Rửa xe ngoại thất", ServiceType = ServiceType.Cleaning, Description = "Rửa xe, đánh bóng, bảo vệ sơn", EstimatedCost = 300000, EstimatedDurationMinutes = 45, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new VehicleService { VehicleServiceId = 6, ServiceName = "Sửa chữa hệ thống phanh", ServiceType = ServiceType.Repair, Description = "Kiểm tra và sửa chữa hệ thống phanh", EstimatedCost = 5000000, EstimatedDurationMinutes = 240, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedUserRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserRoleId = 1, UserId = 1, RoleId = 1, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true },
                new UserRole { UserRoleId = 2, UserId = 2, RoleId = 2, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true },
                new UserRole { UserRoleId = 3, UserId = 3, RoleId = 3, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true },
                new UserRole { UserRoleId = 4, UserId = 4, RoleId = 3, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true },
                new UserRole { UserRoleId = 5, UserId = 5, RoleId = 3, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true },
                new UserRole { UserRoleId = 6, UserId = 6, RoleId = 3, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true },
                new UserRole { UserRoleId = 7, UserId = 7, RoleId = 3, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true },
                new UserRole { UserRoleId = 8, UserId = 8, RoleId = 3, AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true }
            );
        }

        private static void SeedCoOwnerGroups(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoOwnerGroup>().HasData(
                new CoOwnerGroup { CoOwnerGroupId = 1, GroupName = "Nhóm Tesla Model 3", Description = "Nhóm đồng sở hữu Tesla Model 3 màu đen - 3 thành viên", VehicleId = 1, CreatedBy = 1, Status = GroupStatus.Active, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), ActivatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CoOwnerGroup { CoOwnerGroupId = 2, GroupName = "Nhóm Toyota Prius", Description = "Nhóm đồng sở hữu Toyota Prius hybrid - 3 thành viên", VehicleId = 2, CreatedBy = 1, Status = GroupStatus.Active, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), ActivatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CoOwnerGroup { CoOwnerGroupId = 3, GroupName = "Nhóm Honda Civic", Description = "Nhóm đồng sở hữu Honda Civic sedan - 2 thành viên", VehicleId = 3, CreatedBy = 1, Status = GroupStatus.Active, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), ActivatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CoOwnerGroup { CoOwnerGroupId = 4, GroupName = "Nhóm VinFast VF8", Description = "Nhóm đồng sở hữu VinFast VF8 SUV điện - 2 thành viên", VehicleId = 4, CreatedBy = 1, Status = GroupStatus.Active, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), ActivatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedGroupMembers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupMember>().HasData(
                // Tesla Group
                new GroupMember { GroupMemberId = 1, CoOwnerGroupId = 1, UserId = 3, Role = MemberRole.Admin, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new GroupMember { GroupMemberId = 2, CoOwnerGroupId = 1, UserId = 4, Role = MemberRole.Owner, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new GroupMember { GroupMemberId = 3, CoOwnerGroupId = 1, UserId = 5, Role = MemberRole.Owner, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                // Toyota Group
                new GroupMember { GroupMemberId = 4, CoOwnerGroupId = 2, UserId = 4, Role = MemberRole.Admin, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new GroupMember { GroupMemberId = 5, CoOwnerGroupId = 2, UserId = 5, Role = MemberRole.Owner, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new GroupMember { GroupMemberId = 6, CoOwnerGroupId = 2, UserId = 6, Role = MemberRole.Owner, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                // Honda Group
                new GroupMember { GroupMemberId = 7, CoOwnerGroupId = 3, UserId = 7, Role = MemberRole.Admin, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new GroupMember { GroupMemberId = 8, CoOwnerGroupId = 3, UserId = 8, Role = MemberRole.Owner, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                // VinFast Group
                new GroupMember { GroupMemberId = 9, CoOwnerGroupId = 4, UserId = 3, Role = MemberRole.Admin, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new GroupMember { GroupMemberId = 10, CoOwnerGroupId = 4, UserId = 7, Role = MemberRole.Owner, Status = MemberStatus.Active, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedOwnershipShares(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OwnershipShare>().HasData(
                // Tesla Group
                new OwnershipShare { OwnershipShareId = 1, CoOwnerGroupId = 1, UserId = 3, Percentage = 40.00m, InvestmentAmount = 480000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OwnershipShare { OwnershipShareId = 2, CoOwnerGroupId = 1, UserId = 4, Percentage = 35.00m, InvestmentAmount = 420000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OwnershipShare { OwnershipShareId = 3, CoOwnerGroupId = 1, UserId = 5, Percentage = 25.00m, InvestmentAmount = 300000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                // Toyota Group
                new OwnershipShare { OwnershipShareId = 4, CoOwnerGroupId = 2, UserId = 4, Percentage = 50.00m, InvestmentAmount = 400000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OwnershipShare { OwnershipShareId = 5, CoOwnerGroupId = 2, UserId = 5, Percentage = 30.00m, InvestmentAmount = 240000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OwnershipShare { OwnershipShareId = 6, CoOwnerGroupId = 2, UserId = 6, Percentage = 20.00m, InvestmentAmount = 160000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                // Honda Group
                new OwnershipShare { OwnershipShareId = 7, CoOwnerGroupId = 3, UserId = 7, Percentage = 60.00m, InvestmentAmount = 360000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OwnershipShare { OwnershipShareId = 8, CoOwnerGroupId = 3, UserId = 8, Percentage = 40.00m, InvestmentAmount = 240000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                // VinFast Group
                new OwnershipShare { OwnershipShareId = 9, CoOwnerGroupId = 4, UserId = 3, Percentage = 70.00m, InvestmentAmount = 630000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OwnershipShare { OwnershipShareId = 10, CoOwnerGroupId = 4, UserId = 7, Percentage = 30.00m, InvestmentAmount = 270000000, EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedCommonFunds(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommonFund>().HasData(
                new CommonFund { CommonFundId = 1, CoOwnerGroupId = 1, FundName = "Quỹ chung Tesla Model 3", FundType = FundType.Maintenance, Description = "Quỹ chung cho chi phí vận hành xe Tesla", CurrentBalance = 5000000, TargetAmount = 10000000, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CommonFund { CommonFundId = 2, CoOwnerGroupId = 2, FundName = "Quỹ chung Toyota Prius", FundType = FundType.Maintenance, Description = "Quỹ chung cho chi phí vận hành xe Toyota", CurrentBalance = 3000000, TargetAmount = 8000000, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CommonFund { CommonFundId = 3, CoOwnerGroupId = 3, FundName = "Quỹ chung Honda Civic", FundType = FundType.Maintenance, Description = "Quỹ chung cho chi phí vận hành xe Honda", CurrentBalance = 2000000, TargetAmount = 5000000, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CommonFund { CommonFundId = 4, CoOwnerGroupId = 4, FundName = "Quỹ chung VinFast VF8", FundType = FundType.Maintenance, Description = "Quỹ chung cho chi phí vận hành xe VinFast", CurrentBalance = 4000000, TargetAmount = 9000000, IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }

        private static void SeedBookingSchedules(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2024, 10, 28, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<BookingSchedule>().HasData(
                new BookingSchedule { BookingScheduleId = 1, VehicleId = 1, UserId = 3, StartTime = now.AddDays(1), EndTime = now.AddDays(1).AddHours(4), Purpose = "Đi công tác", Status = BookingStatus.Confirmed, Priority = PriorityLevel.Normal, ConfirmedBy = 4, ConfirmedAt = now, CreatedAt = now },
                new BookingSchedule { BookingScheduleId = 2, VehicleId = 1, UserId = 4, StartTime = now.AddDays(3), EndTime = now.AddDays(3).AddHours(6), Purpose = "Đi du lịch cuối tuần", Status = BookingStatus.Pending, Priority = PriorityLevel.Normal, CreatedAt = now }
            );
        }

        private static void SeedExpenses(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2024, 10, 28, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Expense>().HasData(
                new Expense { ExpenseId = 1, VehicleId = 1, CoOwnerGroupId = 1, ExpenseCategoryId = 1, ExpenseTitle = "Phí sạc điện tháng 10/2024", Amount = 500000, Description = "Phí sạc điện tháng 10/2024", ExpenseDate = now.AddDays(-10), Status = ExpenseStatus.Approved, SplitMethod = SplitMethod.ByOwnership, ApprovedBy = 3, ApprovedAt = now.AddDays(-5), CreatedAt = now.AddDays(-10) },
                new Expense { ExpenseId = 2, VehicleId = 1, CoOwnerGroupId = 1, ExpenseCategoryId = 2, ExpenseTitle = "Bảo dưỡng định kỳ 15,000km", Amount = 2000000, Description = "Bảo dưỡng định kỳ 15,000km", ExpenseDate = now.AddDays(-20), Status = ExpenseStatus.Approved, SplitMethod = SplitMethod.ByOwnership, ApprovedBy = 3, ApprovedAt = now.AddDays(-15), CreatedAt = now.AddDays(-20) }
            );
        }

        private static void SeedVotingSessions(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2024, 10, 28, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<VotingSession>().HasData(
                new VotingSession { VotingSessionId = 1, CoOwnerGroupId = 1, Title = "Bỏ phiếu về việc nâng cấp hệ thống âm thanh", Description = "Thảo luận và bỏ phiếu về việc nâng cấp hệ thống âm thanh cho xe Tesla", DecisionType = DecisionType.Other, Status = VotingStatus.Active, StartDate = now, EndDate = now.AddDays(7), CreatedBy = 3, CreatedAt = now, IsPassed = false }
            );
        }
    }
}
