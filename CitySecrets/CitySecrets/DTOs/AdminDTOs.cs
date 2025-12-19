using System.ComponentModel.DataAnnotations;

namespace CitySecrets.DTOs
{
    // ============ REQUEST DTOs ============
    
    public class HiddenGemRequest
    {
        [Required]
        public bool IsHiddenGem { get; set; }
        
        [Range(0, 100, ErrorMessage = "Hidden Gem Score must be between 0 and 100")]
        public int HiddenGemScore { get; set; }
    }
    
    public class RejectRequest
    {
        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;
    }
    
    public class BanRequest
    {
        [Required(ErrorMessage = "Reason for ban is required")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;
    }
    
    public class BulkDeleteRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one Place ID is required")]
        public List<int> PlaceIds { get; set; } = new();
    }
    
    public class BulkApproveRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one Review ID is required")]
        public List<int> ReviewIds { get; set; } = new();
    }
    
    public class CreatePlaceAdminRequest
    {
        [Required(ErrorMessage = "Place name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double Latitude { get; set; }
        
        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double Longitude { get; set; }
        
        public decimal? AveragePrice { get; set; }
        
        [StringLength(50)]
        public string? PriceRange { get; set; }
        
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [Url]
        public string? Website { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? OpeningHours { get; set; }
        
        public bool IsHiddenGem { get; set; }
        
        [Range(0, 100)]
        public int HiddenGemScore { get; set; }
    }
    
    public class UpdatePlaceAdminRequest
    {
        [Required(ErrorMessage = "Place name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double Latitude { get; set; }
        
        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double Longitude { get; set; }
        
        public decimal? AveragePrice { get; set; }
        
        [StringLength(50)]
        public string? PriceRange { get; set; }
        
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [Url]
        public string? Website { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? OpeningHours { get; set; }
        
        public bool IsHiddenGem { get; set; }
        
        [Range(0, 100)]
        public int HiddenGemScore { get; set; }
    }
    
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        
        [Url]
        public string? IconUrl { get; set; }
    }
    
    public class UpdateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        
        [Url]
        public string? IconUrl { get; set; }
    }
    
    // ============ RESPONSE DTOs ============
    
    public class DashboardStatsDto
    {
        public PlaceStatsDto Places { get; set; } = new();
        public UserStatsDto Users { get; set; } = new();
        public ReviewStatsDto Reviews { get; set; } = new();
        public int TotalCategories { get; set; }
    }
    
    public class PlaceStatsDto
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Deleted { get; set; }
        public int HiddenGems { get; set; }
    }
    
    public class UserStatsDto
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Banned { get; set; }
    }
    
    public class ReviewStatsDto
    {
        public int Total { get; set; }
        public int Flagged { get; set; }
    }
    
    public class SystemHealthDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public DatabaseHealthDto Database { get; set; } = new();
        public ActivityHealthDto Activity { get; set; } = new();
        public IssuesDto Issues { get; set; } = new();
    }
    
    public class DatabaseHealthDto
    {
        public bool Connected { get; set; }
        public RecordCountsDto TotalRecords { get; set; } = new();
    }
    
    public class RecordCountsDto
    {
        public int Places { get; set; }
        public int Users { get; set; }
        public int Reviews { get; set; }
        public int Categories { get; set; }
    }
    
    public class ActivityHealthDto
    {
        public PeriodActivityDto Last24Hours { get; set; } = new();
        public PeriodActivityDto Last7Days { get; set; } = new();
    }
    
    public class PeriodActivityDto
    {
        public int NewPlaces { get; set; }
        public int NewReviews { get; set; }
        public int NewUsers { get; set; }
        public int? AdminActions { get; set; }
    }
    
    public class IssuesDto
    {
        public int PendingPlaces { get; set; }
        public int FlaggedReviews { get; set; }
        public int InactiveUsers { get; set; }
    }
    
    public class BulkOperationResult
    {
        public int SuccessCount { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<int> ProcessedIds { get; set; } = new();
    }
}
