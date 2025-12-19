using CitySecrets.Models;

namespace CitySecrets.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(int userId, NotificationType type, string title, string message, int? relatedEntityId = null);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
