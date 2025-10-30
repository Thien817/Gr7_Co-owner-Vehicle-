using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Settings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public User? CurrentUser { get; set; }

        public async Task OnGetAsync()
        {
            var userIdStr = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                CurrentUser = await _userService.GetUserByIdAsync(userId);
            }
        }
    }
}

