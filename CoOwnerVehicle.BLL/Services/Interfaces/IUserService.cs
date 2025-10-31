using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByCitizenIdAsync(string citizenId);
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersByRoleAsync(string roleName);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> VerifyUserAsync(int userId);
        Task<bool> AssignRoleToUserAsync(int userId, string roleName);
        Task<bool> RemoveRoleFromUserAsync(int userId, string roleName);
        Task<List<Role>> GetUserRolesAsync(int userId);
        Task<bool> IsUserInRoleAsync(int userId, string roleName);
        Task<bool> ValidatePasswordAsync(string email, string password);
        Task<string> HashPasswordAsync(string password);
        Task<List<User>> GetRecentUsersAsync(int count = 10);
        Task<int> GetUnverifiedUsersCountAsync();
        Task<int> GetPendingVerificationsCountAsync();
    }
}
