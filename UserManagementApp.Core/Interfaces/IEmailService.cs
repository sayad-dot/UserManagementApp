namespace UserManagementApp.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string verificationToken);
    }
}