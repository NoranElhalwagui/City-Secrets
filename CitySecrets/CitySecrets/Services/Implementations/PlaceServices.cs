using CitySecrets.Models;
using CitySecrets.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CitySecrets.Services.Implementations
{


    public class PlaceService : IPlaceService
    {
        private readonly AppDbContext _context;

        public PlaceService(AppDbContext context)
        {
            _context = context;
        }

        // 1️⃣ Get all active, non-deleted places
        public async Task<IEnumerable<Place>> GetAllPlacesAsync()
        {
            return await _context.Place
                .Where(p => !p.IsDeleted && p.IsActive)
                .ToListAsync();
        }

        // 2️⃣ Get place by id + increment visit count
        public async Task<Place?> GetPlaceByIdAsync(int placeId)
        {
            var place = await _context.Place
                .FirstOrDefaultAsync(p => p.PlaceId == placeId && !p.IsDeleted);

            if (place == null)
                return null;

            place.VisitCount++;
            place.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return place;
        }

        // 3️⃣ Create new place (user or admin)
        public async Task<Place> CreatePlaceAsync(Place place, int userId)
        {
            place.CreatedAt = DateTime.UtcNow;
            place.IsVerified = false; // admin must verify
            place.IsDeleted = false;

            _context.Place.Add(place);
            await _context.SaveChangesAsync();

            return place;
        }

        // 4️⃣ Update place (owner or admin – simplified)
        public async Task<Place?> UpdatePlaceAsync(int placeId, Place updatedPlace, int userId)
        {
            var existingPlace = await _context.Place
                .FirstOrDefaultAsync(p => p.PlaceId == placeId && !p.IsDeleted);

            if (existingPlace == null)
                return null;

            existingPlace.Name = updatedPlace.Name;
            existingPlace.Description = updatedPlace.Description;
            existingPlace.Address = updatedPlace.Address;
            existingPlace.AveragePrice = updatedPlace.AveragePrice;
            existingPlace.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingPlace;
        }

        // 5️⃣ Soft delete (admin only – simplified check)
        public async Task<bool> DeletePlaceAsync(int placeId, int adminUserId)
        {
            // Simple admin check for now
            if (adminUserId != 1)
                return false;

            var place = await _context.Place.FindAsync(placeId);
            if (place == null)
                return false;

            place.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        // 6️⃣ Verify place (admin only)
        public async Task<bool> VerifyPlaceAsync(int placeId, int adminUserId)
        {
            if (adminUserId != 1)
                return false;

            var place = await _context.Place.FindAsync(placeId);
            if (place == null)
                return false;

            place.IsVerified = true;
            await _context.SaveChangesAsync();

            return true;
        }

        // 7️⃣ Search by name, description, or address
        public async Task<IEnumerable<Place>> SearchPlacesAsync(string keyword)
        {
            return await _context.Place
                .Where(p =>
                    !p.IsDeleted &&
                    (p.Name.Contains(keyword) ||
                     p.Description!.Contains(keyword) ||
                     p.Address.Contains(keyword)))
                .ToListAsync();
        }

        // 8️⃣ Nearby places (Haversine formula)
        public async Task<IEnumerable<Place>> GetNearbyPlacesAsync(
            double latitude,
            double longitude,
            double radiusKm)
        {
            var places = await _context.Place
                .Where(p => !p.IsDeleted && p.IsActive)
                .ToListAsync();

            return places.Where(p =>
                CalculateDistance(latitude, longitude, p.Latitude, p.Longitude) <= radiusKm
            );
        }

        // 9️⃣ Get places by category
        public async Task<IEnumerable<Place>> GetPlacesByCategoryAsync(int categoryId)
        {
            return await _context.Place
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .ToListAsync();
        }

        // 🔟 Recalculate popularity & hidden gem scores
        public async Task RecalculatePlaceScoresAsync(int placeId)
        {
            var place = await _context.Place.FindAsync(placeId);
            if (place == null)
                return;

            place.PopularityScore =
                (place.VisitCount * 2) +
                (place.TotalReviews * 3);

            place.HiddenGemScore =
                place.IsHiddenGem ? 80 - place.PopularityScore : 0;

            await _context.SaveChangesAsync();
        }

        // 🔢 Helper: Haversine formula
        private static double CalculateDistance(
            double lat1, double lon1,
            double lat2, double lon2)
        {
            const double R = 6371; // Earth radius in km

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }

}
