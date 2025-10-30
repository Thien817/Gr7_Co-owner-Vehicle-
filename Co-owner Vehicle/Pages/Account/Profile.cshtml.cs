using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.ComponentModel.DataAnnotations;

namespace Co_owner_Vehicle.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IBookingService _bookingService;
        private readonly IExpenseService _expenseService;

        public ProfileModel(
            IUserService userService,
            IGroupService groupService,
            IBookingService bookingService,
            IExpenseService expenseService)
        {
            _userService = userService;
            _groupService = groupService;
            _bookingService = bookingService;
            _expenseService = expenseService;
        }

        // User Info
        public User? CurrentUser { get; set; }
        public List<string> UserRoles { get; set; } = new();

        // Activity Stats
        public ProfileStats Stats { get; set; } = new();

        // Edit Personal Info
        [BindProperty]
        public EditPersonalInfoModel EditPersonalInfo { get; set; } = new();

        // Change Password
        [BindProperty]
        public ChangePasswordModel ChangePassword { get; set; } = new();

        public class EditPersonalInfoModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập tên")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
            public string PhoneNumber { get; set; } = string.Empty;

            public DateTime? DateOfBirth { get; set; }
            public string? CitizenId { get; set; }
            public string? DriverLicenseNumber { get; set; }
            public string? Address { get; set; }
        }

        public class ChangePasswordModel
        {
            [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
            [DataType(DataType.Password)]
            [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public class ProfileStats
        {
            public int TotalVehicles { get; set; }
            public int TotalBookings { get; set; }
            public decimal TotalExpenses { get; set; }
            public decimal UsagePercentage { get; set; }
        }

        public async Task OnGetAsync()
        {
            var userId = this.GetCurrentUserId();
            if (userId == 0) return;

            await LoadUserDataAsync(userId);
            await LoadStatsAsync(userId);
        }

        public async Task<IActionResult> OnPostEditPersonalInfoAsync()
        {
            var userId = this.GetCurrentUserId();
            if (userId == 0) return RedirectToPage();

            if (!ModelState.IsValid)
            {
                await LoadUserDataAsync(userId);
                await LoadStatsAsync(userId);
                return Page();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            // Update user info
            user.FirstName = EditPersonalInfo.FirstName;
            user.LastName = EditPersonalInfo.LastName;
            user.PhoneNumber = EditPersonalInfo.PhoneNumber;
            user.DateOfBirth = EditPersonalInfo.DateOfBirth;
            user.CitizenId = EditPersonalInfo.CitizenId;
            user.DriverLicenseNumber = EditPersonalInfo.DriverLicenseNumber;
            user.Address = EditPersonalInfo.Address;

            await _userService.UpdateUserAsync(user);

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var userId = this.GetCurrentUserId();
            if (userId == 0) return RedirectToPage();

            if (!ModelState.IsValid)
            {
                await LoadUserDataAsync(userId);
                await LoadStatsAsync(userId);
                return Page();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            // Validate current password
            var isValidCurrentPassword = await _userService.ValidatePasswordAsync(user.Email, ChangePassword.CurrentPassword);
            if (!isValidCurrentPassword)
            {
                ModelState.AddModelError("ChangePassword.CurrentPassword", "Mật khẩu hiện tại không đúng.");
                await LoadUserDataAsync(userId);
                await LoadStatsAsync(userId);
                return Page();
            }

            // Update password (plain text - no hashing)
            user.PasswordHash = ChangePassword.NewPassword;
            await _userService.UpdateUserAsync(user);

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToPage();
        }

        private async Task LoadUserDataAsync(int userId)
        {
            CurrentUser = await _userService.GetUserByIdAsync(userId);
            if (CurrentUser != null)
            {
                var roles = await _userService.GetUserRolesAsync(userId);
                UserRoles = roles.Select(r => r.RoleName).ToList();

                // Initialize EditPersonalInfo with current values
                EditPersonalInfo = new EditPersonalInfoModel
                {
                    FirstName = CurrentUser.FirstName,
                    LastName = CurrentUser.LastName,
                    PhoneNumber = CurrentUser.PhoneNumber,
                    DateOfBirth = CurrentUser.DateOfBirth,
                    CitizenId = CurrentUser.CitizenId,
                    DriverLicenseNumber = CurrentUser.DriverLicenseNumber,
                    Address = CurrentUser.Address
                };
            }
        }

        private async Task LoadStatsAsync(int userId)
        {
            // Load user's groups
            var groups = await _groupService.GetGroupsByUserIdAsync(userId);
            Stats.TotalVehicles = groups.Select(g => g.VehicleId).Distinct().Count();

            // Load bookings count
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            Stats.TotalBookings = bookings.Count;

            // Load total expenses
            Stats.TotalExpenses = await _expenseService.GetTotalExpensesByUserAsync(userId);

            // Calculate usage percentage (simplified - based on bookings vs total possible)
            Stats.UsagePercentage = groups.Count > 0 ? Math.Min(100, (Stats.TotalBookings * 100.0m / Math.Max(1, groups.Count * 10))) : 0;
        }
    }
}

