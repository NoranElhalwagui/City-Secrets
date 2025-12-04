namespace CitySecrets.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!; 

        public int PlaceId { get; set; }
        public required Place Place { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
