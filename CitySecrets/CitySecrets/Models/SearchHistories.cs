using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class SearchHistory
    {
        [Key]
        public int SearchId { get; set; }
        
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        
        // Navigation property
        public User? User { get; set; }
        
        [Required]
        [MaxLength(500)]
        public required string SearchQuery { get; set; }
        
        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        
        // Navigation property
        public Category? Category { get; set; }
        
        public double? Latitude { get; set; }
        
        public double? Longitude { get; set; }
        
        [MaxLength(1000)]
        public string? Filters { get; set; } // JSON "value pairs: organize data using key" string of applied filters
        
        public int ResultCount { get; set; } = 0;
        
        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsDeleted { get; set; } = false;
    }
}