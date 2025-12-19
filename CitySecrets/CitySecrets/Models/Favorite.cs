using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        // Navigation property
        public User? User { get; set; }

        [Required]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        // Navigation property
        public Place? Place { get; set; }

        // Optional: Organize favorites into lists (e.g., "Want to Visit", "My Favorites")
        [MaxLength(100)]
        public string? ListName { get; set; }

        // Optional: Personal notes about this favorite place
        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
