using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Org.BouncyCastle.Asn1.X509;
using Quartz.Util;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Services.HRIServices;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.Email;



namespace SupervisorMobility.API.Services.EmailService
{
    public class EmailServices : IEmailServices
    {
        // private readonly DatabaseContext context;
        private readonly IEmailDeliveryResultService _emailDeliveryResultService;
        private readonly IEmailQueueService _emailQueueService;
        private readonly EmailConfiguration _emailConfig;
        private readonly AppSettingsConfiguration _appSettings;
        private readonly IHRIServices _hriService;

        public EmailServices(
            // DatabaseContext context, 
            IEmailDeliveryResultService emailDeliveryResultService,
            IEmailQueueService emailQueueService,
            EmailConfiguration emailConfig,
            AppSettingsConfiguration appSettings,
            IHRIServices hriService
            )
        {
            // this.context = context;
            this._emailDeliveryResultService = emailDeliveryResultService;
            this._emailQueueService = emailQueueService;
            this._emailConfig = emailConfig;
            this._appSettings = appSettings;
            this._hriService = hriService;
        }

        /// <summary>
        /// Creates and connects a new SMTP client
        /// </summary>
        public async Task<SmtpClient> CreateConnectedSmtpClientAsync()
        {
            var client = new SmtpClient();
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Timeout = 20000; // 20 seconds timeout

            try
            {
                using var cts = new CancellationTokenSource(20000); // 60 seconds for connection
                if (this._appSettings.production)
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto, cts.Token);
                }
                else
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.SslOnConnect, cts.Token);
                    await client.AuthenticateAsync(this._emailConfig.UserName, this._emailConfig.Password, cts.Token);
                }
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SMTP server: {ex.Message}");
                client.Dispose();
                return null;
            }
        }

        /// <summary>
        /// Safely disconnects and disposes an SMTP client
        /// </summary>
        public async Task DisconnectAndDisposeClientAsync(SmtpClient client)
        {
            if (client == null) return;

            try
            {
                if (client.IsConnected)
                {
                    // Use false to close connection immediately without waiting for server response
                    await client.DisconnectAsync(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disconnecting SMTP client: {ex.Message}");
            }
            finally
            {
                client.Dispose();
            }
        }


        public async Task<bool> SendEmailAsync(EmailQueue queued)
        {
            var result = false;

            // Create notification object
            var _noti = new Notification
            {
                MadeBy = queued.MadeBy != null ? queued.MadeBy.Name : string.Empty,
                //public int TargetRelation { get; set; }
                NotificationType = queued.NotificationType,
                NotificationText = string.Empty,
                User = queued.Staff,
                IsAccepted = false,
                IsActive = false,
                EntryDate = queued.EntryDate
            };


            // Validate Contant
            var contact = _noti.User;
            if (contact == null || contact.Email.IsNullOrWhiteSpace())
            {
                Console.WriteLine("Invalid or missing email address for contact.");
                return false;
            }


            // Create Email Body
            var msg = await this.CreateMessageToBody(_noti, queued.TargetRelationID, queued.TargetRelationAux);
            _noti.NotificationText = msg;
            var message = await this.CreateEmailBodyByNotificationType(_noti);


            // Create a new client and use the overload
            var client = await CreateConnectedSmtpClientAsync();
            if (client == null) return false;

            try
            {
                if (client == null || !client.IsConnected)
                {
                    Console.WriteLine("SMTP client is not connected");
                    return false;
                }

                // Control Variables
                bool error = false;

                var address = contact.Email ?? string.Empty;
                var email = new MailboxAddress("", address);

                // Crear DTO para el registro del resultado del correo
                var messageId = Guid.NewGuid().ToString();
                var emailBodyText = message.Body != null ? message.Body.ToString() : string.Empty;
                var emailResultDto = new CreateEmailDeliveryResultDto
                {
                    ToEmail = email.Address,
                    FromEmail = _emailConfig.UserName,
                    Subject = "DigitalPIR@no-reply.com",
                    MessageBody = emailBodyText,
                    IsDelivered = false,
                    DeliveryStatus = "Pending",
                    SmtpServer = _emailConfig.SmtpServer,
                    Port = _emailConfig.Port,
                    EmailType = "Notification",
                    SentByUserID = contact.UserId > 0 ? contact.UserId : (int?)null,
                    ReferenceEntity = _noti.NotificationType,
                    // ReferenceEntityID = _noti.TargetRelation,
                    MessageID = messageId
                };


                try
                {
                    // Check if connection is still alive before sending
                    if (!client.IsConnected)
                    {
                        try
                        {
                            using var cts = new CancellationTokenSource(20000); // 20 second timeout for reconnection
                            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.None, cts.Token);
                            client.Timeout = 20000; // 20 seconds timeout
                        }
                        catch (Exception reconnectEx)
                        {
                            throw new Exception($"Failed to reconnect to SMTP server: {reconnectEx.Message}", reconnectEx);
                        }
                    }

                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Compas", _emailConfig.UserName));
                    if (this._appSettings.production)
                    {
                        emailMessage.To.Add(email);
                        if (queued.CCPEmails!=null)
                        {
                            var ccpEmails = queued.CCPEmails.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var ccpEmail in ccpEmails)
                            {
                                emailMessage.Cc.Add(new MailboxAddress("", ccpEmail.Trim()));
                            }
                        }
                        

                    }
                    else
                    {
                        // In environment development, send to a test email
                        emailMessage.To.Add(new MailboxAddress("Desarrollo DigitalPIR", "gmartinez@gruposinco.com.mx"));
                        emailMessage.Cc.Add(new MailboxAddress("Daniel Mares", "dmares@gruposinco.com.mx"));

                        if (queued.CCPEmails != null)
                        {
                            var ccpEmails = queued.CCPEmails.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var ccpEmail in ccpEmails)
                            {
                                emailMessage.Cc.Add(new MailboxAddress("", ccpEmail.Trim()));
                            }
                        }
                    }

                    if (this._appSettings.production)
                    {
                        emailMessage.Cc.Add(new MailboxAddress("Eric Montanez Valadez", "eric.montanezvaladez@compas-mx.com"));
                        emailMessage.Cc.Add(new MailboxAddress("Alias", "cp@gruposinco.com.mx"));
                    }
                    emailMessage.Sender = MailboxAddress.Parse(_emailConfig.UserName);
                    emailMessage.Subject = getSubject(_noti.NotificationType);
                    emailMessage.Body = message.Body;
                    emailMessage.MessageId = messageId;

                    var response = await client.SendAsync(emailMessage);

                    // Registrar como enviado exitosamente
                    result = emailResultDto.IsDelivered = true;
                    emailResultDto.DeliveryStatus = response;

                    await _emailDeliveryResultService.SaveEmailResultAsync(emailResultDto);
                }
                catch (Exception ex)
                {
                    error = true;
                    result = false;
                    Console.WriteLine($"Error sending email to {contact.Email}: {ex.Message}");

                    // Registrar como fallido
                    emailResultDto.IsDelivered = false;
                    emailResultDto.DeliveryStatus = "Failed";
                    emailResultDto.ErrorMessage = ex.Message + ex.InnerException;
                    emailResultDto.ErrorDetails = ex.StackTrace;

                    try
                    {
                        await _emailDeliveryResultService.SaveEmailResultAsync(emailResultDto);
                    }
                    catch (Exception saveEx)
                    {
                        Console.WriteLine($"Error saving email delivery result: {saveEx.Message}");
                    }

                }

            }
            finally
            {
                await DisconnectAndDisposeClientAsync(client);
            }

            return result;
        }


        public async Task<string> CreateMessageToBody(Notification _noti, int? targetRelationId = null, string? targetRelationAux = null)
        {
            string notiMessage = "";
            string target = "(Control Number Not Found)";

            switch (_noti.NotificationType)
            {
                case "NotGood": notiMessage = "A new not good part was reported to you, you can check more in the DigitalPIR system" + getPrivacyNotice(); break;
                case "PIR":
                    // PIR pirRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
                    // if (pirRelation != null && !pirRelation.ControlNumber.IsNullOrEmpty())
                    // {
                    //     target = $"({pirRelation.ControlNumber})";
                    // }
                    notiMessage = $"A new PIR {target}, was created and related to you, you can check more in the DigitalPIR system" + getPrivacyNotice();
                    break;

                case "RevisionError":
                    // PIR RevisionErrorrelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
                    // if (RevisionErrorrelation != null && !RevisionErrorrelation.ControlNumber.IsNullOrEmpty())
                    // {
                    //     target = $"({RevisionErrorrelation.ControlNumber})";
                    // }
                    notiMessage = $"While on revision an error was found in a PIR {target} which is related to you: {_noti.NotificationText}, you can check more in the DigitalPIR system" + getPrivacyNotice();
                    break;

                case "Revisiones NG detectadas":
                    if(targetRelationId == null)
                    {
                        notiMessage = $"While on revision, a HRI related to you has detected not good revision, you can check more in the Supervisor Mobility System. " + getPrivacyNotice();
                        break;
                    }
                    var hri = await _hriService.GetHRIById((int)targetRelationId);
                    if(hri == null || hri.Success == false || hri.Data == null)
                    {
                        notiMessage = $"While on revision, a HRI related to you has detected not good revision, you can check more in the Supervisor Mobility System. " + getPrivacyNotice();
                        break;
                    }
                    notiMessage = $"Se han detectado revisiones en NG en el HRI: {hri?.Data.ControlNumber}. \n\n {targetRelationAux}" + getPrivacyNotice();
                    break;

                default: notiMessage = $"You have a new notification in DigitalPIR system from the user {_noti.MadeBy}" + getPrivacyNotice(); break;
            }

            return notiMessage;
        }

        private string getSubject(string type)
        {
            switch (type)
            {
                case "NotGood":
                    return string.Empty;
                case "PIR":
                    return string.Empty;
                case "RevisionError":
                    return string.Empty;
                case "Revisiones NG detectadas":
                    return "Revisiones NG detectadas";
                default: return string.Empty;
            }
        }
        private string getPrivacyNotice()
        {
            return $"\nPlease log in into: {_appSettings.SupervisorMobilityUrl}\r\nIf you are external from COMPAS, log in into: https://ec2amaz-s74pd8m.compas-mexico.com:10300/DigitalPIR\r\n\r\nIf you don’t remember your user and/or password please contact to: Eric.MontanezValadez@compas-mx.com \r\n\r\n" +
                "AVISO DE PRIVACIDAD: \r\nLos datos personales de los cuales Cooperation Manufacturing Plant Aguascalientes, S.A.P.I. de C.V. es responsable son procesados y manejados de conformidad " +
                "con las disposiciones y principios aplicables de la Ley Federal de Protección de Datos Personales en Posesión de Particulares y su Reglamento. COMPAS tomará todas las medidas necesarias " +
                "para proteger la información personal en los términos de dicha Ley. \r\nPara mayor información acerca del tratamiento y de los derechos que puede hacer valer, usted puede acceder al Aviso " +
                "de Privacidad completo solicitándolo al correo electrónico: Data.Privacy@compas-mx.com  \r\n \r\nPRIVACY NOTICE: \r\nThe personal data of which Cooperation Manufacturing Plant Aguascalientes, " +
                "S.A.P.I. de C.V. is responsible for, is processed and handled in accordance with the provisions and principles of the Data Protection Act and its Regulations. COMPAS will take all necessary " +
                "measures to protect personal information in the terms established by the Act. \r\nFor more information about the treatment and rights that you may exercise, you can request our complete " +
                "Privacy Notice to the email: Data.Privacy@compas-mx.com ";
        }
        public async Task<MimeMessage> CreateEmailBodyByNotificationType(Notification notification)
        {
            var message = new MimeMessage();
            message.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = $"Notification By: {notification.MadeBy} \n\n {notification.NotificationText}" };
            return message;
        }

    }
}