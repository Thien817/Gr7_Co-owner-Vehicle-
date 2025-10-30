using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Co_owner_Vehicle.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    ExpenseCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.ExpenseCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CitizenId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DriverLicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    VIN = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    EngineNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentMileage = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                });

            migrationBuilder.CreateTable(
                name: "VehicleServices",
                columns: table => new
                {
                    VehicleServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServices", x => x.VehicleServiceId);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleId);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVerifications",
                columns: table => new
                {
                    UserVerificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VerificationType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVerifications", x => x.UserVerificationId);
                    table.ForeignKey(
                        name: "FK_UserVerifications_Users_ReviewedBy",
                        column: x => x.ReviewedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_UserVerifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingSchedules",
                columns: table => new
                {
                    BookingScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PickupLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReturnLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstimatedMileage = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConfirmedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSchedules", x => x.BookingScheduleId);
                    table.ForeignKey(
                        name: "FK_BookingSchedules_Users_ConfirmedBy",
                        column: x => x.ConfirmedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_BookingSchedules_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_BookingSchedules_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateTable(
                name: "CoOwnerGroups",
                columns: table => new
                {
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DissolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DissolutionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoOwnerGroups", x => x.CoOwnerGroupId);
                    table.ForeignKey(
                        name: "FK_CoOwnerGroups_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_CoOwnerGroups_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateTable(
                name: "ServiceRecords",
                columns: table => new
                {
                    ServiceRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    VehicleServiceId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ServiceNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IssuesFound = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PerformedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRecords", x => x.ServiceRecordId);
                    table.ForeignKey(
                        name: "FK_ServiceRecords_Users_PerformedBy",
                        column: x => x.PerformedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ServiceRecords_VehicleServices_VehicleServiceId",
                        column: x => x.VehicleServiceId,
                        principalTable: "VehicleServices",
                        principalColumn: "VehicleServiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRecords_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateTable(
                name: "CheckInOutRecords",
                columns: table => new
                {
                    CheckInOutRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingScheduleId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CheckTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QRCodeData = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Mileage = table.Column<int>(type: "int", nullable: true),
                    VehicleCondition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProcessedBy = table.Column<int>(type: "int", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInOutRecords", x => x.CheckInOutRecordId);
                    table.ForeignKey(
                        name: "FK_CheckInOutRecords_BookingSchedules_BookingScheduleId",
                        column: x => x.BookingScheduleId,
                        principalTable: "BookingSchedules",
                        principalColumn: "BookingScheduleId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CheckInOutRecords_Users_ProcessedBy",
                        column: x => x.ProcessedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_CheckInOutRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_CheckInOutRecords_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateTable(
                name: "VehicleUsageHistories",
                columns: table => new
                {
                    VehicleUsageHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingScheduleId = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartMileage = table.Column<int>(type: "int", nullable: true),
                    EndMileage = table.Column<int>(type: "int", nullable: true),
                    StartLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EndLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UsageNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IssuesReported = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleUsageHistories", x => x.VehicleUsageHistoryId);
                    table.ForeignKey(
                        name: "FK_VehicleUsageHistories_BookingSchedules_BookingScheduleId",
                        column: x => x.BookingScheduleId,
                        principalTable: "BookingSchedules",
                        principalColumn: "BookingScheduleId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleUsageHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_VehicleUsageHistories_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateTable(
                name: "CommonFunds",
                columns: table => new
                {
                    CommonFundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false),
                    FundName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FundType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonFunds", x => x.CommonFundId);
                    table.ForeignKey(
                        name: "FK_CommonFunds_CoOwnerGroups_CoOwnerGroupId",
                        column: x => x.CoOwnerGroupId,
                        principalTable: "CoOwnerGroups",
                        principalColumn: "CoOwnerGroupId");
                });

            migrationBuilder.CreateTable(
                name: "EContracts",
                columns: table => new
                {
                    EContractId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false),
                    ContractTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContractContent = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    SignedBy = table.Column<int>(type: "int", nullable: true),
                    TerminationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TerminatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EContracts", x => x.EContractId);
                    table.ForeignKey(
                        name: "FK_EContracts_CoOwnerGroups_CoOwnerGroupId",
                        column: x => x.CoOwnerGroupId,
                        principalTable: "CoOwnerGroups",
                        principalColumn: "CoOwnerGroupId");
                    table.ForeignKey(
                        name: "FK_EContracts_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_EContracts_Users_SignedBy",
                        column: x => x.SignedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    ExpenseCategoryId = table.Column<int>(type: "int", nullable: false),
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false),
                    ExpenseTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SplitMethod = table.Column<int>(type: "int", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceiptPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_CoOwnerGroups_CoOwnerGroupId",
                        column: x => x.CoOwnerGroupId,
                        principalTable: "CoOwnerGroups",
                        principalColumn: "CoOwnerGroupId");
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "ExpenseCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Expenses_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateTable(
                name: "FinancialReports",
                columns: table => new
                {
                    FinancialReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false),
                    ReportTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReportType = table.Column<int>(type: "int", nullable: false),
                    ReportPeriod = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalExpenses = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPayments = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportData = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ReportFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedBy = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialReports", x => x.FinancialReportId);
                    table.ForeignKey(
                        name: "FK_FinancialReports_CoOwnerGroups_CoOwnerGroupId",
                        column: x => x.CoOwnerGroupId,
                        principalTable: "CoOwnerGroups",
                        principalColumn: "CoOwnerGroupId");
                    table.ForeignKey(
                        name: "FK_FinancialReports_Users_GeneratedBy",
                        column: x => x.GeneratedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    GroupMemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeftAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaveReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InvitedBy = table.Column<int>(type: "int", nullable: true),
                    InvitedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.GroupMemberId);
                    table.ForeignKey(
                        name: "FK_GroupMembers_CoOwnerGroups_CoOwnerGroupId",
                        column: x => x.CoOwnerGroupId,
                        principalTable: "CoOwnerGroups",
                        principalColumn: "CoOwnerGroupId");
                    table.ForeignKey(
                        name: "FK_GroupMembers_Users_InvitedBy",
                        column: x => x.InvitedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_GroupMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnershipShares",
                columns: table => new
                {
                    OwnershipShareId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    InvestmentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnershipShares", x => x.OwnershipShareId);
                    table.ForeignKey(
                        name: "FK_OwnershipShares_CoOwnerGroups_CoOwnerGroupId",
                        column: x => x.CoOwnerGroupId,
                        principalTable: "CoOwnerGroups",
                        principalColumn: "CoOwnerGroupId");
                    table.ForeignKey(
                        name: "FK_OwnershipShares_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsageAnalytics",
                columns: table => new
                {
                    UsageAnalyticsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AnalysisDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalUsageHours = table.Column<int>(type: "int", nullable: false),
                    TotalMileage = table.Column<int>(type: "int", nullable: false),
                    BookingCount = table.Column<int>(type: "int", nullable: false),
                    UsagePercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    OwnershipPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    FairnessScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AIRecommendations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AnalysisNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageAnalytics", x => x.UsageAnalyticsId);
                    table.ForeignKey(
                        name: "FK_UsageAnalytics_CoOwnerGroups_CoOwnerGroupId",
                        column: x => x.CoOwnerGroupId,
                        principalTable: "CoOwnerGroups",
                        principalColumn: "CoOwnerGroupId");
                    table.ForeignKey(
                        name: "FK_UsageAnalytics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VotingSessions",
                columns: table => new
                {
                    VotingSessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoOwnerGroupId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DecisionType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    RequiredVotes = table.Column<int>(type: "int", nullable: true),
                    YesVotes = table.Column<int>(type: "int", nullable: true),
                    NoVotes = table.Column<int>(type: "int", nullable: true),
                    AbstainVotes = table.Column<int>(type: "int", nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResultNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingSessions", x => x.VotingSessionId);
                    table.ForeignKey(
                        name: "FK_VotingSessions_CoOwnerGroups_CoOwnerGroupId",
                        column: x => x.CoOwnerGroupId,
                        principalTable: "CoOwnerGroups",
                        principalColumn: "CoOwnerGroupId");
                    table.ForeignKey(
                        name: "FK_VotingSessions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "FundTransactions",
                columns: table => new
                {
                    FundTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommonFundId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RelatedExpenseId = table.Column<int>(type: "int", nullable: true),
                    ProcessedBy = table.Column<int>(type: "int", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundTransactions", x => x.FundTransactionId);
                    table.ForeignKey(
                        name: "FK_FundTransactions_CommonFunds_CommonFundId",
                        column: x => x.CommonFundId,
                        principalTable: "CommonFunds",
                        principalColumn: "CommonFundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundTransactions_Expenses_RelatedExpenseId",
                        column: x => x.RelatedExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId");
                    table.ForeignKey(
                        name: "FK_FundTransactions_Users_ProcessedBy",
                        column: x => x.ProcessedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ExpenseId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    VoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VotingSessionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Choice = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VotedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.VoteId);
                    table.ForeignKey(
                        name: "FK_Votes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Votes_VotingSessions_VotingSessionId",
                        column: x => x.VotingSessionId,
                        principalTable: "VotingSessions",
                        principalColumn: "VotingSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "ExpenseCategoryId", "CategoryName", "CategoryType", "CreatedAt", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "Phí sạc điện", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi phí sạc điện cho xe điện", true },
                    { 2, "Bảo dưỡng định kỳ", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi phí bảo dưỡng định kỳ", true },
                    { 3, "Bảo hiểm xe", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Phí bảo hiểm phương tiện", true },
                    { 4, "Đăng kiểm", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Phí đăng kiểm phương tiện", true },
                    { 5, "Vệ sinh xe", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi phí vệ sinh và làm sạch xe", true },
                    { 6, "Sửa chữa", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi phí sửa chữa khi có hỏng hóc", true },
                    { 7, "Nhiên liệu", 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi phí nhiên liệu (xăng, dầu)", true },
                    { 8, "Phí đỗ xe", 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi phí đỗ xe tại các bãi đỗ", true },
                    { 9, "Phí cầu đường", 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Phí cầu đường và cao tốc", true },
                    { 10, "Khác", 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Các chi phí khác", true }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "CreatedAt", "Description", "IsActive", "RoleName" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Quản trị viên hệ thống", true, "Admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nhân viên vận hành", true, "Staff" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chủ xe đồng sở hữu", true, "Co-owner" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CitizenId", "CreatedAt", "DateOfBirth", "DriverLicenseNumber", "Email", "FirstName", "IsActive", "IsVerified", "LastName", "PasswordHash", "PhoneNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "admin@coowner.com", "Admin", true, true, "System", "admin123", "0901234567", null },
                    { 2, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "staff@coowner.com", "Staff", true, true, "Operator", "staff123", "0905555555", null },
                    { 3, "123 Đường ABC, Quận 1, TP.HCM", "123456789", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL123456", "nguyenvana@email.com", "Nguyễn Văn", true, true, "A", "user123", "0901111111", null },
                    { 4, "456 Đường XYZ, Quận 2, TP.HCM", "987654321", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1985, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL789012", "tranthib@email.com", "Trần Thị", true, true, "B", "user123", "0902222222", null },
                    { 5, "789 Đường DEF, Quận 3, TP.HCM", "456789123", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1992, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL345678", "levanc@email.com", "Lê Văn", true, true, "C", "user123", "0903333333", null },
                    { 6, "321 Đường GHI, Quận 4, TP.HCM", "789123456", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1988, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL901234", "phamthid@email.com", "Phạm Thị", true, true, "D", "user123", "0904444444", null },
                    { 7, "555 Đường JKL, Quận 5, TP.HCM", "111222333", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1987, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL567890", "hoangvane@email.com", "Hoàng Văn", true, true, "E", "user123", "0906666666", null },
                    { 8, "777 Đường MNO, Quận 6, TP.HCM", "444555666", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1991, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL123789", "vothif@email.com", "Võ Thị", true, true, "F", "user123", "0907777777", null }
                });

            migrationBuilder.InsertData(
                table: "VehicleServices",
                columns: new[] { "VehicleServiceId", "CreatedAt", "Description", "EstimatedCost", "EstimatedDurationMinutes", "IsActive", "ServiceName", "ServiceType" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thay dầu, lọc gió, kiểm tra hệ thống", 2000000m, 120, true, "Bảo dưỡng định kỳ 10,000km", 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thay dầu, lọc gió, kiểm tra phanh, hệ thống điện", 3500000m, 180, true, "Bảo dưỡng định kỳ 20,000km", 1 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kiểm tra tình trạng pin, hệ thống sạc", 1500000m, 90, true, "Kiểm tra pin xe điện", 3 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vệ sinh ghế, dashboard, sàn xe", 500000m, 60, true, "Vệ sinh nội thất", 4 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rửa xe, đánh bóng, bảo vệ sơn", 300000m, 45, true, "Rửa xe ngoại thất", 4 },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kiểm tra và sửa chữa hệ thống phanh", 5000000m, 240, true, "Sửa chữa hệ thống phanh", 2 }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "VehicleId", "Brand", "Color", "CreatedAt", "CurrentMileage", "Description", "EngineNumber", "ImagePath", "LicensePlate", "Model", "PurchaseDate", "PurchasePrice", "Status", "UpdatedAt", "VIN", "VehicleType", "Year" },
                values: new object[,]
                {
                    { 1, "Tesla", "Đen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15000, "Tesla Model 3 màu đen, xe điện cao cấp", null, null, "30A-12345", "Model 3", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1200000000m, 1, null, "1HGBH41JXMN109186", 1, 2023 },
                    { 2, "Toyota", "Trắng", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25000, "Toyota Prius hybrid, tiết kiệm nhiên liệu", null, null, "30B-67890", "Prius", new DateTime(2022, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 800000000m, 1, null, "1HGBH41JXMN109187", 2, 2022 },
                    { 3, "Honda", "Xám", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 35000, "Honda Civic sedan, động cơ xăng", null, null, "30C-11111", "Civic", new DateTime(2021, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 600000000m, 1, null, "1HGBH41JXMN109188", 3, 2021 },
                    { 4, "VinFast", "Xanh", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8000, "VinFast VF8 SUV điện, thương hiệu Việt Nam", null, null, "30D-22222", "VF8", new DateTime(2023, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 900000000m, 1, null, "1HGBH41JXMN109189", 1, 2023 }
                });

            migrationBuilder.InsertData(
                table: "BookingSchedules",
                columns: new[] { "BookingScheduleId", "CancellationReason", "CancelledAt", "CompletedAt", "ConfirmedAt", "ConfirmedBy", "CreatedAt", "EndTime", "EstimatedMileage", "Notes", "PickupLocation", "Priority", "Purpose", "ReturnLocation", "StartTime", "Status", "UserId", "VehicleId" },
                values: new object[,]
                {
                    { 1, null, null, null, new DateTime(2024, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), 4, new DateTime(2024, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 10, 29, 4, 0, 0, 0, DateTimeKind.Utc), null, null, null, 2, "Đi công tác", null, new DateTime(2024, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3, 1 },
                    { 2, null, null, null, null, null, new DateTime(2024, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 10, 31, 6, 0, 0, 0, DateTimeKind.Utc), null, null, null, 2, "Đi du lịch cuối tuần", null, new DateTime(2024, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "CoOwnerGroups",
                columns: new[] { "CoOwnerGroupId", "ActivatedAt", "CreatedAt", "CreatedBy", "Description", "DissolutionReason", "DissolvedAt", "GroupName", "Status", "UpdatedAt", "VehicleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm đồng sở hữu Tesla Model 3 màu đen - 3 thành viên", null, null, "Nhóm Tesla Model 3", 1, null, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm đồng sở hữu Toyota Prius hybrid - 3 thành viên", null, null, "Nhóm Toyota Prius", 1, null, 2 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm đồng sở hữu Honda Civic sedan - 2 thành viên", null, null, "Nhóm Honda Civic", 1, null, 3 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Nhóm đồng sở hữu VinFast VF8 SUV điện - 2 thành viên", null, null, "Nhóm VinFast VF8", 1, null, 4 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserRoleId", "AssignedAt", "ExpiresAt", "IsActive", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 2, 2 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 3, 3 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 3, 4 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 3, 5 },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 3, 6 },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 3, 7 },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 3, 8 }
                });

            migrationBuilder.InsertData(
                table: "CommonFunds",
                columns: new[] { "CommonFundId", "CoOwnerGroupId", "CreatedAt", "CurrentBalance", "Description", "FundName", "FundType", "IsActive", "TargetAmount", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5000000m, "Quỹ chung cho chi phí vận hành xe Tesla", "Quỹ chung Tesla Model 3", 1, true, 10000000m, null },
                    { 2, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3000000m, "Quỹ chung cho chi phí vận hành xe Toyota", "Quỹ chung Toyota Prius", 1, true, 8000000m, null },
                    { 3, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2000000m, "Quỹ chung cho chi phí vận hành xe Honda", "Quỹ chung Honda Civic", 1, true, 5000000m, null },
                    { 4, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4000000m, "Quỹ chung cho chi phí vận hành xe VinFast", "Quỹ chung VinFast VF8", 1, true, 9000000m, null }
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "ExpenseId", "Amount", "ApprovedAt", "ApprovedBy", "CoOwnerGroupId", "CreatedAt", "Description", "ExpenseCategoryId", "ExpenseDate", "ExpenseTitle", "Notes", "ReceiptPath", "RejectionReason", "SplitMethod", "Status", "VehicleId", "VendorName" },
                values: new object[,]
                {
                    { 1, 500000m, new DateTime(2024, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), 3, 1, new DateTime(2024, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Phí sạc điện tháng 10/2024", 1, new DateTime(2024, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Phí sạc điện tháng 10/2024", null, null, null, 1, 2, 1, null },
                    { 2, 2000000m, new DateTime(2024, 10, 13, 0, 0, 0, 0, DateTimeKind.Utc), 3, 1, new DateTime(2024, 10, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Bảo dưỡng định kỳ 15,000km", 2, new DateTime(2024, 10, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Bảo dưỡng định kỳ 15,000km", null, null, null, 1, 2, 1, null }
                });

            migrationBuilder.InsertData(
                table: "GroupMembers",
                columns: new[] { "GroupMemberId", "CoOwnerGroupId", "InvitedAt", "InvitedBy", "JoinedAt", "LeaveReason", "LeftAt", "Role", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, 1, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, 1, 3 },
                    { 2, 1, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 1, 4 },
                    { 3, 1, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 1, 5 },
                    { 4, 2, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, 1, 4 },
                    { 5, 2, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 1, 5 },
                    { 6, 2, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 1, 6 },
                    { 7, 3, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, 1, 7 },
                    { 8, 3, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 1, 8 },
                    { 9, 4, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, 1, 3 },
                    { 10, 4, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 1, 7 }
                });

            migrationBuilder.InsertData(
                table: "OwnershipShares",
                columns: new[] { "OwnershipShareId", "CoOwnerGroupId", "CreatedAt", "EffectiveFrom", "EffectiveTo", "InvestmentAmount", "IsActive", "Notes", "Percentage", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 480000000m, true, null, 40.00m, null, 3 },
                    { 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 420000000m, true, null, 35.00m, null, 4 },
                    { 3, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 300000000m, true, null, 25.00m, null, 5 },
                    { 4, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 400000000m, true, null, 50.00m, null, 4 },
                    { 5, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 240000000m, true, null, 30.00m, null, 5 },
                    { 6, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 160000000m, true, null, 20.00m, null, 6 },
                    { 7, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 360000000m, true, null, 60.00m, null, 7 },
                    { 8, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 240000000m, true, null, 40.00m, null, 8 },
                    { 9, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 630000000m, true, null, 70.00m, null, 3 },
                    { 10, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 270000000m, true, null, 30.00m, null, 7 }
                });

            migrationBuilder.InsertData(
                table: "VotingSessions",
                columns: new[] { "VotingSessionId", "AbstainVotes", "CoOwnerGroupId", "CompletedAt", "CreatedAt", "CreatedBy", "DecisionType", "Description", "EndDate", "IsPassed", "NoVotes", "RequiredVotes", "ResultNotes", "StartDate", "Status", "Title", "YesVotes" },
                values: new object[] { 1, 0, 1, null, new DateTime(2024, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), 3, 8, "Thảo luận và bỏ phiếu về việc nâng cấp hệ thống âm thanh cho xe Tesla", new DateTime(2024, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), false, 0, null, null, new DateTime(2024, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Bỏ phiếu về việc nâng cấp hệ thống âm thanh", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedules_ConfirmedBy",
                table: "BookingSchedules",
                column: "ConfirmedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedules_UserId",
                table: "BookingSchedules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedules_VehicleId_StartTime_EndTime",
                table: "BookingSchedules",
                columns: new[] { "VehicleId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_CheckInOutRecords_BookingScheduleId",
                table: "CheckInOutRecords",
                column: "BookingScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInOutRecords_ProcessedBy",
                table: "CheckInOutRecords",
                column: "ProcessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInOutRecords_UserId",
                table: "CheckInOutRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInOutRecords_VehicleId",
                table: "CheckInOutRecords",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonFunds_CoOwnerGroupId",
                table: "CommonFunds",
                column: "CoOwnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CoOwnerGroups_CreatedBy",
                table: "CoOwnerGroups",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CoOwnerGroups_VehicleId",
                table: "CoOwnerGroups",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_EContracts_CoOwnerGroupId",
                table: "EContracts",
                column: "CoOwnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EContracts_CreatedBy",
                table: "EContracts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EContracts_SignedBy",
                table: "EContracts",
                column: "SignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ApprovedBy",
                table: "Expenses",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CoOwnerGroupId",
                table: "Expenses",
                column: "CoOwnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_VehicleId",
                table: "Expenses",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_CoOwnerGroupId",
                table: "FinancialReports",
                column: "CoOwnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_GeneratedBy",
                table: "FinancialReports",
                column: "GeneratedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FundTransactions_CommonFundId",
                table: "FundTransactions",
                column: "CommonFundId");

            migrationBuilder.CreateIndex(
                name: "IX_FundTransactions_ProcessedBy",
                table: "FundTransactions",
                column: "ProcessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FundTransactions_RelatedExpenseId",
                table: "FundTransactions",
                column: "RelatedExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_CoOwnerGroupId",
                table: "GroupMembers",
                column: "CoOwnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_InvitedBy",
                table: "GroupMembers",
                column: "InvitedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_UserId",
                table: "GroupMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipShares_CoOwnerGroupId",
                table: "OwnershipShares",
                column: "CoOwnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipShares_UserId",
                table: "OwnershipShares",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ExpenseId",
                table: "Payments",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecords_PerformedBy",
                table: "ServiceRecords",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecords_VehicleId",
                table: "ServiceRecords",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecords_VehicleServiceId",
                table: "ServiceRecords",
                column: "VehicleServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_UsageAnalytics_CoOwnerGroupId",
                table: "UsageAnalytics",
                column: "CoOwnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UsageAnalytics_UserId",
                table: "UsageAnalytics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CitizenId",
                table: "Users",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DriverLicenseNumber",
                table: "Users",
                column: "DriverLicenseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserVerifications_ReviewedBy",
                table: "UserVerifications",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerifications_UserId",
                table: "UserVerifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicensePlate",
                table: "Vehicles",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VIN",
                table: "Vehicles",
                column: "VIN");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleUsageHistories_BookingScheduleId",
                table: "VehicleUsageHistories",
                column: "BookingScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleUsageHistories_UserId",
                table: "VehicleUsageHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleUsageHistories_VehicleId",
                table: "VehicleUsageHistories",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId",
                table: "Votes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VotingSessionId_UserId",
                table: "Votes",
                columns: new[] { "VotingSessionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VotingSessions_CoOwnerGroupId",
                table: "VotingSessions",
                column: "CoOwnerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_VotingSessions_CreatedBy",
                table: "VotingSessions",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckInOutRecords");

            migrationBuilder.DropTable(
                name: "EContracts");

            migrationBuilder.DropTable(
                name: "FinancialReports");

            migrationBuilder.DropTable(
                name: "FundTransactions");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "OwnershipShares");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ServiceRecords");

            migrationBuilder.DropTable(
                name: "UsageAnalytics");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserVerifications");

            migrationBuilder.DropTable(
                name: "VehicleUsageHistories");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "CommonFunds");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "VehicleServices");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "BookingSchedules");

            migrationBuilder.DropTable(
                name: "VotingSessions");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "CoOwnerGroups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
