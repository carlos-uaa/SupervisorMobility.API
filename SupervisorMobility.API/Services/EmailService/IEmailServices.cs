// using SupervisorMobility.API.DTOs;
using SupervisorMobility.API.Models;
using SupervisorMobility.API.Models.Email;
using MailKit.Net.Smtp;
using MimeKit;

namespace SupervisorMobility.API.Services.EmailService
{
    public interface IEmailServices
    {
        public Task<bool> SendEmailAsync(EmailQueue queued);
        
    }
}
