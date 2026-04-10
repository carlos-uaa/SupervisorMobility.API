using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using SupervisorMobility.API.Models.Email;






namespace SupervisorMobility.API.Services.EmailService
{
    public class EmailService : IEmailService
    {
        // private readonly DatabaseContext context;
        // private readonly EmailConfiguration _emailConfig;
        // private readonly AppSettings _appSettings;
        private readonly IEmailDeliveryResultService _emailDeliveryResultService;
        private readonly IEmailQueueService _emailQueueService;

        public EmailService(
            // DatabaseContext context, 
            // EmailConfiguration emailConfig, 
            // IOptions<AppSettings> appSettings, 
            IEmailDeliveryResultService emailDeliveryResultService,
            IEmailQueueService emailQueueService
            )
        {
            // this._emailConfig = emailConfig;
            // this.context = context;
            // this._appSettings = appSettings.Value;
            this._emailDeliveryResultService = emailDeliveryResultService;
            this._emailQueueService = emailQueueService;
            
        }

        // public MimeMessage CreateEmailMessage(string email, string message)
        // {
        //     var emailMessage = new MimeMessage();
        //     emailMessage.From.Add(new MailboxAddress("Compas", _emailConfig.UserName));
        //     emailMessage.To.Add(new MailboxAddress("", email));
        //     emailMessage.Subject = $"{_emailConfig.UserName}";
        //     emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message };
        //     return emailMessage;
        // }


        /// <summary>
        /// Sends emails using an existing SMTP client connection (for connection pooling)
        /// </summary>
        // public async Task<bool> SendEmail(List<Users> contacts, MimeMessage message, Notification template)
        // {
        //     // Create a new client and use the overload
        //     var client = await CreateConnectedSmtpClientAsync();
        //     if (client == null) return false;

        //     try
        //     {
        //         return await SendEmail(contacts, message, template, client);
        //     }
        //     finally
        //     {
        //         await DisconnectAndDisposeClientAsync(client);
        //     }
        // }

        // public async Task<bool> SendEmail(List<Users> contacts, MimeMessage message, Notification template, SmtpClient existingClient)
        // {
        //     if (existingClient == null || !existingClient.IsConnected)
        //     {
        //         Console.WriteLine("SMTP client is not connected");
        //         return false;
        //     }

        //     // Control Variables
        //     bool error = false;

        //     foreach (var contact in contacts)
        //     {
        //         var email = new MailboxAddress("", contact.Email);

        //         // Crear DTO para el registro del resultado del correo
        //         var messageId = Guid.NewGuid().ToString();
        //         var emailBodyText = message.Body.ToString();
        //         var emailResultDto = new CreateEmailDeliveryResultDto
        //         {
        //             ToEmail = email.Address,
        //             FromEmail = _emailConfig.UserName,
        //             Subject = "DigitalPIR@no-reply.com",
        //             MessageBody = emailBodyText,
        //             IsDelivered = false,
        //             DeliveryStatus = "Pending",
        //             SmtpServer = _emailConfig.SmtpServer,
        //             Port = _emailConfig.Port,
        //             EmailType = "Notification",
        //             SentByUserID = contact.UserID > 0 ? contact.UserID : (int?)null,
        //             ReferenceEntity = template.NotificationType,
        //             ReferenceEntityID = template.TargetRelation,
        //             MessageID = messageId
        //         };

        //         try
        //         {
        //             // Check if connection is still alive before sending
        //             if (!existingClient.IsConnected)
        //             {
        //                 try
        //                 {
        //                     using var cts = new CancellationTokenSource(20000); // 20 second timeout for reconnection
        //                     await existingClient.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.None, cts.Token);
        //                     existingClient.Timeout = 20000; // 20 seconds timeout
        //                 }
        //                 catch (Exception reconnectEx)
        //                 {
        //                     throw new Exception($"Failed to reconnect to SMTP server: {reconnectEx.Message}", reconnectEx);
        //                 }
        //             }

        //             var emailMessage = new MimeMessage();
        //             emailMessage.From.Add(new MailboxAddress("Compas", _emailConfig.UserName));
        //             emailMessage.To.Add(email);
        //             emailMessage.Sender = MailboxAddress.Parse(_emailConfig.UserName);
        //             emailMessage.Subject = "DigitalPIR@no-reply.com";
        //             emailMessage.Body = message.Body;
        //             emailMessage.MessageId = messageId;

        //             var response = await existingClient.SendAsync(emailMessage);

