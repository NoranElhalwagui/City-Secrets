using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class PlaceView
    {
        [Key]
        public int ViewId { get; set; }
        
        [Required]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }
        
        // Navigation property
        public Place? Place { get; set; }
        
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; } // Null for anonymous/guest users
        
        // Navigation property
        public User? User { get; set; }
        
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
        
        [MaxLength(50)]
        public string? IpAddress { get; set; } // For analytics and preventing view spam
        
        [MaxLength(500)]
        public string? UserAgent { get; set; } // Browser/device information
        
        [MaxLength(100)]
        public string? SessionId { get; set; } // Group views from same session
        
        [MaxLength(20)]
        public string? DeviceType { get; set; } // Mobile, Desktop, Tablet
        
        [MaxLength(100)]
        public string? Referrer { get; set; } // Where the user came from
        
        public int? DurationSeconds { get; set; } // How long they viewed
        
        public bool IsDeleted { get; set; } = false;
    }
}