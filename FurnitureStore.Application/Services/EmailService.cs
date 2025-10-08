using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpOptions, ILogger<EmailService> logger)
        {
            _smtpSettings = smtpOptions.Value; // Get the actual settings object
            _logger = logger;
        }
        public async Task SendEmailAsync(EmailMessageDto emailMessage)
        {
            try
            {
                using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSSL
                };

                using var mailMessage = new MailMessage(_smtpSettings.From, emailMessage.To)
                {
                    Subject = emailMessage.Subject,
                    Body = emailMessage.Body,
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", emailMessage.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", emailMessage.To);
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            var subject = "Your FurnitureStore OTP Code";
            var body = $@"
                <h2>Furniture Store - Password Reset</h2>
                <p>Your OTP code is:</p>
                <h3 style='color:#2e86de'>{otpCode}</h3>
                <p>This code will expire in 5 minutes.</p>
                <br/>
                <p>If you didn't request this, please ignore this email.</p>";

            var message = new EmailMessageDto
            {
                To = toEmail,
                Subject = subject,
                Body = body
            };

            await SendEmailAsync(message);

        }
    }
}
