using AutoMapper;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Services.EmailService;
using SupervisorMobility.API.Services.WhatsAppService;

namespace SupervisorMobility.API.Business
{
    public class NotificationService : INotificationService
    {
        private readonly ISupervisorMobilityRepository _repository;
        private readonly IEmailQueueService _emailQueueService;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IMapper _mapper;

        public NotificationService(
            ISupervisorMobilityRepository repository,
            IEmailQueueService emailQueueService,
            IWhatsAppService whatsAppService,
            IMapper mapper
        )
        {
            _repository = repository;
            _emailQueueService = emailQueueService;
            _whatsAppService = whatsAppService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new notification.
        /// </summary>
        /// <param name="notify">The notification to create.</param>
        /// <param name="specialOptions">The special options for the notification.</param>
        /// <returns>The created notification.</returns>
        public async Task<Notification> CreateNotificationAsync(NotificationToCreateDto notify, SpecialOptionsNotification? specialOptions = null)
        {
            var notifyToAdd = _mapper.Map<Notification>(notify);

            _repository.AddNotificationAsync(notifyToAdd);
            await _repository.SaveChangesAsync();

            if(specialOptions != null)
            {
                if(specialOptions.Email.HasValue && specialOptions.Email.Value)
                {
                    var emailQueueEntry = await _emailQueueService.AddEmailQueueEntryAsync(notifyToAdd, specialOptions.type);
                }

                if(specialOptions.WhatsApp.HasValue && specialOptions.WhatsApp.Value)
                {                    
                    var recipientPhoneNumber = "524492339120"; // This should be dynamically determined based on the notification data
                    var whatsAppSended = await _whatsAppService.SendWhatsAppTemplateAsync(recipientPhoneNumber, specialOptions.type);
                }

                if(specialOptions.MicrosoftTeams.HasValue && specialOptions.MicrosoftTeams.Value)
                {
                    // Logic to add the notification to the Microsoft Teams queue
                    // This is a placeholder for the actual implementation
                    // You would need to create a MicrosoftTeamsQueue entity and repository method similar to EmailQueue
                }
            }

            return notifyToAdd;
        }

        /// <summary>
        /// Updates a notification.
        /// </summary>
        /// <param name="notifyForUpdate">The notification data to update.</param>
        /// <param name="notifyEntity">The existing notification entity to update.</param>
        /// <returns>A boolean indicating whether the update was successful.</returns>
        public async Task<bool> UpdateNotificationAsync(NotificationForUpdateDto notifyForUpdate, Notification notifyEntity)
        {
            _mapper.Map(notifyForUpdate, notifyEntity);
            await _repository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Removes a notification.
        /// </summary>
        /// <param name="notificationEntity">The notification entity to remove.</param>
        /// <returns>A boolean indicating whether the removal was successful.</returns>
        public async Task<bool> RemoveNotificationAsync(Notification notificationEntity)
        {
            _repository.DeleteNotificationAsync(notificationEntity);
            await _repository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets all notifications.
        /// </summary>
        /// <returns>A list of notifications.</returns>
        public async Task<IEnumerable<Notification>> GetNotificationsAsync()
        {
            return await _repository.GetAllNotificationsAsync();
        }

        /// <summary>
        /// Get a specific notification by its ID.
        /// </summary>
        /// <param name="notificationId">The ID of the notification to fetch.</param>
        /// <returns>The fetched notification or null if not found.</returns>
        public async Task<Notification?> FetchNotificationAsync(int notificationId)
        {
            return await _repository.GetNotificationAsync(notificationId);
        }

        /// <summary>
        /// Gets all notifications for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose notifications to fetch.</param>
        /// <returns>A list of notifications for the specified user.</returns>
        public async Task<IEnumerable<Notification>> GetNotificationsFromUserAsync(int userId)
        {
            return await _repository.GetAllNotificationsFromUserAsync(userId);
        }
    }
}