using System;
using System.ComponentModel.DataAnnotations;

namespace CitySecrets.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        
        public int UserId { get; set; }
        public User? User { get; set; }

        
        public int PlaceId { get; set; }
        public Place? Place { get; set; }

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

