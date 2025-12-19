using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace CitySecrets.Models
{
    
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; } 

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        // Navigation properties - Collections
        public ICollection<Place>? Places { get; set; }
        public ICollection<SearchHistory>? SearchHistories { get; set; }
    }
}