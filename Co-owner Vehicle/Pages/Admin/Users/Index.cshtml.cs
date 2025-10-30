using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public List<User> UnverifiedUsers { get; set; } = new();

        public async Task OnGetAsync()
        {
            var all = await _userService.GetAllUsersAsync();
            UnverifiedUsers = all.Where(u => !u.IsVerified && u.IsActive).OrderByDescending(u => u.CreatedAt).ToList();
        }

        public async Task<IActionResult> OnPostVerifyAsync(int userId)
        {
            await _userService.VerifyUserAsync(userId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeactivateAsync(int userId)
        {
            await _userService.DeleteUserAsync(userId);
            return RedirectToPage();
        }
    }
}