        //             // Registrar como enviado exitosamente
        //             emailResultDto.IsDelivered = true;
        //             emailResultDto.DeliveryStatus = response;
        //             await _emailDeliveryResultService.SaveEmailResultAsync(emailResultDto);
        //         }
        //         catch (Exception ex)
        //         {
        //             error = true;
        //             Console.WriteLine($"Error sending email to {contact.Email}: {ex.Message}");

        //             // Registrar como fallido
        //             emailResultDto.IsDelivered = false;
        //             emailResultDto.DeliveryStatus = "Failed";
        //             emailResultDto.ErrorMessage = ex.Message + ex.InnerException;
        //             emailResultDto.ErrorDetails = ex.StackTrace;

        //             try
        //             {
        //                 await _emailDeliveryResultService.SaveEmailResultAsync(emailResultDto);
        //             }
        //             catch (Exception saveEx)
        //             {
        //                 Console.WriteLine($"Error saving email delivery result: {saveEx.Message}");
        //             }

        //             // If connection error, try to reconnect for next iteration
        //             if (ex.Message.Contains("read operation failed") ||
        //                 ex.Message.Contains("connection") ||
        //                 ex.Message.Contains("timeout") ||
        //                 !existingClient.IsConnected)
        //             {
        //                 Console.WriteLine("Connection error detected, attempting to reconnect for next email...");
        //                 try
        //                 {
        //                     if (existingClient.IsConnected)
        //                     {
        //                         await existingClient.DisconnectAsync(false);
        //                     }
        //                     using var cts = new CancellationTokenSource(20000);
        //                     await existingClient.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.None, cts.Token);
        //                     existingClient.Timeout = 20000;
        //                 }
        //                 catch (Exception reconnectEx)
        //                 {
        //                     Console.WriteLine($"Failed to reconnect after error: {reconnectEx.Message}");
        //                     // Connection is broken, but we continue the loop to record other failures
        //                 }
        //             }
        //         }

        //         await Task.Delay(TimeSpan.FromSeconds(5));
        //     }

        //     return !error;
        // }

        // public async Task<bool> SendEmailToSupplierGroup(int SupplierID, NotificacionWMessageDto notification)
        // {
        //     // Search Users
        //     Supplier supplier = await context.Suppliers.Include(p => p.Manager).Include(p => p.Director).Include(p => p.CostumeService).FirstOrDefaultAsync(p => p.SupplierID == SupplierID);
        //     List<Users> contacts = new List<Users>();

        //     if (supplier != null)
        //     {
        //         if (supplier.Manager != null)
        //         {
        //             if (!string.IsNullOrEmpty(supplier.Manager.Email))
        //             {
        //                 var usr = new Users();
        //                 usr.Email = supplier.Manager.Email;
        //                 contacts.Add(usr);
        //             }
        //         }

        //         if (supplier.Director != null)
        //         {
        //             if (!string.IsNullOrEmpty(supplier.Director.Email))
        //             {
        //                 var usr = new Users();
        //                 usr.Email = supplier.Director.Email;
        //                 contacts.Add(usr);
        //             }
        //         }

        //         if (supplier.CostumeService != null)
        //         {
        //             if (!string.IsNullOrEmpty(supplier.CostumeService.Email))
        //             {
        //                 var usr = new Users();
        //                 usr.Email = supplier.CostumeService.Email;
        //                 contacts.Add(usr);
        //             }
        //         }
        //     }

        //     // Get Window Persons
        //     var sup = await context.Suppliers.Include(p => p.AdditionalWPsToBeNotified).FirstOrDefaultAsync(p => p.SupplierID == SupplierID);
        //     if (sup != null && sup.AdditionalWPsToBeNotified != null)
        //     {
        //         var windowpersons = sup.AdditionalWPsToBeNotified.ToList();
        //         if (windowpersons.Count() >= 1)
        //         {
        //             foreach (var wp in windowpersons)
        //             {
        //                 if (!string.IsNullOrEmpty(wp.Email))
        //                 {
        //                     var usr = new Users();
        //                     usr.Email = wp.Email;
        //                     contacts.Add(usr);
        //                 }
        //             }
        //         }
        //     }

        //     if (contacts.Count <= 0) return true;


        //     var madeby = await context.Users.Include(u => u.Superiror).FirstOrDefaultAsync(u => u.UserID == notification.MadeBy);
        //     if (madeby == null) return false;

