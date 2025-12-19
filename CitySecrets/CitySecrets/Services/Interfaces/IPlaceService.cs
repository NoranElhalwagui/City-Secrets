using CitySecrets.Models;

namespace CitySecrets.Services.Interfaces
{
    public interface IPlaceService
    {
        Task<IEnumerable<Place>> GetAllPlacesAsync();
        Task<Place?> GetPlaceByIdAsync(int placeId);
        Task<Place> CreatePlaceAsync(Place place, int userId);
        Task<Place?> UpdatePlaceAsync(int placeId, Place updatedPlace, int userId);
        Task<bool> DeletePlaceAsync(int placeId, int adminUserId);
        Task<bool> VerifyPlaceAsync(int placeId, int adminUserId);
        Task<IEnumerable<Place>> SearchPlacesAsync(string keyword);
        Task<IEnumerable<Place>> GetNearbyPlacesAsync(double latitude, double longitude, double radiusKm);
        Task<IEnumerable<Place>> GetPlacesByCategoryAsync(int categoryId);
        Task RecalculatePlaceScoresAsync(int placeId);
    }
}
