using MailKit.Net.Smtp;
using MimeKit;
using UserManagementApp.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace UserManagementApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationToken)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("User Management App", _configuration["Email:From"] ?? "noreply@usermanagement.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Verify Your Email Address";

                var verificationUrl = $"{_configuration["App:BaseUrl"]}/api/auth/verify-email?token={verificationToken}";

                message.Body = new TextPart("html")
                {
                    Text = $@"
                        <h2>Welcome to User Management App</h2>
                        <p>Please verify your email address by clicking the link below:</p>
                        <a href='{verificationUrl}'>Verify Email</a>
                        <p>If you didn't create an account, please ignore this email.</p>
                    "
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["Email:SmtpServer"] ?? "smtp.gmail.com",
                    int.Parse(_configuration["Email:Port"] ?? "587"),
                    false
                );

                await client.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log email sending errors but don't throw to user
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }
    }
}