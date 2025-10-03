using UserManagementApp.Core.Enums;

namespace UserManagementApp.API.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LastLoginTime { get; set; }
        public DateTime RegistrationTime { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? LastActivityTime { get; set; }
        public bool IsSelected { get; set; } // For checkbox selection
    }
}