using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Services;
using Co_owner_Vehicle.Services.Interfaces;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using CoOwnerVehicle.DAL.Repositories.Implementations;
using FluentValidation;
using FluentValidation.AspNetCore;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Log connection string for debugging (only in Development)
if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Connection String: {connectionString}");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CoOwnerVehicle.BLL.Validators.User.UserCreateDtoValidator>();
builder.Services.AddAutoMapper(typeof(CoOwnerVehicle.BLL.DTOs.User.UserDto).Assembly);

// Add Entity Framework (MigrationsAssembly = DAL)
builder.Services.AddDbContext<CoOwnerVehicleDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => 
        {
            sql.MigrationsAssembly("CoOwnerVehicle.DAL");
            sql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Configure Authentication with Cookie Scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Cookie expires after 7 days
        options.SlidingExpiration = true; // Renew cookie on activity
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Default policy: require authentication
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Role-based policies
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
    options.AddPolicy("CoOwnerOnly", policy => policy.RequireRole("Co-owner"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Staff", "Admin"));
});

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICommonFundService, CommonFundService>();
builder.Services.AddScoped<IVotingService, VotingService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IServiceRecordService, ServiceRecordService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<ICheckInOutService, CheckInOutService>();

// Register DAL Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ICoOwnerGroupRepository, CoOwnerGroupRepository>();
builder.Services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
builder.Services.AddScoped<IOwnershipShareRepository, OwnershipShareRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICommonFundRepository, CommonFundRepository>();
builder.Services.AddScoped<IFundTransactionRepository, FundTransactionRepository>();
builder.Services.AddScoped<IVotingSessionRepository, VotingSessionRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<ICheckInOutRepository, CheckInOutRecordRepository>();
builder.Services.AddScoped<IServiceRecordRepository, ServiceRecordRepository>();

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CoOwnerVehicleDbContext>();
        // Apply migrations automatically
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        // Don't rethrow - let the app continue to start
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use Authentication & Authorization (order matters!)
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
