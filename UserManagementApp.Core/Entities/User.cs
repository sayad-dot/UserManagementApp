using System;
using System.ComponentModel.DataAnnotations;
using UserManagementApp.Core.Enums;

namespace UserManagementApp.Core.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime? LastLoginTime { get; set; }
        
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
        
        public UserStatus Status { get; set; } = UserStatus.Unverified;
        
        public DateTime? LastActivityTime { get; set; }
        
        public string? EmailVerificationToken { get; set; }
    }
}