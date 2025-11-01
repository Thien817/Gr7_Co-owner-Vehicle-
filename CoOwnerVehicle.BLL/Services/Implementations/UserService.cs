using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Co_owner_Vehicle.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IUserRoleRepository _userRoles;

        public UserService(IUserRepository users, IRoleRepository roles, IUserRoleRepository userRoles)
        {
            _users = users;
            _roles = roles;
            _userRoles = userRoles;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _users.GetQueryable()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _users.GetQueryable()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByCitizenIdAsync(string citizenId)
        {
            return await _users.GetQueryable()
                .FirstOrDefaultAsync(u => u.CitizenId == citizenId);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _users.GetQueryable()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _users.GetQueryable()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.IsActive && u.UserRoles.Any(ur => ur.Role.RoleName == roleName && ur.IsActive))
                .ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            user.IsVerified = false;

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _users.Update(user);
            await _users.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            _users.Update(user);
            await _users.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyUserAsync(int userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            _users.Update(user);
            await _users.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, string roleName)
        {
            var user = await _users.GetByIdAsync(userId);
            var role = await _roles.GetQueryable()
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
            
            if (user == null || role == null) return false;

            // Check if user already has this role
            var existingUserRole = await _userRoles.GetQueryable()
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);

            if (existingUserRole != null)
            {
                existingUserRole.IsActive = true;
                existingUserRole.AssignedAt = DateTime.UtcNow;
                _userRoles.Update(existingUserRole);
            }
            else
            {
                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = role.RoleId,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await _userRoles.AddAsync(userRole);
            }

            await _userRoles.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, string roleName)
        {
            var role = await _roles.GetQueryable()
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
            if (role == null) return false;

            var userRole = await _userRoles.GetQueryable()
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);

            if (userRole == null) return false;

            userRole.IsActive = false;
            _userRoles.Update(userRole);
            await _userRoles.SaveChangesAsync();
            return true;
        }

        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            return await _userRoles.GetQueryable()
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<bool> IsUserInRoleAsync(int userId, string roleName)
        {
            return await _userRoles.GetQueryable()
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.UserId == userId && ur.Role.RoleName == roleName && ur.IsActive);
        }

        public async Task<bool> ValidatePasswordAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null) return false;

            // Direct password comparison (plain text)
            return user.PasswordHash == password;
        }

        public Task<string> HashPasswordAsync(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Task.FromResult(Convert.ToBase64String(hashedBytes));
        }

        public async Task<List<User>> GetRecentUsersAsync(int count = 10)
        {
            return await _users.GetQueryable()
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetUnverifiedUsersCountAsync()
        {
            return await _users.GetQueryable()
                .CountAsync(u => !u.IsVerified && u.IsActive);
        }

        public async Task<int> GetPendingVerificationsCountAsync()
        {
            return await _users.GetQueryable()
                .SelectMany(u => u.UserVerifications)
                .CountAsync(uv => uv.Status == VerificationStatus.Pending);
        }
    }
}
