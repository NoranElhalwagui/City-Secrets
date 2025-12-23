using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using CitySecrets.Services;
using CitySecrets.Models;
using CitySecrets.DTOs;

namespace CitySecrets.Tests.Services
{
    public class SearchServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly SearchService _service;

        public SearchServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new SearchService(_context);

            SeedData();
        }

        private void SeedData()
        {
            var category = new Category
            {
                CategoryId = 1,
                Name = "Cafe",
                IsDeleted = false
            };

            var place = new Place
            {
                PlaceId = 1,
                Name = "Hidden Cafe",
                Description = "Nice quiet place",
                Address = "Cairo",
                CategoryId = 1,
                Category = category,
                IsVerified = true,
                IsDeleted = false,
                IsHiddenGem = true,
                Latitude = 30.0,
                Longitude = 31.0,
                AveragePrice = 50,
                Reviews = new System.Collections.Generic.List<Review>
                {
                    new Review
                    {
                        PlaceId = 1,
                        UserId = 1,
                        Rating = 4,
                        Title = "Great",
                        Comment = "Nice place"
                    }
                }
            };

            _context.Categories.Add(category);
            _context.Place.Add(place);
            _context.SaveChanges();
        }

        [Fact]
        public void Search_ValidQuery_ShouldReturnResults()
        {
            var result = _service.Search("Cafe", null, 1, 10);

            result.TotalResults.Should().Be(1);
            result.Places.Should().HaveCount(1);
        }

        [Fact]
        public void Search_ShouldSaveHistory_WhenUserLoggedIn()
        {
            var result = _service.Search("Cafe", userId: 1, page: 1, pageSize: 10);

            _context.SearchHistories.Should().HaveCount(1);
            _context.SearchHistories.First().SearchQuery.Should().Be("Cafe");
        }

        [Fact]
        public void AdvancedSearch_FilterByCategory_ShouldWork()
        {
            var request = new AdvancedSearchRequest
            {
                CategoryId = 1,
                Page = 1,
                PageSize = 10
            };

            var result = _service.AdvancedSearch(request, null);

            result.Places.Should().HaveCount(1);
        }

        [Fact]
        public void AdvancedSearch_MinRating_ShouldFilter()
        {
            var request = new AdvancedSearchRequest
            {
                MinRating = 5,
                Page = 1,
                PageSize = 10
            };

            var result = _service.AdvancedSearch(request, null);

            result.Places.Should().BeEmpty();
        }

        [Fact]
        public void GetSearchSuggestions_ShouldReturnPlaceAndCategory()
        {
            var suggestions = _service.GetSearchSuggestions("Cafe", 10);

            suggestions.Should().NotBeEmpty();
            suggestions.Any(s => s.Type == "place").Should().BeTrue();
            suggestions.Any(s => s.Type == "category").Should().BeTrue();
        }

        [Fact]
        public void GetUserSearchHistory_ShouldReturnUserHistory()
        {
            _context.SearchHistories.Add(new SearchHistory
            {
                UserId = 1,
                SearchQuery = "Cafe",
                SearchedAt = DateTime.UtcNow,
                IsDeleted = false
            });
            _context.SaveChanges();

            var history = _service.GetUserSearchHistory(1, 10);

            history.Should().HaveCount(1);
        }

        [Fact]
        public void ClearUserSearchHistory_ShouldSoftDelete()
        {
            var history = new SearchHistory
            {
                UserId = 1,
                SearchQuery = "Cafe",
                IsDeleted = false
            };

            _context.SearchHistories.Add(history);
            _context.SaveChanges();

            _service.ClearUserSearchHistory(1);

            _context.SearchHistories.First().IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void DeleteSearchHistoryItem_ShouldDelete_WhenOwner()
        {
            var history = new SearchHistory
            {
                SearchId = 1,
                UserId = 1,
                SearchQuery = "Cafe",
                IsDeleted = false
            };

            _context.SearchHistories.Add(history);
            _context.SaveChanges();

            var result = _service.DeleteSearchHistoryItem(1, 1);

            result.Should().BeTrue();
            history.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void SearchNearby_ShouldReturnNearbyPlaces()
        {
            var result = _service.SearchNearby(30.0, 31.0, 5, null, null, 1, 10);

            result.Places.Should().HaveCount(1);
        }

        [Fact]
        public void GetAvailableFilters_ShouldReturnCategoriesAndPrices()
        {
            var filters = _service.GetAvailableFilters();

            filters.AvailableCategories.Should().NotBeEmpty();
            filters.MinPrice.Should().BeGreaterThanOrEqualTo(0);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
