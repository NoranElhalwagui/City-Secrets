using System;

namespace CitySecrets.DTOs
{
    // DTO for notification in responses
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? ActionUrl { get; set; }
        public string? Icon { get; set; }
        public string Priority { get; set; } = "Normal";
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = ""; // Human-readable time like "2 hours ago"
    }

    // DTO for paginated notifications result
    public class PaginatedNotificationsResult
    {
        public List<NotificationDto> Notifications { get; set; } = new();
        public int Total { get; set; }
        public int UnreadCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    // DTO for notification preferences
    public class NotificationPreferencesDto
    {
        public bool EnableEmail { get; set; }
        public bool EnablePush { get; set; }
        public bool EnableReviewNotifications { get; set; }
        public bool EnableRecommendationNotifications { get; set; }
        public bool EnablePlaceApprovalNotifications { get; set; }
        public bool EnableNewFollowerNotifications { get; set; }
    }

    // DTO for sending a notification (admin/system use)
    public class SendNotificationRequest
    {
        public int UserId { get; set; }
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string Priority { get; set; } = "Normal";
    }
}
