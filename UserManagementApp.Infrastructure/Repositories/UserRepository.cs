using Microsoft.EntityFrameworkCore;
using UserManagementApp.Core.Entities;
using UserManagementApp.Core.Enums;
using UserManagementApp.Core.Interfaces;
using UserManagementApp.Infrastructure.Data;

namespace UserManagementApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // THIRD REQUIREMENT: Data sorted by last login time
            return await _context.Users
                .OrderByDescending(u => u.LastLoginTime)
                .ToListAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUnverifiedUsersAsync()
        {
            var unverifiedUsers = _context.Users.Where(u => u.Status == UserStatus.Unverified);
            _context.Users.RemoveRange(unverifiedUsers);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task UpdateStatusAsync(int userId, UserStatus status)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                user.Status = status;
                await UpdateAsync(user);
            }
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginTime = DateTime.UtcNow; // Use UTC
                user.LastActivityTime = DateTime.UtcNow; // Use UTC
                await UpdateAsync(user);
            }
        }
    }
}