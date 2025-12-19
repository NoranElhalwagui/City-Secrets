using CitySecrets.Models;
using CitySecrets.DTOs;
using CitySecrets.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CitySecrets.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public PaginatedNotificationsResult GetUserNotifications(int userId, bool unreadOnly, int page, int pageSize)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var total = query.Count();
            var unreadCount = _context.Notifications.Count(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);

            var notifications = query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    Type = n.Type,
                    Title = n.Title,
                    Message = n.Message,
                    RelatedEntityType = n.RelatedEntityType,
                    RelatedEntityId = n.RelatedEntityId,
                    ActionUrl = n.ActionUrl,
                    Icon = n.Icon,
                    Priority = n.Priority,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt,
                    TimeAgo = GetTimeAgo(n.CreatedAt)
                })
                .ToList();

            return new PaginatedNotificationsResult
            {
                Notifications = notifications,
                Total = total,
                UnreadCount = unreadCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public int GetUnreadCount(int userId)
        {
            return _context.Notifications
                .Count(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
        }

        public bool MarkAsRead(int notificationId, int userId)
        {
            var notification = _context.Notifications
                .FirstOrDefault(n => n.NotificationId == notificationId && n.UserId == userId && !n.IsDeleted);

            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _context.SaveChanges();

            return true;
        }

        public int MarkAllAsRead(int userId)
        {
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToList();

            var count = notifications.Count;
            var now = DateTime.UtcNow;

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
            }

            _context.SaveChanges();
            return count;
        }

        public bool DeleteNotification(int notificationId, int userId)
        {
            var notification = _context.Notifications
                .FirstOrDefault(n => n.NotificationId == notificationId && n.UserId == userId && !n.IsDeleted);

            if (notification == null)
                return false;

            // Soft delete
            notification.IsDeleted = true;
            _context.SaveChanges();

            return true;
        }

        public int ClearReadNotifications(int userId)
        {
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId && n.IsRead && !n.IsDeleted)
                .ToList();

            var count = notifications.Count;

            foreach (var notification in notifications)
            {
                notification.IsDeleted = true;
            }

            _context.SaveChanges();
            return count;
        }

        public void UpdateNotificationPreferences(int userId, NotificationPreferencesDto preferences)
        {
            // This would typically update a UserPreferences table
            // For now, we'll just accept it (you can implement storage later)
            var userPref = _context.UserPreferences.FirstOrDefault(up => up.UserId == userId);
            
            if (userPref != null)
            {
                // Store preferences in UserPreferences model
                // You may need to add these fields to UserPreferences model
                _context.SaveChanges();
            }
        }

        public bool SendNotification(SendNotificationRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Type = request.Type,
                Title = request.Title,
                Message = request.Message,
                RelatedEntityType = request.RelatedEntityType,
                RelatedEntityId = request.RelatedEntityId,
                Priority = request.Priority,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();

            return true;
        }

        // Helper method to convert DateTime to human-readable format
        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} min ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)}w ago";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)}mo ago";
            
            return $"{(int)(timeSpan.TotalDays / 365)}y ago";
        }
    }
}