        //     var _noti = new Notification
        //     {
        //         MadeBy = madeby.DisplayName,
        //         TargetRelation = notification.TargetRelation,
        //         NotificationType = notification.NotificationType,
        //         Message = notification.Message,
        //         Staff = null,
        //         IsAccepted = notification.IsAccepted,
        //         EntryDate = notification.EntryDate,
        //     };

        //     foreach (var user in contacts)
        //     {
        //         _noti.Staff = user;
        //         await _emailQueueService.CreateEmailQueueAsync(_noti, madeby);
        //     }

        //     return true;
        // }

        /// <summary>
        /// Creates and connects a new SMTP client
        /// </summary>
        // public async Task<SmtpClient> CreateConnectedSmtpClientAsync()
        // {
        //     var client = new SmtpClient();
        //     client.CheckCertificateRevocation = false;
        //     client.ServerCertificateValidationCallback = (s, c, h, e) => true;
        //     client.Timeout = 20000; // 20 seconds timeout

        //     try
        //     {
        //         using var cts = new CancellationTokenSource(20000); // 60 seconds for connection
        //         if (this._appSettings.Enviroment.production)
        //         {
        //             await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto, cts.Token);
        //         }
        //         else
        //         {
        //             await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.SslOnConnect, cts.Token);
        //             await client.AuthenticateAsync(this._emailConfig.UserName, this._emailConfig.Password, cts.Token);
        //         }
        //         return client;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error connecting to SMTP server: {ex.Message}");
        //         client.Dispose();
        //         return null;
        //     }
        // }

        /// <summary>
        /// Safely disconnects and disposes an SMTP client
        /// </summary>
        // public async Task DisconnectAndDisposeClientAsync(SmtpClient client)
        // {
        //     if (client == null) return;

        //     try
        //     {
        //         if (client.IsConnected)
        //         {
        //             // Use false to close connection immediately without waiting for server response
        //             await client.DisconnectAsync(false);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error disconnecting SMTP client: {ex.Message}");
        //     }
        //     finally
        //     {
        //         client.Dispose();
        //     }
        // }
        /// <summary>
        /// Sends a single email using an existing connected SMTP client
        /// </summary>
        // public async Task<bool> SendSingleEmailAsync(SmtpClient client, string toEmail, string subject, string body, string? referenceEntity = null, int? referenceEntityId = null, int? sentByUserId = null)
        // {
        //     if (client == null || !client.IsConnected)
        //     {
        //         Console.WriteLine("SMTP client is not connected");
        //         return false;
        //     }

        //     var messageId = Guid.NewGuid().ToString();
        //     var emailResultDto = new CreateEmailDeliveryResultDto
        //     {
        //         ToEmail = toEmail,
        //         FromEmail = _emailConfig.UserName,
        //         Subject = subject,
        //         MessageBody = body,
        //         IsDelivered = false,
        //         DeliveryStatus = "Pending",
        //         SmtpServer = _emailConfig.SmtpServer,
        //         Port = _emailConfig.Port,
        //         EmailType = "Notification",
        //         SentByUserID = sentByUserId,
        //         ReferenceEntity = referenceEntity,
        //         ReferenceEntityID = referenceEntityId,
        //         MessageID = messageId
        //     };

        //     try
        //     {
        //         var emailMessage = new MimeMessage();
        //         emailMessage.From.Add(new MailboxAddress("Compas", _emailConfig.UserName));
        //         emailMessage.To.Add(new MailboxAddress("", toEmail));
        //         emailMessage.Subject = subject;
        //         emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = body };
        //         emailMessage.MessageId = messageId;

        //         var response = await client.SendAsync(emailMessage);

        //         emailResultDto.IsDelivered = true;
        //         emailResultDto.DeliveryStatus = response;
        //         await _emailDeliveryResultService.SaveEmailResultAsync(emailResultDto);
        //         return true;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error sending email to {toEmail}: {ex.Message}");

        //         emailResultDto.IsDelivered = false;
        //         emailResultDto.DeliveryStatus = "Failed";
        //         emailResultDto.ErrorMessage = ex.Message;
        //         emailResultDto.ErrorDetails = ex.StackTrace;

        //         try
        //         {
        //             await _emailDeliveryResultService.SaveEmailResultAsync(emailResultDto);
        //         }
        //         catch (Exception saveEx)
        //         {
        //             Console.WriteLine($"Error saving email delivery result: {saveEx.Message}");
        //         }
        //         return false;
        //     }
        // }


        // private bool ValidateEmail(string? email)
        // {
        //     if (string.IsNullOrEmpty(email))
        //     {
        //         return false;
        //     }
        //     var trimmedEmail = email.Trim();

