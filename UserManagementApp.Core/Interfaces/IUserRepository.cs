using UserManagementApp.Core.Entities;
using UserManagementApp.Core.Enums;

namespace UserManagementApp.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task DeleteUnverifiedUsersAsync();
        Task<bool> EmailExistsAsync(string email);
        Task UpdateStatusAsync(int userId, UserStatus status);
        Task UpdateLastLoginAsync(int userId);
    }
}