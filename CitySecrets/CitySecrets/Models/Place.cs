using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class Place
    {
        [Key]
        public int PlaceId { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public required string Address { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal AveragePrice { get; set; }

        public int VisitCount { get; set; }

        [MaxLength(20)]
        public string? PriceRange { get; set; }

        [Range(0, 5)]
        public double AverageRating { get; set; }

        public int TotalReviews { get; set; }

        public int PopularityScore { get; set; }

        [Range(-1, 1)]
        public double SentimentScore { get; set; }//how positive or negative people feel about this place "will be calculated"

        public bool IsHiddenGem { get; set; }

        [Range(0, 100)]
        public int HiddenGemScore { get; set; } //how much a place is a hidden gem 

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public string? OpeningHours { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        [Required]
        public int CategoryId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int RatingCount1 { get; set; }//number of people put one star for this place and so on 
        public int RatingCount2 { get; set; }
        public int RatingCount3 { get; set; }
        public int RatingCount4 { get; set; }
        public int RatingCount5 { get; set; }


    }
}