        //     if (trimmedEmail.EndsWith("."))
        //     {
        //         return false;
        //     }
        //     try
        //     {
        //         var addr = new System.Net.Mail.MailAddress(email);
        //         return addr.Address == trimmedEmail;
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        // }

        // private string getPrivacyNotice()
        // {
        //     return $"\nPlease log in into: {_appSettings.DigitalPIRUrl}\r\nIf you are external from COMPAS, log in into: https://ec2amaz-s74pd8m.compas-mexico.com:10300/DigitalPIR\r\n\r\nIf you don’t remember your user and/or password please contact to: Eric.MontanezValadez@compas-mx.com \r\n\r\n" +
        //         "AVISO DE PRIVACIDAD: \r\nLos datos personales de los cuales Cooperation Manufacturing Plant Aguascalientes, S.A.P.I. de C.V. es responsable son procesados y manejados de conformidad " +
        //         "con las disposiciones y principios aplicables de la Ley Federal de Protección de Datos Personales en Posesión de Particulares y su Reglamento. COMPAS tomará todas las medidas necesarias " +
        //         "para proteger la información personal en los términos de dicha Ley. \r\nPara mayor información acerca del tratamiento y de los derechos que puede hacer valer, usted puede acceder al Aviso " +
        //         "de Privacidad completo solicitándolo al correo electrónico: Data.Privacy@compas-mx.com  \r\n \r\nPRIVACY NOTICE: \r\nThe personal data of which Cooperation Manufacturing Plant Aguascalientes, " +
        //         "S.A.P.I. de C.V. is responsible for, is processed and handled in accordance with the provisions and principles of the Data Protection Act and its Regulations. COMPAS will take all necessary " +
        //         "measures to protect personal information in the terms established by the Act. \r\nFor more information about the treatment and rights that you may exercise, you can request our complete " +
        //         "Privacy Notice to the email: Data.Privacy@compas-mx.com ";
        // }
        // public async Task<MimeMessage> CreateEmailBodyByNotificationType(Notification notification)
        // {
        //     var message = new MimeMessage();
        //     message.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = $"Notification By: {notification.MadeBy} \n\n {notification.Message}" };
        //     return message;
        // }


        // private async Task<string> CreateMessageToBody(Notification notification)
        // {

        //     return await CreateMessageToBody(new NotificacionWMessageDtoo
        //     {
        //         MadeBy = notification.MadeBy,
        //         TargetRelation = notification.TargetRelation,
        //         NotificationType = notification.NotificationType,
        //         Message = string.Empty,
        //         StaffId = notification.Staff.UserID,
        //         IsAccepted = notification.IsAccepted,
        //         EntryDate = notification.EntryDate
        //     });
        // }
        // public async Task<string> CreateMessageToBody(NotificationsDTO notification)
        // {
        //     var madeby = await context.Users.Include(u => u.Superiror).FirstOrDefaultAsync(u => u.UserID == notification.MadeBy);
        //     if (madeby == null) return "";
        //     return await CreateMessageToBody(new NotificacionWMessageDtoo
        //     {
        //         MadeBy = madeby.DisplayName,
        //         TargetRelation = notification.TargetRelation,
        //         NotificationType = notification.NotificationType,
        //         Message = string.Empty,
        //         StaffId = notification.StaffId,
        //         IsAccepted = notification.IsAccepted,
        //         EntryDate = notification.EntryDate
        //     });
        // }

        // public async Task<string> CreateMessageToBodyNoti(NotificacionWMessageDto notification)
        // {
        //     string message = "";
        //     try
        //     {
        //         var pirRelation = await context.PIR.Include(p=>p.Supplier).FirstOrDefaultAsync(t => t.PIRID == notification.TargetRelation);
        //         var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == notification.MadeBy);

        //         switch (notification.NotificationType)
        //         {
        //             case "NotGood":
        //                 break;

        //             case "PIR":
        //                 message = $"A new PIR was created with the control number {pirRelation.ControlNumber} Please make a Revision";
        //                 break;

        //             case "RevisionError":
        //                 message = $"An error was detected in the pir {pirRelation.ControlNumber}\nDescription: {notification.Message}";
        //                 break;

        //             case "RejectedPIR":
        //                 message = $"The PIR {pirRelation.ControlNumber} was Rejected by {user.DisplayName}\nReason: {notification.Message}";
        //                 break;

        //             case "LimitDate":
        //                 message = notification.Message;
        //                 break;

