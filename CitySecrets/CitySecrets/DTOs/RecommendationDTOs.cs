using System.ComponentModel.DataAnnotations;

namespace CitySecrets.DTOs
{
    // ============ REQUEST DTOs ============
    
    public class RecommendationRequest
    {
        [Range(1, 100, ErrorMessage = "Count must be between 1 and 100")]
        public int Count { get; set; } = 10;
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        [Range(1, 50, ErrorMessage = "Radius must be between 1 and 50 km")]
        public double? RadiusKm { get; set; } = 5.0;
        
        public List<int>? CategoryIds { get; set; }
        public string? PriceRange { get; set; }
        public int? MinRating { get; set; }
        public bool? VerifiedOnly { get; set; }
    }
    
    public class PersonalizedRecommendationRequest
    {
        [Range(1, 50, ErrorMessage = "Count must be between 1 and 50")]
        public int Count { get; set; } = 10;
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        public bool IncludeVisited { get; set; } = false;
        public bool IncludeFavorites { get; set; } = false;
    }
    
    public class HiddenGemSearchRequest
    {
        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double Latitude { get; set; }
        
        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double Longitude { get; set; }
        
        [Range(1, 50, ErrorMessage = "Count must be between 1 and 50")]
        public int Count { get; set; } = 10;
        
        [Range(1, 50, ErrorMessage = "Radius must be between 1 and 50 km")]
        public double RadiusKm { get; set; } = 10.0;
        
        [Range(1, 100, ErrorMessage = "Min hidden gem score must be between 1 and 100")]
        public int? MinHiddenGemScore { get; set; } = 50;
    }
    
    // ============ RESPONSE DTOs ============
    
    public class RecommendationDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? DistanceKm { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal? AveragePrice { get; set; }
        public string? PriceRange { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsHiddenGem { get; set; }
        public int HiddenGemScore { get; set; }
        public bool IsVerified { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public double RecommendationScore { get; set; }
        public string RecommendationReason { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }
    
    public class PersonalizedRecommendationDto : RecommendationDto
    {
        public bool HasVisited { get; set; }
        public bool IsFavorited { get; set; }
        public double PersonalizationScore { get; set; }
        public List<string> MatchReasons { get; set; } = new();
    }
    
    public class TrendingPlaceDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int ViewsLast7Days { get; set; }
        public int ReviewsLast7Days { get; set; }
        public double TrendingScore { get; set; }
        public string TrendingReason { get; set; } = string.Empty;
    }
    
    public class RecommendationResultDto
    {
        public List<RecommendationDto> Recommendations { get; set; } = new();
        public int TotalCount { get; set; }
        public string? BasedOn { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }
    
    public class RecommendationStatsDto
    {
        public int TotalPlaces { get; set; }
        public int HiddenGems { get; set; }
        public int VerifiedPlaces { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, int> CategoryDistribution { get; set; } = new();
    }
}
