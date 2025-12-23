using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using CitySecrets.Models;
using CitySecrets.Services.Implementations;

namespace CitySecrets.Tests.Services
{
    public class PlaceServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly PlaceService _service;

        public PlaceServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new PlaceService(_context);
        }

        [Fact]
        public async Task GetAllPlacesAsync_ShouldReturnOnlyActiveAndNotDeleted()
        {
            _context.Place.AddRange(
                CreatePlace("Active", isActive: true, isDeleted: false),
                CreatePlace("Deleted", isActive: true, isDeleted: true),
                CreatePlace("Inactive", isActive: false, isDeleted: false)
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetAllPlacesAsync();

            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Active");
        }

        [Fact]
        public async Task GetPlaceByIdAsync_ShouldIncreaseVisitCount()
        {
            var place = CreatePlace("Cafe");
            _context.Place.Add(place);
            await _context.SaveChangesAsync();

            var result = await _service.GetPlaceByIdAsync(place.PlaceId);

            result.Should().NotBeNull();
            result!.VisitCount.Should().Be(1);
        }

        [Fact]
        public async Task CreatePlaceAsync_ShouldCreatePlace()
        {
            var place = CreatePlace("New Place");

            var result = await _service.CreatePlaceAsync(place, userId: 2);

            result.PlaceId.Should().BeGreaterThan(0);
            result.IsVerified.Should().BeFalse();
            result.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePlaceAsync_ExistingPlace_ShouldUpdateFields()
        {
            var place = CreatePlace("Old");
            _context.Place.Add(place);
            await _context.SaveChangesAsync();

            var updated = CreatePlace("New Name", address: "New Address");

            var result = await _service.UpdatePlaceAsync(place.PlaceId, updated, 2);

            result.Should().NotBeNull();
            result!.Name.Should().Be("New Name");
            result.Address.Should().Be("New Address");
        }

        [Fact]
        public async Task DeletePlaceAsync_NonAdmin_ShouldFail()
        {
            var place = CreatePlace("ToDelete");
            _context.Place.Add(place);
            await _context.SaveChangesAsync();

            var result = await _service.DeletePlaceAsync(place.PlaceId, adminUserId: 2);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeletePlaceAsync_Admin_ShouldSoftDelete()
        {
            var place = CreatePlace("ToDeleteAdmin");
            _context.Place.Add(place);
            await _context.SaveChangesAsync();

            var result = await _service.DeletePlaceAsync(place.PlaceId, adminUserId: 1);

            result.Should().BeTrue();
            _context.Place.First().IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task VerifyPlaceAsync_Admin_ShouldVerify()
        {
            var place = CreatePlace("Verify", isVerified: false);
            _context.Place.Add(place);
            await _context.SaveChangesAsync();

            var result = await _service.VerifyPlaceAsync(place.PlaceId, adminUserId: 1);

            result.Should().BeTrue();
            _context.Place.First().IsVerified.Should().BeTrue();
        }

        [Fact]
        public async Task SearchPlacesAsync_ShouldReturnMatchingResults()
        {
            _context.Place.Add(CreatePlace("Hidden Cafe", isDeleted: false));
            await _context.SaveChangesAsync();

            var result = await _service.SearchPlacesAsync("Cafe");

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetNearbyPlacesAsync_ShouldReturnWithinRadius()
        {
            _context.Place.Add(CreatePlace("Near", latitude: 30.0, longitude: 31.0, isActive: true, isDeleted: false));

            await _context.SaveChangesAsync();

            var result = await _service.GetNearbyPlacesAsync(30.0, 31.0, 5);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetPlacesByCategoryAsync_ShouldFilterCorrectly()
        {
            _context.Place.AddRange(
                CreatePlace("Cat1", categoryId: 1),
                CreatePlace("Cat2", categoryId: 2)
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetPlacesByCategoryAsync(1);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task RecalculatePlaceScoresAsync_ShouldUpdateScores()
        {
            var place = CreatePlace("Score", visitCount: 10, totalReviews: 5, isHiddenGem: true);

            _context.Place.Add(place);
            await _context.SaveChangesAsync();

            await _service.RecalculatePlaceScoresAsync(place.PlaceId);

            var updated = _context.Place.First();
            updated.PopularityScore.Should().Be(10 * 2 + 5 * 3);
            updated.HiddenGemScore.Should().Be(80 - updated.PopularityScore);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private static Place CreatePlace(
            string name,
            string? address = null,
            string? description = null,
            double latitude = 0,
            double longitude = 0,
            int categoryId = 1,
            bool isActive = true,
            bool isDeleted = false,
            bool isVerified = true,
            bool isHiddenGem = false,
            int visitCount = 0,
            int totalReviews = 0)
        {
            return new Place
            {
                Name = name,
                Description = description ?? "Desc",
                Address = address ?? "Addr",
                Latitude = latitude,
                Longitude = longitude,
                CategoryId = categoryId,
                IsActive = isActive,
                IsDeleted = isDeleted,
                IsVerified = isVerified,
                IsHiddenGem = isHiddenGem,
                VisitCount = visitCount,
                TotalReviews = totalReviews
            };
        }
    }
}