        //             case "ForRevision":
        //                 message = $"The PIR {pirRelation.ControlNumber}\nReason: {notification.Message}";
        //                 break;

        //             case "RevisionSupplierStaff":
        //                 message = notification.Message;
        //                 break;

        //             case "MotherCOmpanyAware":
        //                 message = $"The Mother Company received your PIR revision about the PIR {pirRelation.ControlNumber}\nReason: {notification.Message}";
        //                 break;

        //             case "TicketAccepted":
        //                 message = $"The NG Ticket {pirRelation.ControlNumber} was accepted you need to move the NG part to quarantine area.";
        //                 break;

        //             case "TicketRejection":
        //                 message = $"The NG Ticket {pirRelation.ControlNumber}  was rejected by {user.DisplayName}\nReason: @notification.Message";
        //                 break;

        //             case "SortReportCreated":
        //                 message = $"A new Sorting Report {pirRelation.ControlNumber} was created by {user.DisplayName}, it was related to you, check it out on Sorting Reports";
        //                 break;

        //             case "CDCPDoc":
        //                 message = $"The respective documentation was attached to the PIR, you can this documentation in the PIR {pirRelation.ControlNumber}";
        //                 break;

        //             case "CDCPDocRevition":
        //                 message = $"Reason: {notification.Message}";
        //                 break;

        //             case "PSDoc":
        //                 message = $"The respective documentation was attached to the PIR, you can this documentation in the PIR {pirRelation.ControlNumber}";
        //                 break;

        //             case "PSDocRevition":
        //                 message = $"Reason: {notification.Message}";
        //                 break;

        //             case "PartsSent":
        //                 message = $"Reason: {notification.Message}";
        //                 break;

        //             case "StaffChanged":
        //                 message = $"You were assigned to supply as the new Staff in the PIR {pirRelation.ControlNumber}";
        //                 break;

        //             case "PIRInformation":
        //                 message = notification.Message;
        //                 break;

        //             case "Possible PIR Reject":
        //                 message = $"The PIR {pirRelation.ControlNumber} was rejected by the Staff {user.DisplayName}\nHis reason was: {notification.Message}\nPlease verify in the eye icon below to confirm or cancel the Rejection.";
        //                 break;

        //             case "PIRRejectedDeny":
        //                 message = $"The General {notification.MadeBy} reviewed your PIR Rejection in the PIR {pirRelation.ControlNumber} and concluded that the rejection is not valid. You must continue with the PIR flow.\nHis reason was: {notification.Message}";
        //                 break;

        //             case "PIRRejectedAccept":
        //                 message = notification.Message;
        //                 break;

        //             case "PIR Rejection Confirmed":
        //                 message = $"The General {user.DisplayName} reviewed your PIR Rejection in the PIR {pirRelation.ControlNumber} and concluded that the rejection is valid. The PIR was canceled & no more actions are needed for it.";
        //                 break;

        //             case "PIRCompletedIBL":
        //                 message = $"Reason: {notification.Message}";
        //                 break;

        //             case "Scrap Process":
        //                 message = notification.Message;
        //                 break;

        //             case "Scrap Process Started":
        //                 message = notification.Message;
        //                 break;

        //             case "PIRClosed":
        //                 message = $"Reason: {notification.Message}";
        //                 break;

        //             case "SignedPIRUploaded":
        //                 message = notification.Message;
        //                 break;

        //             case "SupplierGuide":
        //                 message = notification.Message;
        //                 break;

        //             case "PIREditStaffPQA":
        //                 message = $"The PIR {pirRelation.ControlNumber} was edited by the Staff PQA";
        //                 break;

        //             default:
        //                 message = notification.Message;
        //                 break;

                    
        //         }

        //     }
        //     catch(Exception ex)
        //     {
        //         Console.WriteLine($"Error in CreateMessageToBodyNoti: {ex.Message}");
        //         return "";
        //     }
        //     return message;

        // }


        // public async Task<string> CreateMessageToBody(NotificacionWMessageDtoo notification)
        // {
        //     var _noti = new Notification
        //     {
        //         MadeBy = notification.MadeBy,
        //         TargetRelation = notification.TargetRelation,
        //         NotificationType = notification.NotificationType,
        //         Message = notification.Message,
        //         Staff = await context.Users.FirstOrDefaultAsync(u => u.UserID == notification.StaffId),
        //         IsAccepted = notification.IsAccepted,
        //         EntryDate = notification.EntryDate
        //     };


