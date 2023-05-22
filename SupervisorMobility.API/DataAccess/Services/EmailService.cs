using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.DataAccess.Services
{
    public class EmailService : IEmailService
    {

        private readonly EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public MimeMessage CreateEmailMessage(string email, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.UserName));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = $"{_emailConfig.UserName}";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message };
            return emailMessage; 
        }
        public void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.None);
                    //client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    client.Send(mailMessage);
                    client.Disconnect(true);
                    client.Dispose();
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

    }
}
