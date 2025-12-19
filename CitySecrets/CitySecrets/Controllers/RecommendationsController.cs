using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // 🤖 RECOMMENDATIONS CONTROLLER
    // This provides smart place suggestions using algorithms
    // Helps users discover new places based on their tastes and what's popular
    /// <summary>
    /// Controller for AI-powered place recommendations and discovery
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        // Connection to recommendation service (handles all the smart algorithms)
        private readonly IRecommendationService _recommendationService;
        
        // Constructor: Sets up the recommendation service
        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        // 🎯 GET: api/recommendations/personalized
        // What it does: Suggests places YOU might like
        // How: Analyzes your favorites, reviews, and preferences
        // Why: Makes it easy to find new places that match your taste
        // Requires: User must be logged in
        // Returns: List of places scored by how well they match you
        /// <summary>
        /// Get personalized recommendations based on user preferences and behavior
        /// </summary>
        [HttpGet("personalized")]
        [Authorize]  // Must be logged in
        [ProducesResponseType(typeof(List<PersonalizedRecommendationDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetPersonalizedRecommendations([FromQuery] PersonalizedRecommendationRequest request)
        {
            // Check if request parameters are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            try
            {
                // Get recommendations tailored for this user
                var recommendations = await _recommendationService.GetPersonalizedRecommendationsAsync(userId, request);
                
                return Ok(new
                {
                    recommendations,
                    count = recommendations.Count,
                    basedOn = "Your preferences, favorites, and review history"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get recommendations: {ex.Message}" });
            }
        }

        // 🔥 GET: api/recommendations/trending
        // What it does: Shows what places are popular right now
        // How: Looks at recent views, reviews, favorites, and ratings
        // Why: Discover hot spots and places everyone is talking about
        // Needs: count (how many to show, default 10, max 50)
        // Returns: List of trending places with trending scores
        /// <summary>
        /// Get currently trending places based on recent activity
        /// </summary>
        [HttpGet("trending")]
        [ProducesResponseType(typeof(List<TrendingPlaceDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTrendingPlaces([FromQuery] int count = 10)
        {
            // Make sure count is reasonable
            if (count < 1 || count > 50)
                return BadRequest(new { message = "Count must be between 1 and 50" });

            try
            {
                // Get the top trending places from the last 7 days
                var trending = await _recommendationService.GetTrendingPlacesAsync(count);
                
                return Ok(new
                {
                    trending,
                    count = trending.Count,
                    period = "Last 7 days",
                    basedOn = "Views, reviews, favorites, and ratings"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get trending places: {ex.Message}" });
            }
        }

        // 💎 GET: api/recommendations/hidden-gems
        // What it does: Finds awesome places that most people don't know about
        // How: Looks for high-rated places with fewer views/reviews
        // Why: Discover secret spots before they get crowded
        // Needs: location (latitude/longitude) and search radius
        // Returns: Hidden gem places near you
        /// <summary>
        /// Discover hidden gems near a location
        /// </summary>
        [HttpGet("hidden-gems")]
        [ProducesResponseType(typeof(List<RecommendationDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetHiddenGems([FromQuery] HiddenGemSearchRequest request)
        {
            // Check if request parameters are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Find hidden gems in the area
                var hiddenGems = await _recommendationService.GetHiddenGemsAsync(request);
                
                return Ok(new
                {
                    hiddenGems,
                    count = hiddenGems.Count,
                    radius = $"{request.RadiusKm}km",
                    minScore = request.MinHiddenGemScore ?? 50
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get hidden gems: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get places similar to a specific place
        /// </summary>
        [HttpGet("similar/{placeId}")]
        [ProducesResponseType(typeof(List<RecommendationDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSimilarPlaces(int placeId, [FromQuery] int count = 5)
        {
            if (count < 1 || count > 20)
                return BadRequest(new { message = "Count must be between 1 and 20" });

            try
            {
                var similar = await _recommendationService.GetSimilarPlacesAsync(placeId, count);
                
                if (!similar.Any())
                    return NotFound(new { message = "No similar places found or place doesn't exist" });
                
                return Ok(new
                {
                    similar,
                    count = similar.Count,
                    basedOn = "Category, price range, rating, and location"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get similar places: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get popular places in a specific category
        /// </summary>
        [HttpGet("popular/{categoryId}")]
        [ProducesResponseType(typeof(List<RecommendationDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetPopularPlaces(
            int categoryId, 
            [FromQuery] int count = 10,
            [FromQuery] double? latitude = null,
            [FromQuery] double? longitude = null)
        {
            if (count < 1 || count > 50)
                return BadRequest(new { message = "Count must be between 1 and 50" });

            try
            {
                var popular = await _recommendationService.GetPopularPlacesByCategoryAsync(categoryId, count, latitude, longitude);
                
                return Ok(new
                {
                    popular,
                    count = popular.Count,
                    categoryId,
                    basedOn = "Views, reviews, favorites (last 30 days)"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get popular places: {ex.Message}" });
            }
        }

        /// <summary>
        /// Explore recommendations with custom filters
        /// </summary>
        [HttpGet("explore")]
        [ProducesResponseType(typeof(RecommendationResultDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetExploreRecommendations([FromQuery] RecommendationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : null;

                var result = await _recommendationService.GetExploreRecommendationsAsync(request, userId);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get explore recommendations: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get nearby places within a radius
        /// </summary>
        [HttpGet("nearby")]
        [ProducesResponseType(typeof(List<RecommendationDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetNearbyPlaces(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 5.0,
            [FromQuery] int count = 10)
        {
            if (latitude < -90 || latitude > 90)
                return BadRequest(new { message = "Latitude must be between -90 and 90" });

            if (longitude < -180 || longitude > 180)
                return BadRequest(new { message = "Longitude must be between -180 and 180" });

            if (radiusKm < 1 || radiusKm > 50)
                return BadRequest(new { message = "Radius must be between 1 and 50 km" });

            if (count < 1 || count > 50)
                return BadRequest(new { message = "Count must be between 1 and 50" });

            try
            {
                var nearby = await _recommendationService.GetNearbyRecommendationsAsync(latitude, longitude, radiusKm, count);
                
                return Ok(new
                {
                    nearby,
                    count = nearby.Count,
                    center = new { latitude, longitude },
                    radius = $"{radiusKm}km"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get nearby places: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get recommendation statistics for an area
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(RecommendationStatsDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetRecommendationStats(
            [FromQuery] double? latitude = null,
            [FromQuery] double? longitude = null,
            [FromQuery] double? radiusKm = null)
        {
            if (latitude.HasValue && (latitude.Value < -90 || latitude.Value > 90))
                return BadRequest(new { message = "Latitude must be between -90 and 90" });

            if (longitude.HasValue && (longitude.Value < -180 || longitude.Value > 180))
                return BadRequest(new { message = "Longitude must be between -180 and 180" });

            if (radiusKm.HasValue && (radiusKm.Value < 1 || radiusKm.Value > 100))
                return BadRequest(new { message = "Radius must be between 1 and 100 km" });

            try
            {
                var stats = await _recommendationService.GetRecommendationStatsAsync(latitude, longitude, radiusKm);
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to get stats: {ex.Message}" });
            }
        }
    }
}