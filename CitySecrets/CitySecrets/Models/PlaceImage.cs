using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class PlaceImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        // Navigation property
        public Place? Place { get; set; }

        [ForeignKey(nameof(UploadedByUser))]
        public int? UploadedByUserId { get; set; } // Track who uploaded

        // Navigation property
        public User? UploadedByUser { get; set; }

        [Required]
        [MaxLength(500)]
        public required string ImageUrl { get; set; }

        [MaxLength(200)]
        public string? Caption { get; set; }

        public bool IsPrimary { get; set; } = false; // Main photo for the place

        public int DisplayOrder { get; set; } = 0; // Order to display images

        public bool IsApproved { get; set; } = true; // Admin approval

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
