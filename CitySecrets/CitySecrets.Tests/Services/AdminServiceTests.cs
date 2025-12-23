using System;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using CitySecrets.Models;
using CitySecrets.Services;

namespace CitySecrets.Tests.Services
{
    public class AdminServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _adminService = new AdminService(_context);

            SeedData();
        }

        private void SeedData()
        {
            _context.Place.Add(new Place
            {
                PlaceId = 1,
                Name = "Test Place",
                IsActive = true,
                IsDeleted = false
            });

            _context.Users.Add(new User
            {
                UserId = 1,
                Email = "test@test.com",
                IsActive = true
            });

            _context.Reviews.Add(new Review
            {
                ReviewId = 1,
                IsFlagged = true,
                IsApproved = false
            });

            _context.SaveChanges();
        }

        // ✅ Create Place
        [Fact]
        public void CreatePlace_ShouldCreateSuccessfully()
        {
            var place = new Place { Name = "New Place" };

            var result = _adminService.CreatePlace(place);

            result.Should().NotBeNull();
            result.IsActive.Should().BeTrue();
        }

        // ✅ Update Place
        [Fact]
        public void UpdatePlace_ShouldUpdateName()
        {
            var updated = new Place { Name = "Updated Name" };

            var result = _adminService.UpdatePlace(1, updated);

            result!.Name.Should().Be("Updated Name");
        }

        // ✅ Soft Delete Place
        [Fact]
        public void DeletePlace_ShouldSoftDelete()
        {
            var success = _adminService.DeletePlace(1, false);

            success.Should().BeTrue();
            _context.Place.Find(1)!.IsDeleted.Should().BeTrue();
        }

        // ✅ Approve Review
        [Fact]
        public void ApproveReview_ShouldApprove()
        {
            var success = _adminService.ApproveReview(1);

            success.Should().BeTrue();
            _context.Reviews.Find(1)!.IsApproved.Should().BeTrue();
        }

        // ✅ Ban User
        [Fact]
        public void BanUser_ShouldDeactivateUser()
        {
            var success = _adminService.BanUser(1, 1, "Violation");

            success.Should().BeTrue();
            _context.Users.Find(1)!.IsActive.Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