        //     string notiMessage = "";
        //     string target = "(Control Number Not Found)";

        //     switch (_noti.NotificationType)
        //     {
        //         case "NotGood": notiMessage = "A new not good part was reported to you, you can check more in the DigitalPIR system" + getPrivacyNotice(); break;

        //         case "PIR":
        //             PIR pirRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (pirRelation != null && !pirRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({pirRelation.ControlNumber})";
        //             }
        //             notiMessage = $"A new PIR {target}, was created and related to you, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "RevisionError":
        //             PIR RevisionErrorrelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (RevisionErrorrelation != null && !RevisionErrorrelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({RevisionErrorrelation.ControlNumber})";
        //             }
        //             notiMessage = $"While on revision an error was found in a PIR {target} which is related to you: {_noti.Message}, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "RejectedPIR":
        //             PIR RejectedPIRrelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (RejectedPIRrelation != null && !RejectedPIRrelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({RejectedPIRrelation.ControlNumber})";
        //             }
        //             notiMessage = $"A PIR {target} related to you was rejected: {_noti.Message}, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "LimitDate":
        //             PIR LimitDaterelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (LimitDaterelation != null && !LimitDaterelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({LimitDaterelation.ControlNumber})";
        //             }
        //             notiMessage = $"A PIR {target} which is related to you is out of time with none movement, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "ForRevision":
        //             PIR ForRevisionRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (ForRevisionRelation != null && !ForRevisionRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({ForRevisionRelation.ControlNumber})";
        //             }
        //             notiMessage = $"You were selected to make the revision of a PIR {target}, you can check this in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "RevisionSupplierStaff":
        //             PIR ForRevisionMCRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (ForRevisionMCRelation != null && !ForRevisionMCRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({ForRevisionMCRelation.ControlNumber})";
        //             }
        //             notiMessage = $"You have a new PIR {target} to make a revision, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "MotherCOmpanyAware":
        //             PIR relation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             notiMessage = $"The Mother Company Staff is aware about the PIR {relation.ControlNumber}" + getPrivacyNotice();
        //             break;

        //         case "TicketAccepted":
        //             NGTicket ticket = await context.NGTickets.FirstOrDefaultAsync(t => t.TicketID == _noti.TargetRelation);
        //             notiMessage = $"The NG Ticket related to the part {ticket.Part} (by {ticket.MadeBy.DisplayName}) was ACCEPTED by the staff {_noti.MadeBy} on DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "TicketRejection":
        //             NGTicket ticketRej = await context.NGTickets.FirstOrDefaultAsync(t => t.TicketID == _noti.TargetRelation);
        //             notiMessage = $"The NG Ticket related to the part {ticketRej.Part} (by {ticketRej.MadeBy.DisplayName}) was REJECTED by the staff {_noti.MadeBy} on DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "SortReportCreated":
        //             SortReport report = await context.SortReports.FirstOrDefaultAsync(t => t.ReporttID == _noti.TargetRelation);
        //             notiMessage = $"There is a new Sorting Report creted with the control number {report.ControlNumber} (by {notification.MadeBy}) and it was related to you on DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "CDCPDoc":
        //             PIR CDCPDocRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (CDCPDocRelation != null && !CDCPDocRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({CDCPDocRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The respective documentation was attached to the PIR {target}, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "CDCPDocRevition":
        //             PIR CDCPDocRevitionRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (CDCPDocRevitionRelation != null && !CDCPDocRevitionRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({CDCPDocRevitionRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} CDCP documentation has been aproved and it's ready to start the parts shipment process, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PSDoc":
        //             PIR PSDocRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (PSDocRelation != null && !PSDocRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({PSDocRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The respective documentation was attached to the PIR {target}, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PSDocRevition":
        //             PIR PSDocRevitionRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (PSDocRevitionRelation != null && !PSDocRevitionRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({PSDocRevitionRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} PS documentation has been aproved and it's ready to start the parts shipment process, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PartsSent":
        //             PIR PartSentRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (PartSentRelation != null && !PartSentRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({PartSentRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} related parts have been shipped, you can check details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "StaffChanged":
        //             PIR StaffChangedRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (StaffChangedRelation != null && !StaffChangedRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({StaffChangedRelation.ControlNumber})";
        //             }
        //             notiMessage = $"You were assigned to supply as the new Staff in the PIR {target}, you can check details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PIRInformation":
        //             PIR PIRInformationRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (PIRInformationRelation != null && !PIRInformationRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({PIRInformationRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} has finished the Supplier Revision Succesfully, you can check more in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "Possible PIR Reject":
        //             PIR PossiblePIRRejectRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (PossiblePIRRejectRelation != null && !PossiblePIRRejectRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({PossiblePIRRejectRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} was rejected by the Staff {_noti.MadeBy}, Reason: {_noti.Message}. You can check the details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PIRRejectedDeny":
        //             PIR PIRRejectionDeniedRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (PIRRejectionDeniedRelation != null && !PIRRejectionDeniedRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({PIRRejectionDeniedRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} Rejection was reviewed and concluded that the rejection is not valid. You must continue with the PIR flow. Reason: {_noti.Message}. You can check the details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PIRRejectedAccept":
        //             PIR PIRRejectionAcceptedRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (PIRRejectionAcceptedRelation != null && !PIRRejectionAcceptedRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({PIRRejectionAcceptedRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} Rejection was reviewed and concluded that the rejection is valid. Reason: {_noti.Message}. You can check the details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PIR Rejection Confirmed":
        //             PIR PIRRejectionConfirmedRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (PIRRejectionConfirmedRelation != null && !PIRRejectionConfirmedRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({PIRRejectionConfirmedRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The General {_noti.MadeBy} reviewed your PIR Rejection in the PIR {target} and concluded that the rejection is valid. The PIR was canceled & no more actions are needed for it. You can check the details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PIRCompletedIBL":
        //             PIR completedRealtion = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (completedRealtion != null && !completedRealtion.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({completedRealtion.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} has been marked as completed, you can check details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "Scrap Process":
        //             PIR completedScrapRealtion = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (completedScrapRealtion != null && !completedScrapRealtion.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({completedScrapRealtion.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} has completed the Scrap process successfully, you can check details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "Scrap Process Started":
        //             PIR startedScrapRealtion = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (startedScrapRealtion != null && !startedScrapRealtion.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({startedScrapRealtion.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {target} has begun the Scrap process, you can check details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "PIRClosed":
        //             notiMessage = $"{_noti.Message}, you can check details in the DigitalPIR system" + getPrivacyNotice();
        //             break;

