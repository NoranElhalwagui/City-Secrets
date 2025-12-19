using System;
using System.Collections.Generic;

namespace CitySecrets.DTOs
{
    // Response for search results with pagination
    public class SearchResultDto
    {
        public List<PlaceSearchResultDto> Places { get; set; } = new();
        public int TotalResults { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SearchQuery { get; set; }
        public double SearchTimeMs { get; set; }
    }

    // Individual place in search results (lighter than full PlaceDto)
    public class PlaceSearchResultDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? MainImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public decimal? PriceRange { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? DistanceKm { get; set; }  // Distance from search location
        public bool IsHiddenGem { get; set; }
        public bool IsFavorited { get; set; }  // For logged-in users
        public string? Address { get; set; }
        public string? City { get; set; }
    }

    // Advanced search request (moved from controller)
    public class AdvancedSearchRequest
    {
        public string? Query { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusKm { get; set; }
        public bool? IsHiddenGem { get; set; }
        public List<string>? Amenities { get; set; }
        public string? SortBy { get; set; }  // "rating", "distance", "price", "popular"
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool? IsOpen { get; set; }  // Currently open
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // Search suggestion with additional info
    public class SearchSuggestionDto
    {
        public string Query { get; set; } = string.Empty;
        public string Type { get; set; } = "place";  // "place", "category", "location"
        public int? EntityId { get; set; }  // PlaceId or CategoryId
        public string? Icon { get; set; }
        public int SearchCount { get; set; }  // How many times searched
    }

    // Search history item
    public class SearchHistoryDto
    {
        public int SearchId { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public int ResultCount { get; set; }
        public DateTime SearchedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public string? Location { get; set; }  // If location was used
        public string? AppliedFilters { get; set; }  // Summary of filters
    }

    // Trending search item
    public class TrendingSearchDto
    {
        public string Query { get; set; } = string.Empty;
        public int SearchCount { get; set; }
        public string TrendDirection { get; set; } = "up";  // "up", "down", "stable"
        public double TrendPercentage { get; set; }  // % change from previous period
        public string? Category { get; set; }
    }

    // Nearby search request
    public class NearbySearchRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusKm { get; set; } = 5;
        public string? Query { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinRating { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // Search filters summary (for UI display)
    public class SearchFiltersDto
    {
        public List<CategoryFilterDto> AvailableCategories { get; set; } = new();
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<string> AvailableAmenities { get; set; } = new();
        public List<string> AvailableCities { get; set; } = new();
    }

    public class CategoryFilterDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PlaceCount { get; set; }
    }
}
