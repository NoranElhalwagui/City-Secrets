using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // 🔔 NOTIFICATIONS CONTROLLER
    // This handles user notifications (new reviews, place approvals, etc.)
    // Users can view, mark as read, and delete their notifications
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // All notification operations require login
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        
        // Constructor: Sets up the notification service
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // 📋 GET: api/notifications
        // What it does: Gets your notifications
        // Why: Shows alerts, updates, and messages from the app
        // Can filter: unreadOnly (true/false) to show only unread
        // Paginated: 20 per page by default
        // Returns: List with unread count and time ago ("2 hours ago")
        [HttpGet]
        public IActionResult GetUserNotifications(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Get notifications (with pagination and optional unread filter)
            var notifications = _notificationService.GetUserNotifications(userId, unreadOnly, page, pageSize);
            return Ok(notifications);
        }

        // 🔢 GET: api/notifications/unread-count
        // What it does: Gets count of unread notifications
        // Why: Shows notification badge number (e.g., "🔔 5")
        // Returns: Just the count number
        [HttpGet("unread-count")]
        public IActionResult GetUnreadCount()
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Get unread count
            var count = _notificationService.GetUnreadCount(userId);
            return Ok(new { count });
        }

        // ✅ PUT: api/notifications/{id}/read
        // What it does: Marks a notification as read
        // Why: Removes notification from unread list when user clicks it
        // Needs: notification ID in URL
        [HttpPut("{id}/read")]
        public IActionResult MarkAsRead(int id)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Mark notification as read
            var result = _notificationService.MarkAsRead(id, userId);
            
            // If notification not found or doesn't belong to user
            if (!result)
                return NotFound(new { message = "Notification not found" });
            
            return Ok(new { message = "Notification marked as read" });
        }

        // ✅✅ PUT: api/notifications/read-all
        // What it does: Marks ALL your notifications as read
        // Why: Quick way to clear all unread badges
        // Returns: Count of how many were marked as read
        [HttpPut("read-all")]
        public IActionResult MarkAllAsRead()
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Mark all as read
            var count = _notificationService.MarkAllAsRead(userId);
            return Ok(new { message = "All notifications marked as read", count });
        }

        // 🗑️ DELETE: api/notifications/{id}
        // What it does: Deletes a notification
        // Why: Remove notifications you don't want to see anymore
        // Needs: notification ID in URL
        [HttpDelete("{id}")]
        public IActionResult DeleteNotification(int id)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Delete the notification (soft delete)
            var result = _notificationService.DeleteNotification(id, userId);
            
            // If notification not found
            if (!result)
                return NotFound(new { message = "Notification not found" });
            
            return Ok(new { message = "Notification deleted" });
        }

        // 🧹 DELETE: api/notifications/clear-read
        // What it does: Clears all read notifications
        // Why: Clean up your notification list (keeps only unread)
        // Returns: Count of how many were deleted
        [HttpDelete("clear-read")]
        public IActionResult ClearReadNotifications()
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Clear all read notifications
            var count = _notificationService.ClearReadNotifications(userId);
            return Ok(new { message = "Read notifications cleared", count });
        }

        // ⚙️ PUT: api/notifications/preferences
        // What it does: Updates notification settings
        // Why: Control which types of notifications you receive
        // Needs: preferences (email, push, types of notifications)
        [HttpPut("preferences")]
        public IActionResult UpdateNotificationPreferences([FromBody] NotificationPreferencesDto request)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Update preferences
            _notificationService.UpdateNotificationPreferences(userId, request);
            return Ok(new { message = "Notification preferences updated" });
        }
    }
}