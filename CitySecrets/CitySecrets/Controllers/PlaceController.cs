using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using CitySecrets.Models;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;
        private readonly AppDbContext _context;
        
        public PlacesController(IPlaceService placeService, AppDbContext context)
        {
            _placeService = placeService;
            _context = context;
        }

        /// <summary>
        /// Get all places with filtering, sorting, and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPlaces([FromQuery] PlaceFilterRequest filters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var result = await GetFilteredPlacesAsync(filters, userId);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific place by ID and track view
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaceById(int id)
        {
            var place = await _placeService.GetPlaceByIdAsync(id);
            
            if (place == null)
                return NotFound(new { message = "Place not found" });

            var userId = GetCurrentUserId();
            var placeDto = await MapPlaceToDto(place, userId);
            
            // Track place view
            await TrackPlaceView(id, userId);
            
            return Ok(placeDto);
        }

        /// <summary>
        /// Create a new place (requires authentication)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePlace([FromBody] CreatePlaceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

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
                CreatedByUserId = userId
            };

            var createdPlace = await _placeService.CreatePlaceAsync(place, userId);
            var placeDto = await MapPlaceToDto(createdPlace, userId);
            
            return CreatedAtAction(nameof(GetPlaceById), new { id = createdPlace.PlaceId }, placeDto);
        }

        /// <summary>
        /// Update an existing place (owner or admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePlace(int id, [FromBody] UpdatePlaceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var existingPlace = await _placeService.GetPlaceByIdAsync(id);
            if (existingPlace == null)
                return NotFound(new { message = "Place not found" });

            // Check if user is owner or admin
            var isAdmin = User.IsInRole("Admin");
            if (existingPlace.CreatedByUserId != userId && !isAdmin)
                return Forbid();

            // Update fields
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

            var updatedPlace = await _placeService.UpdatePlaceAsync(id, existingPlace, userId);
            var placeDto = await MapPlaceToDto(updatedPlace!, userId);
            
            return Ok(placeDto);
        }

        /// <summary>
        /// Delete a place (soft delete, admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlace(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var result = await _placeService.DeletePlaceAsync(id, userId);
            
            if (!result)
                return NotFound(new { message = "Place not found" });
            
            return Ok(new { message = "Place deleted successfully" });
        }

        /// <summary>
        /// Search places by keyword
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchPlaces([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return BadRequest(new { message = "Search query must be at least 2 characters" });

            var places = await _placeService.SearchPlacesAsync(query);
            var userId = GetCurrentUserId();
            var placeDtos = new List<PlaceDto>();
            
            foreach (var place in places)
            {
                placeDtos.Add(await MapPlaceToDto(place, userId));
            }
            
            return Ok(placeDtos);
        }

        /// <summary>
        /// Get places near a specific location
        /// </summary>
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyPlaces(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 5)
        {
            if (radiusKm <= 0 || radiusKm > 100)
                return BadRequest(new { message = "Radius must be between 0 and 100 km" });

            var places = await _placeService.GetNearbyPlacesAsync(latitude, longitude, radiusKm);
            var userId = GetCurrentUserId();
            var placeDtos = new List<PlaceDto>();
            
            foreach (var place in places)
            {
                var dto = await MapPlaceToDto(place, userId);
                dto.DistanceKm = CalculateDistance(latitude, longitude, place.Latitude, place.Longitude);
                placeDtos.Add(dto);
            }
            
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