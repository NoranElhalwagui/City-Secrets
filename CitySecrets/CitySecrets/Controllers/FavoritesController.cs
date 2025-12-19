using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // ❤️ FAVORITES CONTROLLER
    // This handles user's favorite places (like bookmarks or saved items)
    // Users can save places, organize them into lists, and add personal notes
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // All favorite operations require login
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        
        // Constructor: Sets up the favorite service
        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public IActionResult GetUserFavorites(
            [FromQuery] string? listName = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var favorites = _favoriteService.GetUserFavorites(userId, listName, page, pageSize);
            return Ok(favorites);
        }

        // ➕ POST: api/favorites
        // What it does: Saves a place to your favorites
        // Why: Bookmark places you like or want to visit later
        // Needs: placeId, optional listName (to organize), optional notes
        // Example: Save "Joe's Pizza" to "Best Pizza" list with note "Try the margherita"
        [HttpPost]
        public IActionResult AddFavorite([FromBody] AddFavoriteRequest request)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            try
            {
                // Add the place to favorites
                var favorite = _favoriteService.AddFavorite(userId, request.PlaceId, request.ListName, request.Notes);
                return CreatedAtAction(nameof(GetUserFavorites), favorite);
            }
            catch (InvalidOperationException ex)
            {
                // If place doesn't exist or already favorited
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🗑️ DELETE: api/favorites/{placeId}
        // What it does: Removes a place from your favorites
        // Why: Unfavorite places you're no longer interested in
        // Needs: placeId in the URL
        [HttpDelete("{placeId}")]
        public IActionResult RemoveFavorite(int placeId)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Remove the place from favorites
            var result = _favoriteService.RemoveFavorite(userId, placeId);
            
            // If place wasn't in favorites
            if (!result)
                return NotFound(new { message = "Favorite not found" });
            
            return Ok(new { message = "Removed from favorites" });
        }

        // ✅ GET: api/favorites/check/{placeId}
        // What it does: Checks if a place is in your favorites
        // Why: Shows heart icon as filled/empty on place details page
        // Returns: { isFavorite: true } or { isFavorite: false }
        [HttpGet("check/{placeId}")]
        public IActionResult CheckIfFavorite(int placeId)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Check if place is favorited
            var isFavorite = _favoriteService.IsFavorite(userId, placeId);
            return Ok(new { isFavorite });
        }

        // 📂 GET: api/favorites/lists
        // What it does: Gets all your favorite list names
        // Why: Shows organized categories (e.g., "Want to Visit", "Date Night Spots")
        // Returns: List names with count of places in each
        [HttpGet("lists")]
        public IActionResult GetFavoriteLists()
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Get all list names and counts
            var lists = _favoriteService.GetFavoriteLists(userId);
            return Ok(lists);
        }

        // ✏️ PUT: api/favorites/{favoriteId}/notes
        // What it does: Updates your personal notes for a favorite place
        // Why: Add reminders like "Try the special on Tuesdays" or "Ask for window seat"
        // Needs: favoriteId in URL, new notes in body
        [HttpPut("{favoriteId}/notes")]
        public IActionResult UpdateFavoriteNotes(int favoriteId, [FromBody] UpdateNotesRequest request)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Update the notes
            var result = _favoriteService.UpdateFavoriteNotes(favoriteId, userId, request.Notes);
            
            // If favorite not found
            if (!result)
                return NotFound(new { message = "Favorite not found" });
            
            return Ok(new { message = "Notes updated successfully" });
        }

        // 📁 PUT: api/favorites/{favoriteId}/move
        // What it does: Moves a favorite to a different list
        // Why: Reorganize your favorites (e.g., move from "Want to Visit" to "Been There")
        // Needs: favoriteId in URL, newListName in body
        [HttpPut("{favoriteId}/move")]
        public IActionResult MoveFavoriteToList(int favoriteId, [FromBody] MoveToListRequest request)
        {
            // Get the user ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Move to new list
            var result = _favoriteService.MoveFavoriteToList(favoriteId, userId, request.NewListName);
            
            // If favorite not found
            if (!result)
                return NotFound(new { message = "Favorite not found" });
            
            return Ok(new { message = "Favorite moved successfully" });
        }
    }

    public class AddFavoriteRequest
    {
        public int PlaceId { get; set; }
        public string? ListName { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateNotesRequest
    {
        public string Notes { get; set; } = "";
    }

    public class MoveToListRequest
    {
        public string NewListName { get; set; } = "";
    }
}