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


        // public MimeMessage CreateEmailMessage(string email, string message);

        /// <summary>
        /// Sends emails using an existing SMTP client connection (for connection pooling)
        /// </summary>
        // public Task<bool> SendEmail(List<Users> contacts, MimeMessage message, Notification template);
        // public Task<bool> SendEmail(List<Users> contacts, MimeMessage message, Notification template, SmtpClient existingClient);
        // public Task<bool> SendEmailToSupplierGroup(int SupplierID, NotificacionWMessageDto notification);
        // public Task<MimeMessage> CreateEmailBodyByNotificationType(Notification notification);
        // public Task<string> CreateMessageToBody(NotificationsDTO notification);
        // public Task<string> CreateMessageToBody(NotificacionWMessageDtoo notification);
        
        /// <summary>
        /// Creates and connects a new SMTP client
        /// </summary>
        // public Task<SmtpClient> CreateConnectedSmtpClientAsync();
        
        /// <summary>
        /// Safely disconnects and disposes an SMTP client
        /// </summary>
        // public Task DisconnectAndDisposeClientAsync(SmtpClient client);

        /// <summary>
        /// Sends a single email using an existing connected SMTP client
        /// </summary>
        // public Task<bool> SendSingleEmailAsync(SmtpClient client, string toEmail, string subject, string body, string? referenceEntity = null, int? referenceEntityId = null, int? sentByUserId = null);
        // public Task<string> CreateMessageToBodyNoti(NotificacionWMessageDto notification);
        
    }
}
