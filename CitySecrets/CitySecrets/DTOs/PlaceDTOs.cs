using System.ComponentModel.DataAnnotations;

namespace CitySecrets.DTOs
{
    // Place Filter Request
    public class PlaceFilterRequest
    {
        public int? CategoryId { get; set; }
        
        [Range(0, 10000)]
        public decimal? MinPrice { get; set; }
        
        [Range(0, 10000)]
        public decimal? MaxPrice { get; set; }
        
        [Range(0, 5)]
        public decimal? MinRating { get; set; }
        
        public bool? IsHiddenGem { get; set; }
        public bool? IsVerified { get; set; }
        
        [RegularExpression("^(name|rating|price|popularity|distance)$")]
        public string SortBy { get; set; } = "popularity";
        
        [RegularExpression("^(asc|desc)$")]
        public string SortOrder { get; set; } = "desc";
        
        [Range(1, 100)]
        public int Page { get; set; } = 1;
        
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
    }

    // Create Place Request
    public class CreatePlaceRequest
    {
        [Required(ErrorMessage = "Place name is required")]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = "";
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = "";
        
        [Required]
        [StringLength(500)]
        public string Address { get; set; } = "";
        
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }
        
        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [Range(0, 10000)]
        public decimal? AveragePrice { get; set; }
        
        [StringLength(20)]
        public string? PriceRange { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [Url]
        public string? Website { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? OpeningHours { get; set; }
    }

    // Update Place Request
    public class UpdatePlaceRequest
    {
        [StringLength(200, MinimumLength = 3)]
        public string? Name { get; set; }
        
        [StringLength(2000)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [Range(-90, 90)]
        public double? Latitude { get; set; }
        
        [Range(-180, 180)]
        public double? Longitude { get; set; }
        
        public int? CategoryId { get; set; }
        
        [Range(0, 10000)]
        public decimal? AveragePrice { get; set; }
        
        [StringLength(20)]
        public string? PriceRange { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [Url]
        public string? Website { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? OpeningHours { get; set; }
    }

    // Place Response DTO
    public class PlaceDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Address { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal AveragePrice { get; set; }
        public string? PriceRange { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int VisitCount { get; set; }
        public int PopularityScore { get; set; }
        public bool IsHiddenGem { get; set; }
        public int HiddenGemScore { get; set; }
        public bool IsVerified { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? OpeningHours { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double? DistanceKm { get; set; } // For nearby searches
        public bool IsFavorited { get; set; } // For logged-in users
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Place Statistics DTO
    public class PlaceStatisticsDto
    {
        public int PlaceId { get; set; }
        public string PlaceName { get; set; } = "";
        public int TotalViews { get; set; }
        public int TotalReviews { get; set; }
        public int TotalFavorites { get; set; }
        public double AverageRating { get; set; }
        public int Rating1Count { get; set; }
        public int Rating2Count { get; set; }
        public int Rating3Count { get; set; }
        public int Rating4Count { get; set; }
        public int Rating5Count { get; set; }
        public double SentimentScore { get; set; }
        public int PopularityScore { get; set; }
        public int VisitCount { get; set; }
    }

    // Paginated Result
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
