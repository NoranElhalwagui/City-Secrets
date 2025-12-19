using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using CitySecrets.Models;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // 📍 PLACES CONTROLLER
    // This handles all operations related to places (restaurants, cafes, attractions, etc.)
    // Users can view places, logged-in users can add/edit their own places
    [ApiController]
    [Route("api/[controller]")]
    public class PlacesController : ControllerBase
    {
        // Connections to services and database
        private readonly IPlaceService _placeService;
        private readonly AppDbContext _context;
        
        // Constructor: Sets up the place service and database connection
        public PlacesController(IPlaceService placeService, AppDbContext context)
        {
            _placeService = placeService;
            _context = context;
        }

        // 🔍 GET: api/places
        // What it does: Gets a list of all places with filters
        // Why: Main search/browse endpoint for discovering places
        // Can filter by: category, price, location, rating, etc.
        // Returns: Paginated list of places
        /// <summary>
        /// Get all places with filtering, sorting, and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPlaces([FromQuery] PlaceFilterRequest filters)
        {
            // Check if filter parameters are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get current user ID (if logged in) to show favorites
            var userId = GetCurrentUserId();
            // Apply filters and get places
            var result = await GetFilteredPlacesAsync(filters, userId);
            return Ok(result);
        }

        // 🏛️ GET: api/places/{id}
        // What it does: Gets detailed info about one specific place
        // Why: Shows full details page for a place (description, reviews, photos, etc.)
        // Also tracks: Counts this as a "view" for analytics
        /// <summary>
        /// Get a specific place by ID and track view
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaceById(int id)
        {
            // Get the place from database
            var place = await _placeService.GetPlaceByIdAsync(id);
            
            // If place doesn't exist
            if (place == null)
                return NotFound(new { message = "Place not found" });

            // Get current user ID (if logged in)
            var userId = GetCurrentUserId();
            // Convert place to DTO format with user-specific data (is it favorited?)
            var placeDto = await MapPlaceToDto(place, userId);
            
            // Track that someone viewed this place (for trending/analytics)
            await TrackPlaceView(id, userId);
            
            return Ok(placeDto);
        }

        // ➕ POST: api/places
        // What it does: Creates a new place
        // Why: Lets users add new places to the app (like adding a hidden gem)
        // Requires: User must be logged in
        // Needs: name, description, address, location (lat/lng), category, etc.
        /// <summary>
        /// Create a new place (requires authentication)
        /// </summary>
        [HttpPost]
        [Authorize]  // Must be logged in
        public async Task<IActionResult> CreatePlace([FromBody] CreatePlaceRequest request)
        {
            // Check if all required fields are filled
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the user ID from their login token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // Create a new Place object with all the info
            var place = new Place
            {
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CategoryId = request.CategoryId,
                AveragePrice = request.AveragePrice ?? 0,
                PriceRange = request.PriceRange,
                PhoneNumber = request.PhoneNumber,
                Website = request.Website,
                Email = request.Email,
                OpeningHours = request.OpeningHours,
                CreatedByUserId = userId  // Track who added this place
            };

            // Save the place to database
            var createdPlace = await _placeService.CreatePlaceAsync(place, userId);
            // Convert to DTO format
            var placeDto = await MapPlaceToDto(createdPlace, userId);
            
            // Return the created place with its new ID
            return CreatedAtAction(nameof(GetPlaceById), new { id = createdPlace.PlaceId }, placeDto);
        }

        // ✏️ PUT: api/places/{id}
        // What it does: Updates an existing place's information
        // Why: Lets users edit their places or admins update any place
        // Requires: Must be logged in AND either be the place owner OR an admin
        // Needs: Updated info (any fields can be changed)
        /// <summary>
        /// Update an existing place (owner or admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]  // Must be logged in
        public async Task<IActionResult> UpdatePlace(int id, [FromBody] UpdatePlaceRequest request)
        {
            // Check if all fields are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the user ID from their login token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // Get the place from database
            var existingPlace = await _placeService.GetPlaceByIdAsync(id);
            if (existingPlace == null)
                return NotFound(new { message = "Place not found" });

            // Check if user is owner or admin
            var isAdmin = User.IsInRole("Admin");
            // Only place owner or admin can edit
            if (existingPlace.CreatedByUserId != userId && !isAdmin)
                return Forbid();

            // Update only the fields that were provided (others stay the same)
            if (request.Name != null) existingPlace.Name = request.Name;
            if (request.Description != null) existingPlace.Description = request.Description;
            if (request.Address != null) existingPlace.Address = request.Address;
            if (request.Latitude.HasValue) existingPlace.Latitude = request.Latitude.Value;
            if (request.Longitude.HasValue) existingPlace.Longitude = request.Longitude.Value;
            if (request.CategoryId.HasValue) existingPlace.CategoryId = request.CategoryId.Value;
            if (request.AveragePrice.HasValue) existingPlace.AveragePrice = request.AveragePrice.Value;
            if (request.PriceRange != null) existingPlace.PriceRange = request.PriceRange;
            if (request.PhoneNumber != null) existingPlace.PhoneNumber = request.PhoneNumber;
            if (request.Website != null) existingPlace.Website = request.Website;
            if (request.Email != null) existingPlace.Email = request.Email;
            if (request.OpeningHours != null) existingPlace.OpeningHours = request.OpeningHours;

            // Save the updated place
            var updatedPlace = await _placeService.UpdatePlaceAsync(id, existingPlace, userId);
            // Convert to DTO format
            var placeDto = await MapPlaceToDto(updatedPlace!, userId);
            
            return Ok(placeDto);
        }

        // 🗑️ DELETE: api/places/{id}
        // What it does: Deletes a place (soft delete - doesn't actually delete, just hides it)
        // Why: Admins can remove inappropriate or closed places
        // Requires: Must be admin only (regular users can't delete places)
        /// <summary>
        /// Delete a place (soft delete, admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Admin only
        public async Task<IActionResult> DeletePlace(int id)
        {
            // Get the user ID from their login token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // Soft delete the place (marks as deleted, doesn't remove from database)
            var result = await _placeService.DeletePlaceAsync(id, userId);
            
            // If place not found
            if (!result)
                return NotFound(new { message = "Place not found" });
            
            return Ok(new { message = "Place deleted successfully" });
        }

        // 🔎 GET: api/places/search?query=pizza
        // What it does: Searches for places by keyword
        // Why: Lets users find places by typing (like "pizza", "cafe", etc.)
        // Needs: search query (at least 2 characters)
        // Returns: All matching places
        /// <summary>
        /// Search places by keyword
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchPlaces([FromQuery] string query)
        {
            // Make sure search query is at least 2 characters
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return BadRequest(new { message = "Search query must be at least 2 characters" });

            // Search places by name, description, or address
            var places = await _placeService.SearchPlacesAsync(query);
            // Get current user ID (if logged in)
            var userId = GetCurrentUserId();
            var placeDtos = new List<PlaceDto>();
            
            // Convert each place to DTO format
            foreach (var place in places)
            {
                placeDtos.Add(await MapPlaceToDto(place, userId));
            }
            
            return Ok(placeDtos);
        }

        // 📍 GET: api/places/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=5
        // What it does: Finds places near a specific location
        // Why: Shows places around user's current location or any address
        // Needs: latitude, longitude, radius in kilometers (default 5km, max 100km)
        // Returns: Places within the radius, sorted by distance (closest first)
        /// <summary>
        /// Get places near a specific location
        /// </summary>
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyPlaces(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 5)
        {
            // Check if radius is reasonable (0-100 km)
            if (radiusKm <= 0 || radiusKm > 100)
                return BadRequest(new { message = "Radius must be between 0 and 100 km" });

            // Get all places within the radius
            var places = await _placeService.GetNearbyPlacesAsync(latitude, longitude, radiusKm);
            // Get current user ID (if logged in)
            var userId = GetCurrentUserId();
            var placeDtos = new List<PlaceDto>();
            
            // Convert each place to DTO and calculate exact distance
            foreach (var place in places)
            {
                var dto = await MapPlaceToDto(place, userId);
                // Calculate how far this place is from the search location
                dto.DistanceKm = CalculateDistance(latitude, longitude, place.Latitude, place.Longitude);
                placeDtos.Add(dto);
            }
            
            // Sort by distance (closest first) and return
            return Ok(placeDtos.OrderBy(p => p.DistanceKm));
        }

        /// <summary>
        /// Get places by category
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetPlacesByCategory(int categoryId)
        {
            var places = await _placeService.GetPlacesByCategoryAsync(categoryId);
            var userId = GetCurrentUserId();
            var placeDtos = new List<PlaceDto>();
            
            foreach (var place in places)
            {
                placeDtos.Add(await MapPlaceToDto(place, userId));
            }
            
            return Ok(placeDtos);
        }

        /// <summary>
        /// Get statistics for a specific place
        /// </summary>
        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetPlaceStatistics(int id)
        {
            var place = await _placeService.GetPlaceByIdAsync(id);
            if (place == null)
                return NotFound(new { message = "Place not found" });

            var stats = new PlaceStatisticsDto
            {
                PlaceId = place.PlaceId,
                PlaceName = place.Name,
                TotalViews = _context.PlaceViews.Count(pv => pv.PlaceId == id && !pv.IsDeleted),
                TotalReviews = place.TotalReviews,
                TotalFavorites = _context.Favourites.Count(f => f.PlaceId == id && !f.IsDeleted),
                AverageRating = place.AverageRating,
                Rating1Count = place.RatingCount1,
                Rating2Count = place.RatingCount2,
                Rating3Count = place.RatingCount3,
                Rating4Count = place.RatingCount4,
                Rating5Count = place.RatingCount5,
                SentimentScore = place.SentimentScore,
                PopularityScore = place.PopularityScore,
                VisitCount = place.VisitCount
            };
            
            return Ok(stats);
        }

        /// <summary>
        /// Verify a place (admin only)
        /// </summary>
        [HttpPost("{id}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyPlace(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var result = await _placeService.VerifyPlaceAsync(id, userId);
            
            if (!result)
                return NotFound(new { message = "Place not found" });
            
            return Ok(new { message = "Place verified successfully" });
        }

        // Helper methods

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null && int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        private async Task<PlaceDto> MapPlaceToDto(Place place, int? userId)
        {
            var category = await _context.Categories.FindAsync(place.CategoryId);
            var isFavorited = false;
            
            if (userId.HasValue)
            {
                isFavorited = await _context.Favourites
                    .AnyAsync(f => f.UserId == userId.Value && f.PlaceId == place.PlaceId && !f.IsDeleted);
            }

            return new PlaceDto
            {
                PlaceId = place.PlaceId,
                Name = place.Name,
                Description = place.Description ?? "",
                Address = place.Address,
                Latitude = place.Latitude,
                Longitude = place.Longitude,
                AveragePrice = place.AveragePrice,
                PriceRange = place.PriceRange,
                AverageRating = place.AverageRating,
                TotalReviews = place.TotalReviews,
                VisitCount = place.VisitCount,
                PopularityScore = place.PopularityScore,
                IsHiddenGem = place.IsHiddenGem,
                HiddenGemScore = place.HiddenGemScore,
                IsVerified = place.IsVerified,
                PhoneNumber = place.PhoneNumber,
                Website = place.Website,
                Email = place.Email,
                OpeningHours = place.OpeningHours,
                CategoryId = place.CategoryId,
                CategoryName = category?.Name,
                IsFavorited = isFavorited,
                CreatedAt = place.CreatedAt,
                UpdatedAt = place.UpdatedAt
            };
        }

        private async Task<PaginatedResult<PlaceDto>> GetFilteredPlacesAsync(PlaceFilterRequest filters, int? userId)
        {
            var query = _context.Place
                .Where(p => !p.IsDeleted && p.IsActive)
                .AsQueryable();

            // Apply filters
            if (filters.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filters.CategoryId.Value);
            
            if (filters.MinPrice.HasValue)
                query = query.Where(p => p.AveragePrice >= filters.MinPrice.Value);
            
            if (filters.MaxPrice.HasValue)
                query = query.Where(p => p.AveragePrice <= filters.MaxPrice.Value);
            
            if (filters.MinRating.HasValue)
                query = query.Where(p => p.AverageRating >= (double)filters.MinRating.Value);
            
            if (filters.IsHiddenGem.HasValue)
                query = query.Where(p => p.IsHiddenGem == filters.IsHiddenGem.Value);
            
            if (filters.IsVerified.HasValue)
                query = query.Where(p => p.IsVerified == filters.IsVerified.Value);

            // Apply sorting
            query = filters.SortBy?.ToLower() switch
            {
                "name" => filters.SortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "rating" => filters.SortOrder == "asc" ? query.OrderBy(p => p.AverageRating) : query.OrderByDescending(p => p.AverageRating),
                "price" => filters.SortOrder == "asc" ? query.OrderBy(p => p.AveragePrice) : query.OrderByDescending(p => p.AveragePrice),
                "distance" => query.OrderBy(p => p.PlaceId), // Would need lat/long for real distance
                _ => query.OrderByDescending(p => p.PopularityScore)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize);

            var places = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            var placeDtos = new List<PlaceDto>();
            foreach (var place in places)
            {
                placeDtos.Add(await MapPlaceToDto(place, userId));
            }

            return new PaginatedResult<PlaceDto>
            {
                Data = placeDtos,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPrevious = filters.Page > 1,
                HasNext = filters.Page < totalPages
            };
        }

        private async Task TrackPlaceView(int placeId, int? userId)
        {
            var placeView = new PlaceView
            {
                PlaceId = placeId,
                UserId = userId,
                ViewedAt = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            _context.PlaceViews.Add(placeView);
            await _context.SaveChangesAsync();
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth radius in km
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
    }
}