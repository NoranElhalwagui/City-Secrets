using CitySecrets.Models;
using CitySecrets.DTOs;

namespace CitySecrets.Services.Interfaces
{
    public interface INotificationService
    {
        // Get user notifications with pagination
        PaginatedNotificationsResult GetUserNotifications(int userId, bool unreadOnly, int page, int pageSize);
        
        // Get unread notification count
        int GetUnreadCount(int userId);
        
        // Mark single notification as read
        bool MarkAsRead(int notificationId, int userId);
        
        // Mark all user notifications as read
        int MarkAllAsRead(int userId);
        
        // Delete a notification
        bool DeleteNotification(int notificationId, int userId);
        
        // Clear all read notifications
        int ClearReadNotifications(int userId);
        
        // Update user notification preferences
        void UpdateNotificationPreferences(int userId, NotificationPreferencesDto preferences);
        
        // Send a notification (system/admin use)
        bool SendNotification(SendNotificationRequest request);
    }
}
