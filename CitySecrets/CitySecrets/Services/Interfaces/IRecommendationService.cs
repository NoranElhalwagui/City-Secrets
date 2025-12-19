using CitySecrets.Models;
using CitySecrets.DTOs;

namespace CitySecrets.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<PersonalizedRecommendationDto>> GetPersonalizedRecommendationsAsync(
            int userId, PersonalizedRecommendationRequest request);
        
        Task<List<TrendingPlaceDto>> GetTrendingPlacesAsync(int count);
        
        Task<List<RecommendationDto>> GetHiddenGemsAsync(HiddenGemSearchRequest request);
        
        Task<List<RecommendationDto>> GetSimilarPlacesAsync(int placeId, int count);
        
        Task<List<RecommendationDto>> GetPopularPlacesByCategoryAsync(int categoryId, int count, double? latitude, double? longitude);
        
        Task<RecommendationResultDto> GetExploreRecommendationsAsync(RecommendationRequest request, int? userId);
        
        Task<List<RecommendationDto>> GetNearbyRecommendationsAsync(double latitude, double longitude, double radiusKm, int count);
        
        Task<RecommendationStatsDto> GetRecommendationStatsAsync(double? latitude, double? longitude, double? radiusKm);
        
        double CalculateRecommendationScore(Place place, int? userId, double? userLatitude, double? userLongitude);
    }
}
