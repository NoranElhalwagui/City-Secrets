using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        // Navigation property
        public Place? Place { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        // Navigation property
        public User? User { get; set; }

        [Required]
        [Range(0, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public required string Comment { get; set; }

        [Range(-1, 1)]
        public double SentimentScore { get; set; }

        public int HelpfulCount { get; set; } = 0; //Counts how many users clicked “Helpful” on this review.

        public DateTime? VisitDate { get; set; } // data when the user visited the place => we can filter or sort reviews by “most recent visit.”

        public bool IsApproved { get; set; } = true; // if true the review is allowed to show publicly 
        public bool IsFlagged { get; set; } = false; // if users report a review IsFlagged = true

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties - Collections
        public ICollection<ReviewHelpfulness>? ReviewHelpfulness { get; set; }
    }
}