using CitySecrets.Models;
using CitySecrets.Services.Interfaces;
namespace CitySecrets.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly List<Notification> _notifications = new();

        public async Task<bool> SendNotificationAsync(
            int userId,
            NotificationType type,
            string title,
            string message,
            int? relatedEntityId = null)
        {
            _notifications.Add(new Notification
            {
                NotificationId = _notifications.Count + 1,
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                RelatedEntityId = relatedEntityId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            return true;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
        {
            var result = _notifications.Where(n => n.UserId == userId);
            if (unreadOnly)
                result = result.Where(n => !n.IsRead);

            return result;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = _notifications
                .FirstOrDefault(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            foreach (var n in _notifications.Where(n => n.UserId == userId))
                n.IsRead = true;

            return true;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return _notifications.Count(n => n.UserId == userId && !n.IsRead);
        }
    }
}
