using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using System.Collections.Generic;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // 🔍 SEARCH CONTROLLER
    // This handles all search functionality (places, advanced filters, location-based)
    // Most searches work without login, but history requires authentication
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        
        // Constructor: Sets up the search service
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // 🔎 GET: api/search?query=cafe&page=1&pageSize=20
        // What it does: Basic search for places
        // Why: Find places by name, description, category, address, or city
        // Returns: Places with ratings, distance, and search time
        // Optional: Works for both guests and logged-in users
        [HttpGet]
        public IActionResult Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            // Make sure search query is provided
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Search query is required" });
            
            // Get current user ID if logged in (for favorites and history)
            var userId = GetCurrentUserId();
            
            // Perform search
            var result = _searchService.Search(query, userId, page, pageSize);
            return Ok(result);
        }

        // 🎯 POST: api/search/advanced
        // What it does: Advanced search with multiple filters
        // Why: Filter by category, price, rating, location, amenities
        // Needs: Body with filters (all optional)
        // Features: Sort by rating, distance, price, or popularity
        [HttpPost("advanced")]
        public IActionResult AdvancedSearch([FromBody] AdvancedSearchRequest request)
        {
            // Get current user ID if logged in
            var userId = GetCurrentUserId();
            
            // Perform advanced search with all filters
            var result = _searchService.AdvancedSearch(request, userId);
            return Ok(result);
        }

        // 💡 GET: api/search/suggestions?query=cof&limit=10
        // What it does: Gets search suggestions as you type
        // Why: Helps users find places faster (autocomplete)
        // Returns: Place names, categories, and popular searches
        // Minimum: 2 characters to start showing suggestions
        [HttpGet("suggestions")]
        public IActionResult GetSearchSuggestions([FromQuery] string query, [FromQuery] int limit = 10)
        {
            // Need at least 2 characters for suggestions
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Ok(new List<SearchSuggestionDto>());
            
            // Get suggestions from places, categories, and popular searches
            var suggestions = _searchService.GetSearchSuggestions(query, limit);
            return Ok(suggestions);
        }

        // 📜 GET: api/search/history?count=10
        // What it does: Gets your recent searches
        // Why: Quick access to previous searches
        // Returns: Search queries with result counts and time ago
        // Auth Required: Must be logged in
        [HttpGet("history")]
        [Authorize]
        public IActionResult GetUserSearchHistory([FromQuery] int count = 10)
        {
            // Get user ID from login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Get user's search history
            var history = _searchService.GetUserSearchHistory(userId, count);
            return Ok(history);
        }

        // 🔥 GET: api/search/trending?count=10
        // What it does: Gets currently trending searches
        // Why: Show what's popular and discover new places
        // Returns: Popular searches with trend direction (↑ up, ↓ down, → stable)
        // Features: Shows percentage change from last week
        [HttpGet("trending")]
        public IActionResult GetTrendingSearches([FromQuery] int count = 10)
        {
            // Get trending searches based on last 7 days
            var trending = _searchService.GetTrendingSearches(count);
            return Ok(trending);
        }

        // 🧹 DELETE: api/search/history
        // What it does: Clears all your search history
        // Why: Privacy - remove all past searches
        // Auth Required: Must be logged in
        [HttpDelete("history")]
        [Authorize]
        public IActionResult ClearSearchHistory()
        {
            // Get user ID from login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Clear all search history
            _searchService.ClearUserSearchHistory(userId);
            return Ok(new { message = "Search history cleared successfully" });
        }

        // 🗑️ DELETE: api/search/history/{searchId}
        // What it does: Deletes one search history item
        // Why: Remove specific searches you don't want in history
        // Needs: searchId in URL
        // Auth Required: Must be logged in
        [HttpDelete("history/{searchId}")]
        [Authorize]
        public IActionResult DeleteSearchHistoryItem(int searchId)
        {
            // Get user ID from login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Delete the specific search history item
            var result = _searchService.DeleteSearchHistoryItem(searchId, userId);
            
            // If search history item not found
            if (!result)
                return NotFound(new { message = "Search history item not found" });
            
            return Ok(new { message = "Search history item deleted" });
        }

        // 📍 GET: api/search/nearby?latitude=40.7&longitude=-74&radiusKm=5
        // What it does: Finds places near a location
        // Why: \"Find places near me\" or search around any location
        // Needs: latitude, longitude (optional: radius, query, category)
        // Returns: Places sorted by distance from your location
        // Default: 5km radius
        [HttpGet("nearby")]
        public IActionResult SearchNearby(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 5,
            [FromQuery] string? query = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // Search for places within radius
            var result = _searchService.SearchNearby(latitude, longitude, radiusKm, query, categoryId, page, pageSize);
            return Ok(result);
        }

        // ⚙️ GET: api/search/filters
        // What it does: Gets all available search filters
        // Why: Show filter options in UI (categories, price range, cities)
        // Returns: Categories with place counts, min/max prices, available cities
        // Use case: Build filter UI dynamically
        [HttpGet("filters")]
        public IActionResult GetAvailableFilters()
        {
            // Get all available filters for the UI
            var filters = _searchService.GetAvailableFilters();
            return Ok(filters);
        }

        // Helper: Gets current user ID if logged in
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? int.Parse(userIdClaim) : null;
        }
    }
}