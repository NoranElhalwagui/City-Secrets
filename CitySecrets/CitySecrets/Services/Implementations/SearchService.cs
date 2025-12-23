using CitySecrets.Models;
using CitySecrets.DTOs;
using CitySecrets.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CitySecrets.Services
{
    public class SearchService : ISearchService
    {
        private readonly AppDbContext _context;

        public SearchService(AppDbContext context)
        {
            _context = context;
        }

        public SearchResultDto Search(string query, int? userId, int page, int pageSize)
        {
            var stopwatch = Stopwatch.StartNew();

            // Build the search query
            var placesQuery = _context.Place
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.IsVerified &&
                    (p.Name.Contains(query) ||
                     p.Description!.Contains(query) ||
                     p.Address!.Contains(query) ||
                     p.Category!.Name.Contains(query)))
                .AsQueryable();

            var totalResults = placesQuery.Count();

            // Get paginated results with ratings
            var places = placesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PlaceSearchResultDto
                {
                    PlaceId = p.PlaceId,
                    Name = p.Name,
                    Description = p.Description,
                    MainImageUrl = p.Images!.FirstOrDefault()!.ImageUrl,
                    CategoryName = p.Category!.Name,
                    CategoryId = p.CategoryId,
                    PriceRange = p.AveragePrice > 0 ? p.AveragePrice : null,
                    AverageRating = p.Reviews!.Any() ? p.Reviews!.Average(r => r.Rating) : 0,
                    ReviewCount = p.Reviews!.Count(),
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    IsHiddenGem = p.IsHiddenGem,
                    IsFavorited = userId.HasValue && p.Favorites!.Any(f => f.UserId == userId.Value),
                    Address = p.Address,
                    City = p.Address
                })
                .ToList();

            // Save search history if user is logged in
            if (userId.HasValue)
            {
                SaveSearchHistory(userId.Value, query, null, null, null, null, totalResults);
            }

            stopwatch.Stop();

            return new SearchResultDto
            {
                Places = places,
                TotalResults = totalResults,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalResults / pageSize),
                SearchQuery = query,
                SearchTimeMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }

        public SearchResultDto AdvancedSearch(AdvancedSearchRequest request, int? userId)
        {
            var stopwatch = Stopwatch.StartNew();

            var placesQuery = _context.Place
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => !p.IsDeleted && p.IsVerified)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                placesQuery = placesQuery.Where(p =>
                    p.Name.Contains(request.Query) ||
                    p.Description!.Contains(request.Query));
            }

            if (request.CategoryId.HasValue)
                placesQuery = placesQuery.Where(p => p.CategoryId == request.CategoryId);

            if (request.MinPrice.HasValue)
                placesQuery = placesQuery.Where(p => p.AveragePrice >= request.MinPrice);

            if (request.MaxPrice.HasValue)
                placesQuery = placesQuery.Where(p => p.AveragePrice <= request.MaxPrice);

            if (request.IsHiddenGem.HasValue)
                placesQuery = placesQuery.Where(p => p.IsHiddenGem == request.IsHiddenGem);

            // Location-based filtering
            List<PlaceSearchResultDto> places;
            if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
            {
                var allPlaces = placesQuery.ToList();
                places = allPlaces
                    .Select(p => new
                    {
                        Place = p,
                        Distance = CalculateDistance(request.Latitude.Value, request.Longitude.Value,
                            p.Latitude, p.Longitude)
                    })
                    .Where(x => x.Distance <= request.RadiusKm.Value)
                    .Select(x => MapToSearchResult(x.Place, x.Distance, userId))
                    .ToList();
            }
            else
            {
                places = placesQuery
                    .Select(p => MapToSearchResult(p, null, userId))
                    .ToList();
            }

            // Apply rating filter after mapping
            if (request.MinRating.HasValue)
                places = places.Where(p => p.AverageRating >= (double)request.MinRating.Value).ToList();

            // Sorting
            places = request.SortBy?.ToLower() switch
            {
                "rating" => places.OrderByDescending(p => p.AverageRating).ToList(),
                "distance" => places.OrderBy(p => p.DistanceKm ?? double.MaxValue).ToList(),
                "price" => places.OrderBy(p => p.PriceRange ?? decimal.MaxValue).ToList(),
                "popular" => places.OrderByDescending(p => p.ReviewCount).ToList(),
                _ => places.OrderByDescending(p => p.AverageRating).ToList()
            };

            var totalResults = places.Count;
            var paginatedPlaces = places
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Save search history
            if (userId.HasValue)
            {
                SaveSearchHistory(userId.Value, request.Query ?? "", request.CategoryId,
                    request.Latitude, request.Longitude, System.Text.Json.JsonSerializer.Serialize(request), totalResults);
            }

            stopwatch.Stop();

            return new SearchResultDto
            {
                Places = paginatedPlaces,
                TotalResults = totalResults,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalResults / request.PageSize),
                SearchQuery = request.Query,
                SearchTimeMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }

        public List<SearchSuggestionDto> GetSearchSuggestions(string query, int limit)
        {
            var suggestions = new List<SearchSuggestionDto>();

            // Place name suggestions
            var placeSuggestions = _context.Place
                .Where(p => !p.IsDeleted && p.IsVerified && p.Name.Contains(query))
                .Take(limit / 2)
                .Select(p => new SearchSuggestionDto
                {
                    Query = p.Name,
                    Type = "place",
                    EntityId = p.PlaceId,
                    Icon = "📍"
                })
                .ToList();

            suggestions.AddRange(placeSuggestions);

            // Category suggestions
            var categorySuggestions = _context.Categories
                .Where(c => !c.IsDeleted && c.Name.Contains(query))
                .Take(limit / 2)
                .Select(c => new SearchSuggestionDto
                {
                    Query = c.Name,
                    Type = "category",
                    EntityId = c.CategoryId,
                    Icon = c.IconUrl ?? "🏷️"
                })
                .ToList();

            suggestions.AddRange(categorySuggestions);

            // Popular searches matching query
            var popularSearches = _context.SearchHistories
                .Where(h => !h.IsDeleted && h.SearchQuery.Contains(query))
                .GroupBy(h => h.SearchQuery)
                .Select(g => new SearchSuggestionDto
                {
                    Query = g.Key,
                    Type = "popular",
                    SearchCount = g.Count(),
                    Icon = "🔥"
                })
                .OrderByDescending(s => s.SearchCount)
                .Take(limit / 3)
                .ToList();

            suggestions.AddRange(popularSearches);

            return suggestions.Take(limit).ToList();
        }

        public List<SearchHistoryDto> GetUserSearchHistory(int userId, int count)
        {
            return _context.SearchHistories
                .Include(h => h.Category)
                .Where(h => h.UserId == userId && !h.IsDeleted)
                .OrderByDescending(h => h.SearchedAt)
                .Take(count)
                .Select(h => new SearchHistoryDto
                {
                    SearchId = h.SearchId,
                    SearchQuery = h.SearchQuery,
                    CategoryName = h.Category != null ? h.Category.Name : null,
                    ResultCount = h.ResultCount,
                    SearchedAt = h.SearchedAt,
                    TimeAgo = GetTimeAgo(h.SearchedAt),
                    Location = h.Latitude.HasValue && h.Longitude.HasValue
                        ? $"{h.Latitude:F2}, {h.Longitude:F2}"
                        : null,
                    AppliedFilters = h.Filters
                })
                .ToList();
        }

        public List<TrendingSearchDto> GetTrendingSearches(int count)
        {
            var last7Days = DateTime.UtcNow.AddDays(-7);
            var last14Days = DateTime.UtcNow.AddDays(-14);

            var currentPeriod = _context.SearchHistories
                .Where(h => !h.IsDeleted && h.SearchedAt >= last7Days)
                .GroupBy(h => h.SearchQuery)
                .Select(g => new { Query = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Query, x => x.Count);

            var previousPeriod = _context.SearchHistories
                .Where(h => !h.IsDeleted && h.SearchedAt >= last14Days && h.SearchedAt < last7Days)
                .GroupBy(h => h.SearchQuery)
                .Select(g => new { Query = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Query, x => x.Count);

            var trending = currentPeriod
                .OrderByDescending(x => x.Value)
                .Take(count)
                .Select(x =>
                {
                    var previousCount = previousPeriod.ContainsKey(x.Key) ? previousPeriod[x.Key] : 0;
                    var trendPercentage = previousCount > 0
                        ? ((double)(x.Value - previousCount) / previousCount) * 100
                        : 100;

                    return new TrendingSearchDto
                    {
                        Query = x.Key,
                        SearchCount = x.Value,
                        TrendDirection = trendPercentage > 10 ? "up" : trendPercentage < -10 ? "down" : "stable",
                        TrendPercentage = Math.Round(trendPercentage, 1)
                    };
                })
                .ToList();

            return trending;
        }

        public void ClearUserSearchHistory(int userId)
        {
            var userHistory = _context.SearchHistories
                .Where(h => h.UserId == userId && !h.IsDeleted)
                .ToList();

            foreach (var history in userHistory)
            {
                history.IsDeleted = true;
            }

            _context.SaveChanges();
        }

        public bool DeleteSearchHistoryItem(int searchId, int userId)
        {
            var searchHistory = _context.SearchHistories
                .FirstOrDefault(h => h.SearchId == searchId && h.UserId == userId && !h.IsDeleted);

            if (searchHistory == null)
                return false;

            searchHistory.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }

        public SearchResultDto SearchNearby(double latitude, double longitude, double radiusKm,
            string? query, int? categoryId, int page, int pageSize)
        {
            var stopwatch = Stopwatch.StartNew();

            var placesQuery = _context.Place
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => !p.IsDeleted && p.IsVerified)
                .ToList(); // Load to memory for distance calculation

            var nearbyPlaces = placesQuery
                .Select(p => new
                {
                    Place = p,
                    Distance = CalculateDistance(latitude, longitude, p.Latitude, p.Longitude)
                })
                .Where(x => x.Distance <= radiusKm)
                .ToList();

            // Apply additional filters
            if (!string.IsNullOrWhiteSpace(query))
                nearbyPlaces = nearbyPlaces.Where(x => x.Place.Name.Contains(query)).ToList();

            if (categoryId.HasValue)
                nearbyPlaces = nearbyPlaces.Where(x => x.Place.CategoryId == categoryId).ToList();

            // Order by distance
            nearbyPlaces = nearbyPlaces.OrderBy(x => x.Distance).ToList();

            var totalResults = nearbyPlaces.Count;
            var paginatedPlaces = nearbyPlaces
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => MapToSearchResult(x.Place, x.Distance, null))
                .ToList();

            stopwatch.Stop();

            return new SearchResultDto
            {
                Places = paginatedPlaces,
                TotalResults = totalResults,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalResults / pageSize),
                SearchQuery = $"Nearby ({latitude:F2}, {longitude:F2})",
                SearchTimeMs = stopwatch.Elapsed.TotalMilliseconds
            };
        }

        public SearchFiltersDto GetAvailableFilters()
        {
            var categories = _context.Categories
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryFilterDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    PlaceCount = c.Places!.Count(p => !p.IsDeleted && p.IsVerified)
                })
                .OrderByDescending(c => c.PlaceCount)
                .ToList();

            var prices = _context.Place
                .Where(p => !p.IsDeleted && p.IsVerified && p.AveragePrice > 0)
                .Select(p => p.AveragePrice)
                .ToList();

            var cities = _context.Place
                .Where(p => !p.IsDeleted && p.IsVerified && !string.IsNullOrEmpty(p.Address))
                .Select(p => p.Address!)
                .Distinct()
                .ToList();

            return new SearchFiltersDto
            {
                AvailableCategories = categories,
                MinPrice = prices.Any() ? prices.Min() : 0,
                MaxPrice = prices.Any() ? prices.Max() : 1000,
                AvailableCities = cities
            };
        }

        // Helper Methods
        private void SaveSearchHistory(int userId, string query, int? categoryId,
            double? latitude, double? longitude, string? filters, int resultCount)
        {
            var searchHistory = new SearchHistory
            {
                UserId = userId,
                SearchQuery = query,
                CategoryId = categoryId,
                Latitude = latitude,
                Longitude = longitude,
                Filters = filters,
                ResultCount = resultCount,
                SearchedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.SearchHistories.Add(searchHistory);
            _context.SaveChanges();
        }

        private PlaceSearchResultDto MapToSearchResult(Place place, double? distance, int? userId)
        {
            return new PlaceSearchResultDto
            {
                PlaceId = place.PlaceId,
                Name = place.Name,
                Description = place.Description,
                MainImageUrl = place.Images?.FirstOrDefault()?.ImageUrl,
                CategoryName = place.Category?.Name,
                CategoryId = place.CategoryId,
                PriceRange = place.AveragePrice > 0 ? place.AveragePrice : null,
                AverageRating = place.Reviews?.Any() == true ? place.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = place.Reviews?.Count ?? 0,
                Latitude = place.Latitude,
                Longitude = place.Longitude,
                DistanceKm = distance.HasValue ? Math.Round(distance.Value, 2) : null,
                IsHiddenGem = place.IsHiddenGem,
                IsFavorited = userId.HasValue && (place.Favorites?.Any(f => f.UserId == userId.Value) ?? false),
                Address = place.Address,
                City = place.Address
            };
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula for calculating distance between two points
            const double R = 6371; // Earth's radius in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1) return "just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} min ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d ago";
            if (timeSpan.TotalDays < 30) return $"{(int)(timeSpan.TotalDays / 7)}w ago";
            if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)}mo ago";
            return $"{(int)(timeSpan.TotalDays / 365)}y ago";
        }
    }
}
