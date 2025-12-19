using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class AdminActionLog
    {
        [Key]
        public int LogId { get; set; }
        
        [Required]
        [ForeignKey(nameof(AdminUser))]
        public int AdminUserId { get; set; }

        // Navigation property
        public User? AdminUser { get; set; }
        
        [Required]
        [MaxLength(50)]
        public required string ActionType { get; set; }
        
        [Required]
        [MaxLength(50)]
        public required string EntityType { get; set; }
        
        public int? EntityId { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public string? OldValues { get; set; } // JSON string of old values
        public string? NewValues { get; set; } // JSON string of new values

        [MaxLength(50)]
        public string? IpAddress { get; set; } // Track IP for security audit
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
