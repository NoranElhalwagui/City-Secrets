using CitySecrets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitySecrets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlacesController(AppDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // GET: api/places
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllPlaces(
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            double? minRating)
        {
            var query = _context.Place
                .Where(p => !p.IsDeleted && p.IsActive)
                .AsQueryable();

            // Apply filters if provided
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.AveragePrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.AveragePrice <= maxPrice.Value);

            if (minRating.HasValue)
                query = query.Where(p => p.AverageRating >= minRating.Value);

            var places = await query.ToListAsync();
            return Ok(places);
        }

        // ---------------------------------------------------------
        // GET: api/places/{id}
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlace(int id)
        {
            var place = await _context.Place
                .FirstOrDefaultAsync(p => p.PlaceId == id && !p.IsDeleted);

            if (place == null)
                return NotFound(new { Message = "Place not found" });

            return Ok(place);
        }

        // ---------------------------------------------------------
        // POST: api/places
        // ADD A NEW PLACE (Owner submits a hidden gem)
        // ---------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreatePlace([FromBody] Place place)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            place.CreatedAt = DateTime.UtcNow;

            _context.Place.Add(place);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlace), new { id = place.PlaceId }, place);
        }

        // ---------------------------------------------------------
        // PUT: api/places/{id}
        // UPDATE A PLACE
        // ---------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlace(int id, [FromBody] Place updatedPlace)
        {
            if (id != updatedPlace.PlaceId)
                return BadRequest("Place ID mismatch");

            var place = await _context.Place.FindAsync(id);

            if (place == null)
                return NotFound("Place not found");

            // update fields
            place.Name = updatedPlace.Name;
            place.Description = updatedPlace.Description;
            place.Address = updatedPlace.Address;
            place.Latitude = updatedPlace.Latitude;
            place.Longitude = updatedPlace.Longitude;
            place.AveragePrice = updatedPlace.AveragePrice;
            place.PriceRange = updatedPlace.PriceRange;
            place.CategoryId = updatedPlace.CategoryId;
            place.Email = updatedPlace.Email;
            place.PhoneNumber = updatedPlace.PhoneNumber;
            place.Website = updatedPlace.Website;
            place.OpeningHours = updatedPlace.OpeningHours;

            place.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(place);
        }

        // ---------------------------------------------------------
        // DELETE: api/places/{id}
        // SOFT DELETE (Keep in DB but hidden)
        // ---------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlace(int id)
        {
            var place = await _context.Place.FindAsync(id);

            if (place == null)
                return NotFound("Place not found");

            place.IsDeleted = true;
            place.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Place deleted (soft delete)" });
        }
    }
}
