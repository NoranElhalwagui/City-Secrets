using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class UserPreference
    {
        [Key]
        public int PreferenceId { get; set; }
        
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        
        // Navigation property-> lets EF connect tables as objects "so u can access related data easily in code"
        public User? User { get; set; }
        
        [MaxLength(500)]
        public string? PreferredCategories { get; set; }
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? MinPrice { get; set; }
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? MaxPrice { get; set; }
        
        [Range(0, 5)]
        public decimal? MinRating { get; set; }
        
        public double? MaxDistance { get; set; } // in km
        
        public bool PreferHiddenGems { get; set; } = true;
        
        public bool NotificationEnabled { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
}