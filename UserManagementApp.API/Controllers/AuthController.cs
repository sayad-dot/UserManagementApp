using Microsoft.AspNetCore.Mvc;
using UserManagementApp.API.DTOs;
using UserManagementApp.Core.Interfaces;

namespace UserManagementApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request.Name, request.Email, request.Password);
                
                var response = new
                {
                    Message = "Registration successful. Please check your email for verification instructions.",
                    UserId = user.Id
                };

                return Ok(response);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // This will catch the unique constraint violation from PostgreSQL
                return BadRequest(new { Message = "Email already exists." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Registration failed." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.LoginAsync(request.Email, request.Password);
            if (user == null)
                return Unauthorized(new { Message = "Invalid email or password" });

            var token = _authService.GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Status,
                    user.LastLoginTime
                }
            });
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var success = await _authService.VerifyEmailAsync(token);
            if (success)
                return Ok("Email verified successfully. You can now login.");
            else
                return BadRequest("Invalid verification token.");
        }
    }
}