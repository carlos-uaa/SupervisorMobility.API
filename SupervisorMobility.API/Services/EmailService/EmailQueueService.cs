using SupervisorMobility.API.Models;
using SupervisorMobility.API.Models.Email;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Services.EmailService
{
    public interface IEmailQueueService
    {
        Task<EmailQueue> AddEmailQueueEntryAsync(Notification notification, string type, int? targetRelationId = null, string CCPEmails = null);
        Task<ServiceResponse<List<EmailQueue>>> GetPendingEmailQueuesAsync();
        Task<ServiceResponse<bool>> IncrementAttempt(int id);
        
        Task<ServiceResponse<EmailQueue>> AcceptEmailQueueAsync(int id);

        // Task<ServiceResponse<EmailQueue>> CreateEmailQueueAsync(EmailQueue emailQueue);
        // Task<ServiceResponse<EmailQueue>> GetEmailQueueByIdAsync(int id);
        // Task<ServiceResponse<List<EmailQueue>>> GetAllEmailQueuesAsync();
        // Task<ServiceResponse<List<EmailQueue>>> GetEmailQueuesByUserAsync(int userId);
        // Task<ServiceResponse<List<EmailQueue>>> GetEmailQueuesByPIRAsync(int pirId);
        // Task<ServiceResponse<EmailQueue>> UpdateEmailQueueAsync(int id, EmailQueue emailQueue);
        // Task<ServiceResponse<bool>> DeleteEmailQueueAsync(int id);
        
        // Task<ServiceResponse<EmailQueue>> RejectEmailQueueAsync(int id);
        
    }

    public class EmailQueueService : IEmailQueueService
    {
        private readonly ISupervisorMobilityRepository _repository;

        public EmailQueueService(
            ISupervisorMobilityRepository repository
        )
        {
            _repository = repository;
        }


        public async Task<EmailQueue> AddEmailQueueEntryAsync(Notification notification, string type, int? targetRelationId = null, string CCPEmails = null)
        {
            // Logic to add the notification to the email queue
            EmailQueue emailQueueEntry = new EmailQueue
            {
                MadeByID = notification.UserId,
                NotificationType = notification.NotificationType ?? type,
                StaffID = notification.UserId, // Assuming the staff is the same as the user for this example
                EntryDate = DateTime.Now,
                IsSend = false,
                Attempts = 0,
                TargetRelationID = targetRelationId ?? 0,
                TargetRelationAux = getTargetAux(type, notification),
                CCPEmails = CCPEmails
            };

            return await _repository.AddEmailQueueEntryAsync(emailQueueEntry);
        }

        public async Task<ServiceResponse<List<EmailQueue>>> GetPendingEmailQueuesAsync()
        {
            try
            {
                var emailQueue = await _repository.GetPendingEmailQueuesAsync();

                return new ServiceResponse<List<EmailQueue>>
                {
                    Success = true,
                    Data = emailQueue
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmailQueue>>
                {
                    Success = false,
                    Message = $"Error retrieving pending email queues: {ex.Message}"
                };
            }
        }


        public async Task<ServiceResponse<bool>> IncrementAttempt(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var emailQueue = await _repository.GetEmailQueueByIdAsync(id);
                if (emailQueue == null)
                {
                    response.Success = false;
                    response.Message = $"Email queue with ID {id} not found";
                    response.Data = false;
                    return response;
                }


                if (emailQueue.Attempts < 5)
                {
                    emailQueue.Attempts = emailQueue.Attempts + 1;
                    await _repository.UpdateEmailQueueAsync(emailQueue);
                }
                else
                {
                    await AcceptEmailQueueAsync(id);
                }

                response.Success = true;
                response.Message = "Attempt incremented successfully";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error incrementing attempt: {ex.Message}";
                response.Data = false;
                return response;
            }
        }

        public async Task<ServiceResponse<EmailQueue>> AcceptEmailQueueAsync(int id)
        {
            try
            {
                var emailQueue = await _repository.GetEmailQueueByIdAsync(id);

                if (emailQueue == null)
                {
                    return new ServiceResponse<EmailQueue>
                    {
                        Success = false,
                        Message = $"Email queue with ID {id} not found"
                    };
                }

                emailQueue.IsSend = true;
                emailQueue.SendDate = DateTime.Now;
                await _repository.UpdateEmailQueueAsync(emailQueue);

                return new ServiceResponse<EmailQueue>
                {
                    Success = true,
                    Data = await _repository.GetEmailQueueByIdAsync(id),
                    Message = "Email queue accepted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmailQueue>
                {
                    Success = false,
                    Message = $"Error accepting email queue: {ex.Message}"
                };
            }
        }

        private string? getTargetAux(string type, Notification notification)
        {
            switch(type)
            {
                case "RevisionWithNG":
                    return notification.NotificationText;
                default:
                    return null;
            }
        }

















        // public async Task<ServiceResponse<EmailQueue>> CreateEmailQueueAsync(Notification notification, Users madeby)
        // {
        //     var emailQueue = new EmailQueue
        //     {
        //         MadeByID = madeby.UserID,
        //         TargetRelationID = notification.TargetRelation,
        //         NotificationType = notification.NotificationType,
        //         StaffID = notification.Staff.UserID,
        //         EntryDate = DateTime.Now,
        //         IsSend = false,
        //         SendDate = null,
        //         Attempts = 0
        //     };

        //     return await CreateEmailQueueAsync(emailQueue);
        // }
        // public async Task<ServiceResponse<EmailQueue>> CreateEmailQueueAsync(EmailQueue emailQueue)
        // {
        //     try
        //     {
        //         // Validar que el usuario MadeBy existe si se proporciona
        //         if (emailQueue.MadeByID.HasValue)
        //         {
        //             var madeByExists = await _context.Users.AnyAsync(u => u.UserID == emailQueue.MadeByID.Value);
        //             if (!madeByExists)
        //             {
        //                 return new ServiceResponse<EmailQueue>
        //                 {
        //                     Success = false,
        //                     Message = $"User with ID {emailQueue.MadeByID.Value} not found"
        //                 };
        //             }
        //         }

        //         // Validar que el PIR existe si se proporciona
        //         if (emailQueue.TargetRelationID.HasValue)
        //         {
        //             var pirExists = await _context.PIR.AnyAsync(p => p.PIRID == emailQueue.TargetRelationID.Value);
        //             if (!pirExists)
        //             {
        //                 return new ServiceResponse<EmailQueue>
        //                 {
        //                     Success = false,
        //                     Message = $"PIR with ID {emailQueue.TargetRelationID.Value} not found"
        //                 };
        //             }
        //         }

        //         // Validar que el usuario Staff existe si se proporciona
        //         if (emailQueue.StaffID.HasValue)
        //         {
        //             var staffExists = await _context.Users.AnyAsync(u => u.UserID == emailQueue.StaffID.Value);
        //             if (!staffExists)
        //             {
        //                 return new ServiceResponse<EmailQueue>
        //                 {
        //                     Success = false,
        //                     Message = $"Staff user with ID {emailQueue.StaffID.Value} not found"
        //                 };
        //             }
        //         }

        //         emailQueue.EntryDate = DateTime.Now;
        //         _context.EmailQueues.Add(emailQueue);
        //         await _context.SaveChangesAsync();

        //         return new ServiceResponse<EmailQueue>
        //         {
        //             Success = true,
        //             Data = await GetEmailQueueWithRelationsAsync(emailQueue.EmailQueueID),
        //             Message = "Email queue created successfully"
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ServiceResponse<EmailQueue>
        //         {
        //             Success = false,
        //             Message = $"Error creating email queue: {ex.Message}"
        //         };
        //     }
        // }

        
        // public async Task<ServiceResponse<EmailQueue>> RejectEmailQueueAsync(int id)
        // {
        //     try
        //     {
        //         var emailQueue = await _context.EmailQueues.FindAsync(id);

        //         if (emailQueue == null)
        //         {
        //             return new ServiceResponse<EmailQueue>
        //             {
        //                 Success = false,
        //                 Message = $"Email queue with ID {id} not found"
        //             };
        //         }

        //         emailQueue.IsSend = false;
        //         await _context.SaveChangesAsync();

        //         return new ServiceResponse<EmailQueue>
        //         {
        //             Success = true,
        //             Data = await GetEmailQueueWithRelationsAsync(id),
        //             Message = "Email queue rejected successfully"
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ServiceResponse<EmailQueue>
        //         {
        //             Success = false,
        //             Message = $"Error rejecting email queue: {ex.Message}"
        //         };
        //     }
        // }

        // public async Task<ServiceResponse<EmailQueue>> GetEmailQueueByIdAsync(int id)
        // {
        //     try
        //     {
        //         var emailQueue = await GetEmailQueueWithRelationsAsync(id);

        //         if (emailQueue == null)
        //         {
        //             return new ServiceResponse<EmailQueue>
        //             {
        //                 Success = false,
        //                 Message = $"Email queue with ID {id} not found"
        //             };
        //         }

        //         return new ServiceResponse<EmailQueue>
        //         {
        //             Success = true,
        //             Data = emailQueue
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ServiceResponse<EmailQueue>
        //         {
        //             Success = false,
        //             Message = $"Error retrieving email queue: {ex.Message}"
        //         };
        //     }
        // }

        // public async Task<ServiceResponse<List<EmailQueue>>> GetAllEmailQueuesAsync()
        // {
        //     try
        //     {
        //         var emailQueues = await _context.EmailQueues
        //             .Include(e => e.MadeBy)
        //             .Include(e => e.TargetRelation)
        //             .Include(e => e.Staff)
        //             .OrderByDescending(e => e.EntryDate)
        //             .ToListAsync();

        //         return new ServiceResponse<List<EmailQueue>>
        //         {
        //             Success = true,
        //             Data = emailQueues
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ServiceResponse<List<EmailQueue>>
        //         {
        //             Success = false,
        //             Message = $"Error retrieving email queues: {ex.Message}"
        //         };
        //     }
        // }

        // public async Task<ServiceResponse<List<EmailQueue>>> GetEmailQueuesByUserAsync(int userId)
        // {
        //     try
        //     {
        //         var emailQueues = await _context.EmailQueues
        //             .Include(e => e.MadeBy)
        //             .Include(e => e.TargetRelation)
        //             .Include(e => e.Staff)
        //             .Where(e => e.MadeByID == userId || e.StaffID == userId)
        //             .OrderByDescending(e => e.EntryDate)
        //             .ToListAsync();

        //         return new ServiceResponse<List<EmailQueue>>
        //         {
        //             Success = true,
        //             Data = emailQueues
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ServiceResponse<List<EmailQueue>>
        //         {
        //             Success = false,
        //             Message = $"Error retrieving email queues by user: {ex.Message}"
        //         };
        //     }
        // }

        // public async Task<ServiceResponse<List<EmailQueue>>> GetEmailQueuesByPIRAsync(int pirId)
        // {
        //     try
        //     {
        //         var emailQueues = await _context.EmailQueues
        //             .Include(e => e.MadeBy)
        //             .Include(e => e.TargetRelation)
        //             .Include(e => e.Staff)
        //             .Where(e => e.TargetRelationID == pirId)
        //             .OrderByDescending(e => e.EntryDate)
        //             .ToListAsync();

        //         return new ServiceResponse<List<EmailQueue>>
        //         {
        //             Success = true,
        //             Data = emailQueues
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ServiceResponse<List<EmailQueue>>
        //         {
        //             Success = false,
        //             Message = $"Error retrieving email queues by PIR: {ex.Message}"
        //         };
        //     }
        // }

        // public async Task<ServiceResponse<EmailQueue>> UpdateEmailQueueAsync(int id, EmailQueue emailQueue)
        // {
        //     try
        //     {
        //         var existingEmailQueue = await _context.EmailQueues.FindAsync(id);

        //         if (existingEmailQueue == null)
        //         {
        //             return new ServiceResponse<EmailQueue>
        //             {
        //                 Success = false,
        //                 Message = $"Email queue with ID {id} not found"
        //             };
        //         }

        //         // Validaciones similares al Create
        //         if (emailQueue.MadeByID.HasValue)
        //         {
        //             var madeByExists = await _context.Users.AnyAsync(u => u.UserID == emailQueue.MadeByID.Value);
        //             if (!madeByExists)
        //             {
        //                 return new ServiceResponse<EmailQueue>
        //                 {
        //                     Success = false,
        //                     Message = $"User with ID {emailQueue.MadeByID.Value} not found"
        //                 };
        //             }
        //         }

        //         if (emailQueue.TargetRelationID.HasValue)
        //         {
        //             var pirExists = await _context.PIR.AnyAsync(p => p.PIRID == emailQueue.TargetRelationID.Value);
        //             if (!pirExists)
        //             {
        //                 return new ServiceResponse<EmailQueue>
        //                 {
        //                     Success = false,
        //                     Message = $"PIR with ID {emailQueue.TargetRelationID.Value} not found"
        //                 };
        //             }
        //         }

        //         if (emailQueue.StaffID.HasValue)
        //         {
        //             var staffExists = await _context.Users.AnyAsync(u => u.UserID == emailQueue.StaffID.Value);
        //             if (!staffExists)
        //             {
        //                 return new ServiceResponse<EmailQueue>
        //                 {
        //                     Success = false,
        //                     Message = $"Staff user with ID {emailQueue.StaffID.Value} not found"
        //                 };
        //             }
        //         }

        //         // Actualizar propiedades
        //         existingEmailQueue.MadeByID = emailQueue.MadeByID;
        //         existingEmailQueue.TargetRelationID = emailQueue.TargetRelationID;
        //         existingEmailQueue.StaffID = emailQueue.StaffID;
        //         existingEmailQueue.NotificationType = emailQueue.NotificationType;
        //         existingEmailQueue.IsSend = emailQueue.IsSend;

        //         await _context.SaveChangesAsync();

        //         return new ServiceResponse<EmailQueue>
        //         {
        //             Success = true,
        //             Data = await GetEmailQueueWithRelationsAsync(id),
        //             Message = "Email queue updated successfully"
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ServiceResponse<EmailQueue>
        //         {
        //             Success = false,
        //             Message = $"Error updating email queue: {ex.Message}"
        //         };
        //     }
        // }

        // public async Task<ServiceResponse<bool>> DeleteEmailQueueAsync(int id)
        // {
        //     try
        //     {
        //         var emailQueue = await _context.EmailQueues.FindAsync(id);

        //         if (emailQueue == null)
        //         {
        //             return new ServiceResponse<bool>
        //             {
        //                 Success = false,
        //                 Message = $"Email queue with ID {id} not found"
        //             };
        //         }

        //         _context.EmailQueues.Remove(emailQueue);
        //         await _context.SaveChangesAsync();

        //         return new ServiceResponse<bool>
        //         {
        //             Success = true,
        //             Data = true,
        //             Message = "Email queue deleted successfully"
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ServiceResponse<bool>
        //         {
        //             Success = false,
        //             Message = $"Error deleting email queue: {ex.Message}"
        //         };
        //     }
        // }

        // private async Task<EmailQueue?> GetEmailQueueWithRelationsAsync(int id)
        // {
        //     return await _context.EmailQueues
        //         .Include(e => e.MadeBy)
        //         .Include(e => e.TargetRelation)
        //         .Include(e => e.Staff)
        //         .FirstOrDefaultAsync(e => e.EmailQueueID == id);
        // }


    }
}
