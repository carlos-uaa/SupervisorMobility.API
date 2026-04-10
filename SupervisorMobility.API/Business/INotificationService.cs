using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.NotificationDtos;

namespace SupervisorMobility.API.Business
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(NotificationToCreateDto notify, SpecialOptionsNotification? specialOptions = null);
        Task<bool> UpdateNotificationAsync(NotificationForUpdateDto notifyForUpdate, Notification notifyEntity);
        Task<bool> RemoveNotificationAsync(Notification notificationEntity);
        Task<IEnumerable<Notification>> GetNotificationsAsync();
        Task<Notification?> FetchNotificationAsync(int notificationId);
        Task<IEnumerable<Notification>> GetNotificationsFromUserAsync(int userId);
    }
}