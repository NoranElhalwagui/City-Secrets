using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class ReviewHelpfulness
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [ForeignKey(nameof(Review))]
        public int ReviewId { get; set; }
        
        // Navigation property
        public Review? Review { get; set; }
        
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        
        // Navigation property
        public User? User { get; set; }
        
        [Required]
        public bool IsHelpful { get; set; } // true = helpful, false = not helpful
        
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsDeleted { get; set; } = false;
    }
}