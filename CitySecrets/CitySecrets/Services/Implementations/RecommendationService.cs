using CitySecrets.Models;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CitySecrets.Services.Implementations
{
    /// <summary>
    /// Simple recommendation service - basic implementations for project submission
    /// </summary>
    public class RecommendationService : IRecommendationService
    {
        private readonly AppDbContext _context;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PersonalizedRecommendationDto>> GetPersonalizedRecommendationsAsync(
            int userId, PersonalizedRecommendationRequest request)
        {
            // Get user preferences
            var userPreference = await _context.UserPreferences
            .FirstOrDefaultAsync(up => up.UserId == userId)
            ?? new UserPreference(); // ✅ fallback to prevent null dereference


            // Get user's favorite categories from their reviews and favorites
            var userCategories = await _context.Reviews
                .Where(r => r.UserId == userId && r.Rating >= 4 && r.Place != null)
                .Select(r => r.Place!.CategoryId)
                .Distinct()
                .ToListAsync();

            var favoriteCategoryIds = await _context.Favourites
                .Where(f => f.UserId == userId && f.Place != null)
                .Select(f => f.Place!.CategoryId)
                .Distinct()
                .ToListAsync();

            userCategories.AddRange(favoriteCategoryIds);
            userCategories = userCategories.Distinct().ToList();

            // Get places user has visited or favorited
            var visitedPlaceIds = await _context.PlaceViews
                .Where(pv => pv.UserId == userId)
                .Select(pv => pv.PlaceId)
                .Distinct()
                .ToListAsync();

            var favoritePlaceIds = await _context.Favourites
                .Where(f => f.UserId == userId)
                .Select(f => f.PlaceId)
                .ToListAsync();

            var query = _context.Place
                .Where(p => p.IsActive && !p.IsDeleted && p.IsVerified)
                .Include(p => p.Category)
                .AsQueryable();

            // Filter out visited/favorited if requested
            if (!request.IncludeVisited)
                query = query.Where(p => !visitedPlaceIds.Contains(p.PlaceId));
            
            if (!request.IncludeFavorites)
                query = query.Where(p => !favoritePlaceIds.Contains(p.PlaceId));

            // Prioritize user's preferred categories
            if (userCategories.Any())
            {
                query = query.Where(p => userCategories.Contains(p.CategoryId));
            }

            // Apply location filter if provided
            var places = await query.ToListAsync();
            
            var recommendations = places
                .Select(p => new PersonalizedRecommendationDto
                {
                    PlaceId = p.PlaceId,
                    Name = p.Name,
                    Description = p.Description,
                    Address = p.Address,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? "Unknown",
                    AveragePrice = p.AveragePrice,
                    PriceRange = p.PriceRange,
                    AverageRating = p.AverageRating,
                    ReviewCount = _context.Reviews.Count(r => r.PlaceId == p.PlaceId && r.IsApproved),
                    IsHiddenGem = p.IsHiddenGem,
                    HiddenGemScore = p.HiddenGemScore,
                    IsVerified = p.IsVerified,
                    HasVisited = visitedPlaceIds.Contains(p.PlaceId),
                    IsFavorited = favoritePlaceIds.Contains(p.PlaceId),
                    PersonalizationScore = CalculatePersonalizationScore(p, userId, userPreference, userCategories),
                    MatchReasons = GetMatchReasons(p, userPreference, userCategories)
                })
                .OrderByDescending(r => r.PersonalizationScore)
                .ThenByDescending(r => r.AverageRating)
                .Take(request.Count)
                .ToList();

            // Calculate distances if location provided
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                foreach (var rec in recommendations)
                {
                    rec.DistanceKm = CalculateDistance(request.Latitude.Value, request.Longitude.Value, rec.Latitude, rec.Longitude);
                }
            }

            return recommendations;
        }

        public async Task<List<TrendingPlaceDto>> GetTrendingPlacesAsync(int count)
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            var trendingData = await _context.Place
                .Where(p => p.IsActive && !p.IsDeleted && p.IsVerified)
                .Include(p => p.Category)
                .Select(p => new
                {
                    Place = p,
                    ViewsLast7Days = _context.PlaceViews.Count(pv => pv.PlaceId == p.PlaceId && pv.ViewedAt >= sevenDaysAgo),
                    ReviewsLast7Days = _context.Reviews.Count(r => r.PlaceId == p.PlaceId && r.CreatedAt >= sevenDaysAgo),
                    FavoritesLast7Days = _context.Favourites.Count(f => f.PlaceId == p.PlaceId && f.CreatedAt >= sevenDaysAgo)
                })
                .ToListAsync();

            var trending = trendingData
                .Select(t => new TrendingPlaceDto
                {
                    PlaceId = t.Place.PlaceId,
                    Name = t.Place.Name,
                    Description = t.Place.Description,
                    CategoryId = t.Place.CategoryId,
                    CategoryName = t.Place.Category?.Name ?? "Unknown",
                    AverageRating = t.Place.AverageRating,
                    ReviewCount = _context.Reviews.Count(r => r.PlaceId == t.Place.PlaceId && r.IsApproved),
                    ViewsLast7Days = t.ViewsLast7Days,
                    ReviewsLast7Days = t.ReviewsLast7Days,
                    TrendingScore = CalculateTrendingScore(t.ViewsLast7Days, t.ReviewsLast7Days, t.FavoritesLast7Days, t.Place.AverageRating),
                    TrendingReason = GetTrendingReason(t.ViewsLast7Days, t.ReviewsLast7Days, t.FavoritesLast7Days)
                })
                .OrderByDescending(t => t.TrendingScore)
                .Take(count)
                .ToList();

            return trending;
        }

        public async Task<List<RecommendationDto>> GetHiddenGemsAsync(HiddenGemSearchRequest request)
        {
            var places = await _context.Place
                .Where(p => p.IsActive && !p.IsDeleted && p.IsVerified && p.IsHiddenGem)
                .Where(p => p.HiddenGemScore >= (request.MinHiddenGemScore ?? 50))
                .Include(p => p.Category)
                .ToListAsync();

            // Filter by distance
            var nearbyGems = places
                .Select(p => new
                {
                    Place = p,
                    Distance = CalculateDistance(request.Latitude, request.Longitude, p.Latitude, p.Longitude)
                })
                .Where(x => x.Distance <= request.RadiusKm)
                .OrderBy(x => x.Distance)
                .Take(request.Count)
                .Select(x => new RecommendationDto
                {
                    PlaceId = x.Place.PlaceId,
                    Name = x.Place.Name,
                    Description = x.Place.Description,
                    Address = x.Place.Address,
                    Latitude = x.Place.Latitude,
                    Longitude = x.Place.Longitude,
                    DistanceKm = Math.Round(x.Distance, 2),
                    CategoryId = x.Place.CategoryId,
                    CategoryName = x.Place.Category?.Name ?? "Unknown",
                    AveragePrice = x.Place.AveragePrice,
                    PriceRange = x.Place.PriceRange,
                    AverageRating = x.Place.AverageRating,
                    ReviewCount = _context.Reviews.Count(r => r.PlaceId == x.Place.PlaceId && r.IsApproved),
                    IsHiddenGem = x.Place.IsHiddenGem,
                    HiddenGemScore = x.Place.HiddenGemScore,
                    IsVerified = x.Place.IsVerified,
                    RecommendationScore = x.Place.HiddenGemScore,
                    RecommendationReason = $"Hidden gem with score {x.Place.HiddenGemScore}/100, {Math.Round(x.Distance, 1)}km away"
                })
                .ToList();

            return nearbyGems;
        }

        public async Task<List<RecommendationDto>> GetSimilarPlacesAsync(int placeId, int count)
        {
            var place = await _context.Place
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.PlaceId == placeId);

            if (place == null)
                return new List<RecommendationDto>();

            // Find similar places based on category, price range, and location
            var similarPlaces = await _context.Place
                .Where(p => p.PlaceId != placeId && p.IsActive && !p.IsDeleted && p.IsVerified)
                .Where(p => p.CategoryId == place.CategoryId || p.PriceRange == place.PriceRange)
                .Include(p => p.Category)
                .ToListAsync();

            var recommendations = similarPlaces
                .Select(p => new
                {
                    Place = p,
                    Distance = CalculateDistance(place.Latitude, place.Longitude, p.Latitude, p.Longitude),
                    SimilarityScore = CalculateSimilarityScore(place, p)
                })
                .OrderByDescending(x => x.SimilarityScore)
                .ThenBy(x => x.Distance)
                .Take(count)
                .Select(x => new RecommendationDto
                {
                    PlaceId = x.Place.PlaceId,
                    Name = x.Place.Name,
                    Description = x.Place.Description,
                    Address = x.Place.Address,
                    Latitude = x.Place.Latitude,
                    Longitude = x.Place.Longitude,
                    DistanceKm = Math.Round(x.Distance, 2),
                    CategoryId = x.Place.CategoryId,
                    CategoryName = x.Place.Category?.Name ?? "Unknown",
                    AveragePrice = x.Place.AveragePrice,
                    PriceRange = x.Place.PriceRange,
                    AverageRating = x.Place.AverageRating,
                    ReviewCount = _context.Reviews.Count(r => r.PlaceId == x.Place.PlaceId && r.IsApproved),
                    IsHiddenGem = x.Place.IsHiddenGem,
                    HiddenGemScore = x.Place.HiddenGemScore,
                    IsVerified = x.Place.IsVerified,
                    RecommendationScore = x.SimilarityScore,
                    RecommendationReason = GetSimilarityReason(place, x.Place)
                })
                .ToList();

            return recommendations;
        }

        public async Task<List<RecommendationDto>> GetPopularPlacesByCategoryAsync(
            int categoryId, int count, double? latitude, double? longitude)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var places = await _context.Place
                .Where(p => p.CategoryId == categoryId && p.IsActive && !p.IsDeleted && p.IsVerified)
                .Include(p => p.Category)
                .ToListAsync();

            var recommendations = places
                .Select(p => new RecommendationDto
                {
                    PlaceId = p.PlaceId,
                    Name = p.Name,
                    Description = p.Description,
                    Address = p.Address,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? "Unknown",
                    AveragePrice = p.AveragePrice,
                    PriceRange = p.PriceRange,
                    AverageRating = p.AverageRating,
                    ReviewCount = _context.Reviews.Count(r => r.PlaceId == p.PlaceId && r.IsApproved),
                    IsHiddenGem = p.IsHiddenGem,
                    HiddenGemScore = p.HiddenGemScore,
                    IsVerified = p.IsVerified,
                    RecommendationScore = CalculatePopularityScore(p, thirtyDaysAgo),
                    RecommendationReason = "Popular in this category"
                })
                .OrderByDescending(r => r.RecommendationScore)
                .ThenByDescending(r => r.AverageRating)
                .Take(count)
                .ToList();

            // Add distance if location provided
            if (latitude.HasValue && longitude.HasValue)
            {
                foreach (var rec in recommendations)
                {
                    rec.DistanceKm = CalculateDistance(latitude.Value, longitude.Value, rec.Latitude, rec.Longitude);
                }
            }

            return recommendations;
        }

        public async Task<RecommendationResultDto> GetExploreRecommendationsAsync(
            RecommendationRequest request, int? userId)
        {
            var query = _context.Place
                .Where(p => p.IsActive && !p.IsDeleted && p.IsVerified)
                .Include(p => p.Category)
                .AsQueryable();

            // Apply filters
            if (request.CategoryIds != null && request.CategoryIds.Any())
                query = query.Where(p => request.CategoryIds.Contains(p.CategoryId));

            if (!string.IsNullOrWhiteSpace(request.PriceRange))
                query = query.Where(p => p.PriceRange == request.PriceRange);

            if (request.MinRating.HasValue)
                query = query.Where(p => p.AverageRating >= request.MinRating.Value);

            if (request.VerifiedOnly == true)
                query = query.Where(p => p.IsVerified);

            var places = await query.ToListAsync();

            // Filter by location if provided
            if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
            {
                places = places
                    .Where(p => CalculateDistance(request.Latitude.Value, request.Longitude.Value, p.Latitude, p.Longitude) <= request.RadiusKm.Value)
                    .ToList();
            }

            var recommendations = places
                .Select(p => new RecommendationDto
                {
                    PlaceId = p.PlaceId,
                    Name = p.Name,
                    Description = p.Description,
                    Address = p.Address,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    DistanceKm = request.Latitude.HasValue && request.Longitude.HasValue
                        ? CalculateDistance(request.Latitude.Value, request.Longitude.Value, p.Latitude, p.Longitude)
                        : null,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? "Unknown",
                    AveragePrice = p.AveragePrice,
                    PriceRange = p.PriceRange,
                    AverageRating = p.AverageRating,
                    ReviewCount = _context.Reviews.Count(r => r.PlaceId == p.PlaceId && r.IsApproved),
                    IsHiddenGem = p.IsHiddenGem,
                    HiddenGemScore = p.HiddenGemScore,
                    IsVerified = p.IsVerified,
                    RecommendationScore = CalculateRecommendationScore(p, userId, request.Latitude, request.Longitude),
                    RecommendationReason = "Explore recommendation based on your filters"
                })
                .OrderByDescending(r => r.RecommendationScore)
                .ThenByDescending(r => r.AverageRating)
                .Take(request.Count)
                .ToList();

            return new RecommendationResultDto
            {
                Recommendations = recommendations,
                TotalCount = recommendations.Count,
                BasedOn = "Explore with custom filters",
                Metadata = new Dictionary<string, object>
                {
                    { "filters_applied", GetAppliedFilters(request) }
                }
            };
        }

        public async Task<List<RecommendationDto>> GetNearbyRecommendationsAsync(
            double latitude, double longitude, double radiusKm, int count)
        {
            var places = await _context.Place
                .Where(p => p.IsActive && !p.IsDeleted && p.IsVerified)
                .Include(p => p.Category)
                .ToListAsync();

            var nearby = places
                .Select(p => new
                {
                    Place = p,
                    Distance = CalculateDistance(latitude, longitude, p.Latitude, p.Longitude)
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderBy(x => x.Distance)
                .Take(count)
                .Select(x => new RecommendationDto
                {
                    PlaceId = x.Place.PlaceId,
                    Name = x.Place.Name,
                    Description = x.Place.Description,
                    Address = x.Place.Address,
                    Latitude = x.Place.Latitude,
                    Longitude = x.Place.Longitude,
                    DistanceKm = Math.Round(x.Distance, 2),
                    CategoryId = x.Place.CategoryId,
                    CategoryName = x.Place.Category?.Name ?? "Unknown",
                    AveragePrice = x.Place.AveragePrice,
                    PriceRange = x.Place.PriceRange,
                    AverageRating = x.Place.AverageRating,
                    ReviewCount = _context.Reviews.Count(r => r.PlaceId == x.Place.PlaceId && r.IsApproved),
                    IsHiddenGem = x.Place.IsHiddenGem,
                    HiddenGemScore = x.Place.HiddenGemScore,
                    IsVerified = x.Place.IsVerified,
                    RecommendationScore = 100 - (x.Distance * 10), // Closer = higher score
                    RecommendationReason = $"Only {Math.Round(x.Distance, 1)}km away"
                })
                .ToList();

            return nearby;
        }

        public async Task<RecommendationStatsDto> GetRecommendationStatsAsync(
            double? latitude, double? longitude, double? radiusKm)
        {
            var query = _context.Place
                .Where(p => p.IsActive && !p.IsDeleted && p.IsVerified)
                .AsQueryable();

            var places = await query.Include(p => p.Category).ToListAsync();

            // Filter by location if provided
            if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
            {
                places = places
                    .Where(p => CalculateDistance(latitude.Value, longitude.Value, p.Latitude, p.Longitude) <= radiusKm.Value)
                    .ToList();
            }

            var categoryDistribution = places
                .GroupBy(p => p.Category?.Name ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            return new RecommendationStatsDto
            {
                TotalPlaces = places.Count,
                HiddenGems = places.Count(p => p.IsHiddenGem),
                VerifiedPlaces = places.Count(p => p.IsVerified),
                AverageRating = places.Any() ? places.Average(p => p.AverageRating) : 0,
                CategoryDistribution = categoryDistribution
            };
        }

        // ============ HELPER METHODS ============

        public double CalculateRecommendationScore(Place place, int? userId, double? userLatitude, double? userLongitude)
        {
            double score = 0;

            // Rating component (0-50 points)
            score += place.AverageRating * 10;

            // Hidden gem bonus (0-20 points)
            if (place.IsHiddenGem)
                score += place.HiddenGemScore * 0.2;

            // Verified bonus (10 points)
            if (place.IsVerified)
                score += 10;

            // Review count component (0-20 points)
            var reviewCount = _context.Reviews.Count(r => r.PlaceId == place.PlaceId && r.IsApproved);
            score += Math.Min(reviewCount, 20);

            // Proximity bonus if location provided (0-10 points)
            if (userLatitude.HasValue && userLongitude.HasValue)
            {
                var distance = CalculateDistance(userLatitude.Value, userLongitude.Value, place.Latitude, place.Longitude);
                score += Math.Max(0, 10 - distance);
            }

            return score;
        }

        private double CalculatePersonalizationScore(Place place, int userId, UserPreference? preference, List<int> userCategories)
        {
            double score = 0;

            // Category match (30 points)
            if (userCategories.Contains(place.CategoryId))
                score += 30;

            // Price range match (20 points)
            if (preference != null && place.AveragePrice > 0)
            {
                if ((!preference.MinPrice.HasValue || place.AveragePrice >= preference.MinPrice.Value) &&
                    (!preference.MaxPrice.HasValue || place.AveragePrice <= preference.MaxPrice.Value))
                    score += 20;
            }

            // Rating component (30 points)
            score += place.AverageRating * 6;

            // Hidden gem preference (20 points)
            if (place.IsHiddenGem)
                score += 20;

            return score;
        }

        private double CalculateTrendingScore(int views, int reviews, int favorites, double rating)
        {
            return (views * 0.3) + (reviews * 2.0) + (favorites * 3.0) + (rating * 5);
        }

        private double CalculateSimilarityScore(Place original, Place candidate)
        {
            double score = 0;

            // Same category (40 points)
            if (original.CategoryId == candidate.CategoryId)
                score += 40;

            // Similar price range (20 points)
            if (original.PriceRange == candidate.PriceRange)
                score += 20;

            // Similar rating (20 points)
            var ratingDiff = Math.Abs(original.AverageRating - candidate.AverageRating);
            score += Math.Max(0, 20 - (ratingDiff * 4));

            // Both hidden gems (20 points)
            if (original.IsHiddenGem && candidate.IsHiddenGem)
                score += 20;

            return score;
        }

        private double CalculatePopularityScore(Place place, DateTime since)
        {
            var views = _context.PlaceViews.Count(pv => pv.PlaceId == place.PlaceId && pv.ViewedAt >= since);
            var reviews = _context.Reviews.Count(r => r.PlaceId == place.PlaceId && r.CreatedAt >= since);
            var favorites = _context.Favourites.Count(f => f.PlaceId == place.PlaceId && f.CreatedAt >= since);

            return (views * 0.5) + (reviews * 5) + (favorites * 10) + (place.AverageRating * 10);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula
            var R = 6371; // Earth radius in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180;

        private List<string> GetMatchReasons(Place place, UserPreference? preference, List<int> userCategories)
        {
            var reasons = new List<string>();

            if (userCategories.Contains(place.CategoryId))
                reasons.Add("Matches your favorite categories");

            if (preference != null && place.AveragePrice > 0 &&
                (!preference.MinPrice.HasValue || place.AveragePrice >= preference.MinPrice.Value) &&
                (!preference.MaxPrice.HasValue || place.AveragePrice <= preference.MaxPrice.Value))
                reasons.Add("In your preferred price range");

            if (place.AverageRating >= 4.5)
                reasons.Add("Highly rated by others");

            if (place.IsHiddenGem)
                reasons.Add("Hidden gem worth discovering");

            return reasons;
        }

        private string GetTrendingReason(int views, int reviews, int favorites)
        {
            if (reviews > 10)
                return $"Popular with {reviews} new reviews this week";
            if (favorites > 5)
                return $"Trending with {favorites} new favorites";
            if (views > 50)
                return $"Hot spot with {views} views this week";
            return "Trending in your area";
        }

        private string GetSimilarityReason(Place original, Place candidate)
        {
            if (original.CategoryId == candidate.CategoryId && original.PriceRange == candidate.PriceRange)
                return $"Similar category and price range to {original.Name}";
            if (original.CategoryId == candidate.CategoryId)
                return $"Same category as {original.Name}";
            if (original.PriceRange == candidate.PriceRange)
                return $"Similar price range to {original.Name}";
            return $"Similar to {original.Name}";
        }

        private List<string> GetAppliedFilters(RecommendationRequest request)
        {
            var filters = new List<string>();

            if (request.CategoryIds != null && request.CategoryIds.Any())
                filters.Add($"Categories: {string.Join(", ", request.CategoryIds)}");

            if (!string.IsNullOrWhiteSpace(request.PriceRange))
                filters.Add($"Price: {request.PriceRange}");

            if (request.MinRating.HasValue)
                filters.Add($"Min Rating: {request.MinRating.Value}");

            if (request.VerifiedOnly == true)
                filters.Add("Verified only");

            if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
                filters.Add($"Within {request.RadiusKm}km");

            return filters;
        }
    }
}
