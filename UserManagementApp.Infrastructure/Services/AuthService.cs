using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserManagementApp.Core.Entities;
using UserManagementApp.Core.Enums;
using UserManagementApp.Core.Interfaces;
using UserManagementApp.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace UserManagementApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
        }

        public async Task<User> RegisterAsync(string name, string email, string password)
{
    // Password hashing
    var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

    var user = new User
    {
        Name = name,
        Email = email,
        PasswordHash = passwordHash,
        Status = UserStatus.Unverified,
        RegistrationTime = DateTime.UtcNow, // Use UTC
        EmailVerificationToken = Guid.NewGuid().ToString()
    };

    var createdUser = await _userRepository.CreateAsync(user);

    // Send verification email asynchronously
    _ = Task.Run(() => _emailService.SendVerificationEmailAsync(createdUser.Email, createdUser.EmailVerificationToken!));

    return createdUser;
}

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            // Users with unverified email can login (as per requirement)
            if (user.Status == UserStatus.Blocked) return null;

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                await _userRepository.UpdateLastLoginAsync(user.Id);
                return user;
            }

            return null;
        }

 public string GenerateJwtToken(User user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "default-secret-key-at-least-32-chars-long");
    var issuer = _configuration["Jwt:Issuer"] ?? "UserManagementApp";
    var audience = _configuration["Jwt:Audience"] ?? "UserManagementAppUsers";

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
            if (user == null) return false;

            // Only change status if user is not blocked
            if (user.Status != UserStatus.Blocked)
            {
                user.Status = UserStatus.Active;
            }
            
            user.EmailVerificationToken = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateUserActivityAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastActivityTime = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}