using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApp.API.DTOs;
using UserManagementApp.Core.Enums;
using UserManagementApp.Core.Interfaces;

namespace UserManagementApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            
            var userResponses = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                LastLoginTime = u.LastLoginTime,
                RegistrationTime = u.RegistrationTime,
                Status = u.Status,
                LastActivityTime = u.LastActivityTime,
                IsSelected = false
            });

            return Ok(userResponses);
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockUsers([FromBody] int[] userIds)
        {
            foreach (var userId in userIds)
            {
                await _userRepository.UpdateStatusAsync(userId, UserStatus.Blocked);
            }

            return Ok(new { Message = "Users blocked successfully" });
        }

        [HttpPost("unblock")]
        public async Task<IActionResult> UnblockUsers([FromBody] int[] userIds)
        {
            foreach (var userId in userIds)
            {
                await _userRepository.UpdateStatusAsync(userId, UserStatus.Active);
            }

            return Ok(new { Message = "Users unblocked successfully" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUsers([FromBody] int[] userIds)
        {
            foreach (var userId in userIds)
            {
                await _userRepository.DeleteAsync(userId);
            }

            return Ok(new { Message = "Users deleted successfully" });
        }

        [HttpDelete("unverified")]
        public async Task<IActionResult> DeleteUnverifiedUsers()
        {
            await _userRepository.DeleteUnverifiedUsersAsync();
            return Ok(new { Message = "Unverified users deleted successfully" });
        }
    }
}