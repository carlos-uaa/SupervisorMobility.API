using MimeKit;

namespace SupervisorMobility.API.DataAccess.Services
{
    public interface IEmailService
    {
        public MimeMessage CreateEmailMessage(string email, string message);

        public void Send( MimeMessage mailMessage);

    }
}