        //         case "SignedPIRUploaded":
        //             PIR SignedPIRRelation = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (SignedPIRRelation != null && !SignedPIRRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({SignedPIRRelation.ControlNumber})";
        //             }
        //             notiMessage = $"A signed PIR document has been uploaded for PIR {target} by {_noti.MadeBy}." + getPrivacyNotice();
        //             break;

        //         case "SupplierGuide":
        //             PIR SupplierGuideRelation = await context.PIR.Include(p=>p.Supplier).FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (SupplierGuideRelation != null && !SupplierGuideRelation.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({SupplierGuideRelation.ControlNumber})";
        //             }
        //             notiMessage = $"The supplier {SupplierGuideRelation.Supplier.SupplierName} has been uploaded the Air Guide document for PIR {target}." + getPrivacyNotice();
        //             break;

        //         case "PIREditStaffPQA":
        //             PIR pirRelationEd = await context.PIR.FirstOrDefaultAsync(t => t.PIRID == _noti.TargetRelation);
        //             if (pirRelationEd != null && !pirRelationEd.ControlNumber.IsNullOrEmpty())
        //             {
        //                 target = $"({pirRelationEd.ControlNumber})";
        //             }
        //             notiMessage = $"The PIR {pirRelationEd.ControlNumber} was edited by the Staff PQA" + getPrivacyNotice();
        //             break;

        //         default: notiMessage = $"You have a new notification in DigitalPIR system from the user {_noti.MadeBy}" + getPrivacyNotice(); break;

        //     }

        //     return notiMessage;
        // }


        // public async Task<bool> SendEmailAsync(EmailQueue queued)
        // {
        //     var result = false;


        //     // Create notification object
        //     var _noti = new Notification
        //     {
        //         MadeBy = queued.MadeBy.DisplayName,
        //         TargetRelation = queued.TargetRelation.PIRID,
        //         NotificationType = queued.NotificationType,
        //         Message = string.Empty,
        //         Staff = queued.Staff,
        //         IsAccepted = false,
        //         EntryDate = queued.EntryDate,
        //     };


        //     // Validate Contant
        //     var contact = _noti.Staff;
        //     if (contact == null || contact.Email.IsNullOrEmpty())
        //     {
        //         Console.WriteLine("Invalid or missing email address for contact.");
        //         return false;
        //     }


