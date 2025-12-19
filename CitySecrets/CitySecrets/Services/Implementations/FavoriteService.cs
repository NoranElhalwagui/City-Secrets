using Microsoft.EntityFrameworkCore;
using CitySecrets.Models;
using CitySecrets.DTOs;
using CitySecrets.Services.Interfaces;

namespace CitySecrets.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _context;

        public FavoriteService(AppDbContext context)
        {
            _context = context;
        }

        public PaginatedFavoritesResult GetUserFavorites(int userId, string? listName, int page, int pageSize)
        {
            var query = _context.Favourites
                .Include(f => f.Place)
                    .ThenInclude(p => p!.Category)
                .Where(f => f.UserId == userId && !f.IsDeleted && !f.Place!.IsDeleted);

            // Filter by list name if provided
            if (!string.IsNullOrWhiteSpace(listName))
            {
                query = query.Where(f => f.ListName == listName);
            }

            var total = query.Count();
            var favorites = query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FavoriteDto
                {
                    Id = f.Id,
                    PlaceId = f.PlaceId,
                    PlaceName = f.Place!.Name,
                    PlaceAddress = f.Place.Address,
                    PlaceCategory = f.Place.Category!.Name,
                    PlaceAverageRating = f.Place.AverageRating,
                    PlaceAveragePrice = f.Place.AveragePrice,
                    ListName = f.ListName,
                    Notes = f.Notes,
                    CreatedAt = f.CreatedAt
                })
                .ToList();

            return new PaginatedFavoritesResult
            {
                Favorites = favorites,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public FavoriteDto AddFavorite(int userId, int placeId, string? listName, string? notes)
        {
            // Check if place exists
            var place = _context.Place.Include(p => p.Category).FirstOrDefault(p => p.PlaceId == placeId && !p.IsDeleted);
            if (place == null)
                throw new InvalidOperationException("Place not found");

            // Check if already favorited
            var existing = _context.Favourites
                .FirstOrDefault(f => f.UserId == userId && f.PlaceId == placeId && !f.IsDeleted);
            
            if (existing != null)
                throw new InvalidOperationException("Place is already in your favorites");

            // Create favorite
            var favorite = new Favorite
            {
                UserId = userId,
                PlaceId = placeId,
                ListName = string.IsNullOrWhiteSpace(listName) ? null : listName,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Favourites.Add(favorite);
            _context.SaveChanges();

            return new FavoriteDto
            {
                Id = favorite.Id,
                PlaceId = placeId,
                PlaceName = place.Name,
                PlaceAddress = place.Address,
                PlaceCategory = place.Category?.Name ?? "",
                PlaceAverageRating = place.AverageRating,
                PlaceAveragePrice = place.AveragePrice,
                ListName = favorite.ListName,
                Notes = favorite.Notes,
                CreatedAt = favorite.CreatedAt
            };
        }

        public bool RemoveFavorite(int userId, int placeId)
        {
            var favorite = _context.Favourites
                .FirstOrDefault(f => f.UserId == userId && f.PlaceId == placeId && !f.IsDeleted);

            if (favorite == null)
                return false;

            // Soft delete
            favorite.IsDeleted = true;
            _context.SaveChanges();

            return true;
        }

        public bool IsFavorite(int userId, int placeId)
        {
            return _context.Favourites
                .Any(f => f.UserId == userId && f.PlaceId == placeId && !f.IsDeleted);
        }

        public List<FavoriteListDto> GetFavoriteLists(int userId)
        {
            var lists = _context.Favourites
                .Where(f => f.UserId == userId && !f.IsDeleted && !string.IsNullOrEmpty(f.ListName))
                .GroupBy(f => f.ListName)
                .Select(g => new FavoriteListDto
                {
                    ListName = g.Key!,
                    Count = g.Count()
                })
                .OrderBy(l => l.ListName)
                .ToList();

            // Add "All Favorites" list (favorites without a specific list name)
            var allCount = _context.Favourites
                .Count(f => f.UserId == userId && !f.IsDeleted && string.IsNullOrEmpty(f.ListName));
            
            if (allCount > 0)
            {
                lists.Insert(0, new FavoriteListDto
                {
                    ListName = "All Favorites",
                    Count = allCount
                });
            }

            return lists;
        }

        public bool UpdateFavoriteNotes(int favoriteId, int userId, string notes)
        {
            var favorite = _context.Favourites
                .FirstOrDefault(f => f.Id == favoriteId && f.UserId == userId && !f.IsDeleted);

            if (favorite == null)
                return false;

            favorite.Notes = notes;
            _context.SaveChanges();

            return true;
        }

        public bool MoveFavoriteToList(int favoriteId, int userId, string newListName)
        {
            var favorite = _context.Favourites
                .FirstOrDefault(f => f.Id == favoriteId && f.UserId == userId && !f.IsDeleted);

            if (favorite == null)
                return false;

            favorite.ListName = string.IsNullOrWhiteSpace(newListName) ? null : newListName;
            _context.SaveChanges();

            return true;
        }
    }
}
