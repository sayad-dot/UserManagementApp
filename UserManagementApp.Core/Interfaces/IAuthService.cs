using UserManagementApp.Core.Entities;

namespace UserManagementApp.Core.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string name, string email, string password);
        Task<User?> LoginAsync(string email, string password);
        string GenerateJwtToken(User user);
        Task<bool> VerifyEmailAsync(string token);
        Task UpdateUserActivityAsync(int userId);
    }
}