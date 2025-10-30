using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Co_owner_Vehicle.Helpers
{
    public static class UserHelper
    {
        /// <summary>
        /// Get current user ID from claims
        /// </summary>
        public static int GetCurrentUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("UserId")?.Value 
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Get current user email from claims
        /// </summary>
        public static string GetCurrentUserEmail(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value 
                ?? user.FindFirst(ClaimTypes.Email)?.Value 
                ?? string.Empty;
        }

        /// <summary>
        /// Get current user full name from claims
        /// </summary>
        public static string GetCurrentUserFullName(ClaimsPrincipal user)
        {
            return user.FindFirst("FullName")?.Value 
                ?? $"{user.FindFirst(ClaimTypes.GivenName)?.Value} {user.FindFirst(ClaimTypes.Surname)?.Value}"
                ?? string.Empty;
        }

        /// <summary>
        /// Check if user is in specific role
        /// </summary>
        public static bool IsInRole(ClaimsPrincipal user, string roleName)
        {
            return user.IsInRole(roleName);
        }

        /// <summary>
        /// Check if user is Admin
        /// </summary>
        public static bool IsAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        /// <summary>
        /// Check if user is Staff
        /// </summary>
        public static bool IsStaff(ClaimsPrincipal user)
        {
            return user.IsInRole("Staff");
        }

        /// <summary>
        /// Check if user is Co-owner
        /// </summary>
        public static bool IsCoOwner(ClaimsPrincipal user)
        {
            return user.IsInRole("Co-owner");
        }

        /// <summary>
        /// Get all roles of current user
        /// </summary>
        public static List<string> GetUserRoles(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }
    }

    /// <summary>
    /// Extension methods for PageModel to easily access user info
    /// </summary>
    public static class PageModelExtensions
    {
        public static int GetCurrentUserId(this PageModel pageModel)
        {
            return UserHelper.GetCurrentUserId(pageModel.User);
        }

        public static string GetCurrentUserEmail(this PageModel pageModel)
        {
            return UserHelper.GetCurrentUserEmail(pageModel.User);
        }

        public static string GetCurrentUserFullName(this PageModel pageModel)
        {
            return UserHelper.GetCurrentUserFullName(pageModel.User);
        }

        public static bool IsInRole(this PageModel pageModel, string roleName)
        {
            return UserHelper.IsInRole(pageModel.User, roleName);
        }

        public static bool IsAdmin(this PageModel pageModel)
        {
            return UserHelper.IsAdmin(pageModel.User);
        }

        public static bool IsStaff(this PageModel pageModel)
        {
            return UserHelper.IsStaff(pageModel.User);
        }

        public static bool IsCoOwner(this PageModel pageModel)
        {
            return UserHelper.IsCoOwner(pageModel.User);
        }

        public static List<string> GetUserRoles(this PageModel pageModel)
        {
            return UserHelper.GetUserRoles(pageModel.User);
        }
    }
}

