using System;
using System.ComponentModel.DataAnnotations;

namespace CitySecrets.Models
{
    public class AdminActionLog
    {
        [Key]
        public int LogId { get; set; }
        
        [Required]
        public int AdminUserId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public required string ActionType { get; set; }
        
        [Required]
        [MaxLength(50)]
        public required string EntityType { get; set; }
        
        public int? EntityId { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
