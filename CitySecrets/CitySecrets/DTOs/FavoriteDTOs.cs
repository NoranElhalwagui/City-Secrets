using System;
using System.Collections.Generic;

namespace CitySecrets.DTOs
{
    // DTO for a single favorite place
    public class FavoriteDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string PlaceName { get; set; } = "";
        public string PlaceAddress { get; set; } = "";
        public string PlaceCategory { get; set; } = "";
        public double PlaceAverageRating { get; set; }
        public decimal PlaceAveragePrice { get; set; }
        public string? ListName { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO for paginated favorites result
    public class PaginatedFavoritesResult
    {
        public List<FavoriteDto> Favorites { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    // DTO for favorite list summary
    public class FavoriteListDto
    {
        public string ListName { get; set; } = "";
        public int Count { get; set; }
    }
}
