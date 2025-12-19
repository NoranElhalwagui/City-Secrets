using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        
        // Navigation property
        public User? User { get; set; }
        
        [Required]
        [MaxLength(50)]
        public required string Type { get; set; } // fro example "NewReview", "PlaceApproved", "ReviewLiked"
        
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }
        
        [Required]
        [MaxLength(500)]
        public required string Message { get; set; }
        
        [MaxLength(50)]
        public string? RelatedEntityType { get; set; } // "Place", "Review", "User"
        
        public int? RelatedEntityId { get; set; } // ID of the related entity
        
        [MaxLength(500)]
        public string? ActionUrl { get; set; } // URL to navigate when notification is clicked
        
        [MaxLength(100)]
        public string? Icon { get; set; } // Icon name or emoji for UI
        
        [MaxLength(20)]
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent
        
        public bool IsRead { get; set; } = false;
        
        public DateTime? ReadAt { get; set; } // Track when user read the notification
        
        public DateTime? ExpiresAt { get; set; } // Auto-delete old notifications
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsDeleted { get; set; } = false;
    }
}