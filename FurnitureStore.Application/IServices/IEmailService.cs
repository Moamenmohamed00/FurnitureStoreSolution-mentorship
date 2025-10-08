using FurnitureStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDto emailMessage);
        Task SendOtpEmailAsync(string toEmail, string otpCode);
    }
}
