using System.ComponentModel.DataAnnotations;

namespace CitySecrets.DTOs
{
    // ============ REQUEST DTOs ============
    
    public class ReviewFilterRequest
    {
        public string? SortBy { get; set; } = "Newest"; // Newest, Oldest, Highest, Lowest, MostHelpful
        
        [Range(1, 5, ErrorMessage = "Minimum rating must be between 1 and 5")]
        public int? MinRating { get; set; }
        
        [Range(1, 5, ErrorMessage = "Maximum rating must be between 1 and 5")]
        public int? MaxRating { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;
        
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;
        
        public bool? IsVerifiedVisit { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
    
    public class CreateReviewRequest
    {
        [Required(ErrorMessage = "Place ID is required")]
        public int PlaceId { get; set; }
        
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }
        
        [Required(ErrorMessage = "Comment is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 2000 characters")]
        public string Comment { get; set; } = string.Empty;
        
        public DateTime? VisitDate { get; set; }
        
        public List<string>? Images { get; set; }
    }
    
    public class UpdateReviewRequest
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Rating { get; set; }
        
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }
        
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 2000 characters")]
        public string? Comment { get; set; }
        
        public DateTime? VisitDate { get; set; }
    }
    
    public class MarkHelpfulRequest
    {
        [Required]
        public bool IsHelpful { get; set; }
    }
    
    public class FlagReviewRequest
    {
        [Required(ErrorMessage = "Reason for flagging is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Reason must be between 10 and 500 characters")]
        public string Reason { get; set; } = string.Empty;
    }
    
    // ============ RESPONSE DTOs ============
    
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int PlaceId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime? VisitDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsApproved { get; set; }
        public bool IsFlagged { get; set; }
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        public bool? CurrentUserMarkedHelpful { get; set; }
        public double? SentimentScore { get; set; }
        public List<string>? Images { get; set; }
    }
    
    public class ReviewStatisticsDto
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        public int ReviewsLast30Days { get; set; }
        public double AverageSentimentScore { get; set; }
        public int VerifiedVisitCount { get; set; }
        public int TotalHelpfulVotes { get; set; }
    }
    
    public class PaginatedReviewsResult
    {
        public List<ReviewDto> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public ReviewStatisticsDto? Statistics { get; set; }
    }
}
