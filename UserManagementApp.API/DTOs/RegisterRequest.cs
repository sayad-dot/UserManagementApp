using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.API.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(1)] // FIFTH REQUIREMENT: Any non-empty password
        public string Password { get; set; } = string.Empty;
    }
}