using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IUserService userService, ILogger<LoginModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate credentials
            var isValid = await _userService.ValidatePasswordAsync(Email, Password);
            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return Page();
            }

            // Get user with roles
            var user = await _userService.GetUserByEmailAsync(Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại hoặc đã bị vô hiệu hóa.");
                return Page();
            }

            // Debug: Log user roles
            _logger.LogInformation($"User: {user.Email}, UserRoles: {user.UserRoles?.Count ?? 0}");
            if (user.UserRoles != null && user.UserRoles.Any())
            {
                foreach (var ur in user.UserRoles)
                {
                    _logger.LogInformation($"Role: {ur.Role?.RoleName}, IsActive: {ur.IsActive}");
                }
            }

            // Check if user is verified (optional check)
            if (!user.IsVerified)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản chưa được xác thực. Vui lòng liên hệ admin.");
                return Page();
            }

            // Create claims identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim("FullName", user.FullName),
                new Claim("UserId", user.UserId.ToString())
            };

            // Add role claims
            if (user.UserRoles != null && user.UserRoles.Any())
            {
                foreach (var userRole in user.UserRoles.Where(ur => ur.IsActive))
                {
                    if (userRole.Role != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
                    }
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = RememberMe,
                ExpiresUtc = RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(2)
            };

            // Sign in
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirect based on role
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Redirect to appropriate dashboard based on highest priority role
            if (user.UserRoles != null && user.UserRoles.Any(ur => ur.IsActive && ur.Role?.RoleName == "Admin"))
            {
                return RedirectToPage("/Dashboard/Admin");
            }
            else if (user.UserRoles != null && user.UserRoles.Any(ur => ur.IsActive && ur.Role?.RoleName == "Staff"))
            {
                return RedirectToPage("/Dashboard/Staff");
            }
            else if (user.UserRoles != null && user.UserRoles.Any(ur => ur.IsActive && ur.Role?.RoleName == "Co-owner"))
            {
                return RedirectToPage("/Dashboard/CoOwner");
            }

            return RedirectToPage("/Index");
        }
    }
}

