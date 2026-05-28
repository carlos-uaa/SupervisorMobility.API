using SupervisorMobility.API.Models.Email;

namespace SupervisorMobility.API.Services.EmailService
{
    public interface IEmailServices
    {
        public Task<bool> SendEmailAsync(EmailQueue queued);
    }
}
