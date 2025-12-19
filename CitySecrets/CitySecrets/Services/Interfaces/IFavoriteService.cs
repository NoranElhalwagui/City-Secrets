using CitySecrets.Models;
using CitySecrets.DTOs;

namespace CitySecrets.Services.Interfaces
{
    public interface IFavoriteService
    {
        // Get user's favorite places with optional filtering by list name
        PaginatedFavoritesResult GetUserFavorites(int userId, string? listName, int page, int pageSize);
        
        // Add a place to favorites
        FavoriteDto AddFavorite(int userId, int placeId, string? listName, string? notes);
        
        // Remove a place from favorites
        bool RemoveFavorite(int userId, int placeId);
        
        // Check if a place is favorited by user
        bool IsFavorite(int userId, int placeId);
        
        // Get all favorite list names for a user
        List<FavoriteListDto> GetFavoriteLists(int userId);
        
        // Update notes for a favorite
        bool UpdateFavoriteNotes(int favoriteId, int userId, string notes);
        
        // Move a favorite to a different list
        bool MoveFavoriteToList(int favoriteId, int userId, string newListName);
    }
}