        //     // Create Email Body
        //     var msg = await this.CreateMessageToBody(_noti);
        //     _noti.Message = msg;
        //     var message = await this.CreateEmailBodyByNotificationType(_noti);


        //     // Create a new client and use the overload
        //     var client = await CreateConnectedSmtpClientAsync();
        //     if (client == null) return false;


        //     try
        //     {
        //         if (client == null || !client.IsConnected)
        //         {
        //             Console.WriteLine("SMTP client is not connected");
        //             return false;
        //         }

        //         // Control Variables
        //         bool error = false;

        //         var email = new MailboxAddress("", contact.Email);

        //         // Crear DTO para el registro del resultado del correo
        //         var messageId = Guid.NewGuid().ToString();
        //         var emailBodyText = message.Body.ToString();
        //         var emailResultDto = new CreateEmailDeliveryResultDto
        //         {
        //             ToEmail = email.Address,
        //             FromEmail = _emailConfig.UserName,
        //             Subject = "DigitalPIR@no-reply.com",
        //             MessageBody = emailBodyText,
        //             IsDelivered = false,
        //             DeliveryStatus = "Pending",
        //             SmtpServer = _emailConfig.SmtpServer,
        //             Port = _emailConfig.Port,
        //             EmailType = "Notification",
        //             SentByUserID = contact.UserID > 0 ? contact.UserID : (int?)null,
        //             ReferenceEntity = _noti.NotificationType,
        //             ReferenceEntityID = _noti.TargetRelation,
        //             MessageID = messageId
        //         };


        //         try
        //         {
        //             // Check if connection is still alive before sending
        //             if (!client.IsConnected)
        //             {
        //                 try
        //                 {
        //                     using var cts = new CancellationTokenSource(20000); // 20 second timeout for reconnection
        //                     await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.None, cts.Token);
        //                     client.Timeout = 20000; // 20 seconds timeout
        //                 }
        //                 catch (Exception reconnectEx)
        //                 {
        //                     throw new Exception($"Failed to reconnect to SMTP server: {reconnectEx.Message}", reconnectEx);
        //                 }
        //             }

        //             var emailMessage = new MimeMessage();
        //             emailMessage.From.Add(new MailboxAddress("Compas", _emailConfig.UserName));
        //             if (this._appSettings.Enviroment.production)
        //             {
        //                 emailMessage.To.Add(email);
        //             }
        //             else
        //             {
        //                 // In environment development, send to a test email
        //                 emailMessage.To.Add(new MailboxAddress("Desarrollo DigitalPIR", "gmartinez@gruposinco.com.mx"));
        //             }
                    
        //             if (this._appSettings.Enviroment.production)
        //             {
        //                 emailMessage.Cc.Add(new MailboxAddress("Eric Montanez Valadez", "eric.montanezvaladez@compas-mx.com"));
        //                 emailMessage.Cc.Add(new MailboxAddress("Alias", "cp@gruposinco.com.mx"));
        //             }
        //             emailMessage.Sender = MailboxAddress.Parse(_emailConfig.UserName);
        //             emailMessage.Subject = "DigitalPIR@no-reply.com";
        //             emailMessage.Body = message.Body;
        //             emailMessage.MessageId = messageId;

        //             var response = await client.SendAsync(emailMessage);

        //             // Registrar como enviado exitosamente
        //             result = emailResultDto.IsDelivered = true;
        //             emailResultDto.DeliveryStatus = response;
                    
        //             await _emailDeliveryResultService.SaveEmailResultAsync(emailResultDto);
        //         }
        //         catch (Exception ex)
        //         {
        //             error = true;
        //             result = false;
        //             Console.WriteLine($"Error sending email to {contact.Email}: {ex.Message}");

        //             // Registrar como fallido
        //             emailResultDto.IsDelivered = false;
        //             emailResultDto.DeliveryStatus = "Failed";
        //             emailResultDto.ErrorMessage = ex.Message + ex.InnerException;
        //             emailResultDto.ErrorDetails = ex.StackTrace;

        //             try
        //             {
        //                 await _emailDeliveryResultService.SaveEmailResultAsync(emailResultDto);
        //             }
        //             catch (Exception saveEx)
        //             {
        //                 Console.WriteLine($"Error saving email delivery result: {saveEx.Message}");
        //             }

        //         }

        //         //     await Task.Delay(TimeSpan.FromSeconds(5));
        //         // }
            
        //     }
        //     finally
        //     {
        //         await DisconnectAndDisposeClientAsync(client);
        //     }

        //     return result;
        // }
    }
}