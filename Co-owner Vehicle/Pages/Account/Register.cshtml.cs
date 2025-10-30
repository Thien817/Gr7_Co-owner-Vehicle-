using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập họ")]
        public string FirstName { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string LastName { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public string? CitizenId { get; set; }

        [BindProperty]
        public string? DriverLicense { get; set; }

        [BindProperty]
        public DateTime? DateOfBirth { get; set; }

        [BindProperty]
        public string? Address { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Bạn phải đồng ý với điều khoản sử dụng")]
        public bool AgreeTerms { get; set; }

        public void OnGet()
        {
            // Display register page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if email already exists
            var existingUser = await _userService.GetUserByEmailAsync(Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(Email), "Email này đã được sử dụng.");
                return Page();
            }

            // Check if CitizenId already exists (if provided)
            if (!string.IsNullOrEmpty(CitizenId))
            {
                var existingCitizen = await _userService.GetUserByCitizenIdAsync(CitizenId);
                if (existingCitizen != null)
                {
                    ModelState.AddModelError(nameof(CitizenId), "Số CMND/CCCD này đã được sử dụng.");
                    return Page();
                }
            }

            // Create new user (plain text password - no hashing)
            var newUser = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                PasswordHash = Password, // Plain text password
                CitizenId = CitizenId,
                DriverLicenseNumber = DriverLicense,
                DateOfBirth = DateOfBirth,
                Address = Address,
                IsActive = true,
                IsVerified = false // Require admin verification
            };

            try
            {
                var createdUser = await _userService.CreateUserAsync(newUser);

                // Assign default role "Co-owner"
                await _userService.AssignRoleToUserAsync(createdUser.UserId, "Co-owner");

                TempData["SuccessMessage"] = "Đăng ký thành công! Tài khoản đang chờ xác thực từ quản trị viên.";
                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi khi đăng ký: {ex.Message}");
                return Page();
            }
        }
    }
}

