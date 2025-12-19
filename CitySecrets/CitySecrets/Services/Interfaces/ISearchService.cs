using CitySecrets.DTOs;
using System.Collections.Generic;

namespace CitySecrets.Services.Interfaces
{
    public interface ISearchService
    {
        SearchResultDto Search(string query, int? userId, int page, int pageSize);
        SearchResultDto AdvancedSearch(AdvancedSearchRequest request, int? userId);
        List<SearchSuggestionDto> GetSearchSuggestions(string query, int limit);
        List<SearchHistoryDto> GetUserSearchHistory(int userId, int count);
        List<TrendingSearchDto> GetTrendingSearches(int count);
        void ClearUserSearchHistory(int userId);
        bool DeleteSearchHistoryItem(int searchId, int userId);
        SearchResultDto SearchNearby(double latitude, double longitude, double radiusKm, string? query, int? categoryId, int page, int pageSize);
        SearchFiltersDto GetAvailableFilters();
    }
}
