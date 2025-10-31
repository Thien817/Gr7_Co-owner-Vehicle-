using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;

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

        public List<User> AllUsers { get; set; } = new();
        public List<User> UnverifiedUsers { get; set; } = new();

        public async Task OnGetAsync()
        {
            var all = await _userService.GetAllUsersAsync();
            AllUsers = all.OrderByDescending(u => u.CreatedAt).ToList();
            UnverifiedUsers = all.Where(u => !u.IsVerified && u.IsActive).OrderByDescending(u => u.CreatedAt).ToList();
        }

        public async Task<IActionResult> OnPostVerifyAsync(int userId)
        {
            try
            {
                var success = await _userService.VerifyUserAsync(userId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã duyệt người dùng thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi duyệt người dùng: {ex.Message}";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeactivateAsync(int userId)
        {
            try
            {
                var currentUserId = UserHelper.GetCurrentUserId(User);
                if (currentUserId == userId)
                {
                    TempData["ErrorMessage"] = "Bạn không thể xóa chính mình";
                    return RedirectToPage();
                }

                var success = await _userService.DeleteUserAsync(userId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã xóa người dùng thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa người dùng: {ex.Message}";
            }
            return RedirectToPage();
        }
    }
}


