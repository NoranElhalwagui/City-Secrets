using CitySecrets.Models;
using CitySecrets.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitySecrets.Services
{
    public class SearchService : ISearchService
    {
        private readonly List<Place> _places = new();
        private readonly List<SearchHistory> _searchHistory = new();

        public async Task<IEnumerable<Place>> SearchAsync(string query, int? userId)
        {
            if (userId.HasValue)
            {
                _searchHistory.Add(new SearchHistory
                {
                    UserId = userId.Value,
                    SearchQuery = query, 
                    SearchedAt = DateTime.UtcNow 
                });
            }

            return _places.Where(p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Place>> AdvancedSearchAsync(
            int? categoryId,
            double? minPrice,
            double? maxPrice,
            double? minRating,
            double? maxDistanceKm)
        {
            var result = _places.AsQueryable();

            if (categoryId.HasValue)
                result = result.Where(p => p.CategoryId == categoryId);

            if (minPrice.HasValue)
                result = result.Where(p => p.PriceRange >= minPrice);

            if (maxPrice.HasValue)
                result = result.Where(p => p.PriceRange <= maxPrice);

            if (minRating.HasValue)
                result = result.Where(p => p.PriceRange >= minRating);

            return result.ToList();
        }

        public async Task<IEnumerable<string>> GetSearchSuggestionsAsync(string partialQuery)
        {
            return _places
                .Where(p => p.Name.StartsWith(partialQuery, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Name)
                .Distinct()
                .Take(10);
        }

        public async Task<IEnumerable<SearchHistory>> GetUserSearchHistoryAsync(int userId, int count = 10)
        {
            return _searchHistory
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SearchedAt) // Fix: Adjusted to match the correct property name
                .Take(count);
        }

        public async Task<IEnumerable<string>> GetTrendingSearchesAsync(int count = 10)
        {
            return _searchHistory
                .GroupBy(h => h.SearchQuery) 
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key);
        }
    }
}
