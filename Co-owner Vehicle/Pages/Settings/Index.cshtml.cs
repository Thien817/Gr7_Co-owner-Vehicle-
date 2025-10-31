using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Co_owner_Vehicle.Pages.Settings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }
    }
}

