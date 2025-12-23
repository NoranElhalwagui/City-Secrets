using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using FluentAssertions;
using CitySecrets.Services;
using CitySecrets.Models;
using CitySecrets.DTOs;

namespace CitySecrets.Tests.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "TestSecretKeyThatIsLongEnough123456" },
                    { "Jwt:AccessTokenExpirationMinutes", "60" },
                    { "Jwt:RefreshTokenExpirationDays", "7" }
                })
                .Build();

            _authService = new AuthService(_context, config);
        }

        [Fact]
        public void Register_ValidRequest_ShouldSucceed()
        {
            var request = new RegisterRequest
            {
                Username = "testuser",
                Email = "test@test.com",
                Password = "password123",
                FullName = "Test User"
            };

            var result = _authService.Register(request);

            result.Success.Should().BeTrue();
            result.AccessToken.Should().NotBeNull();
            result.RefreshToken.Should().NotBeNull();
            _context.Users.Should().HaveCount(1);
        }

        [Fact]
        public void Register_DuplicateEmail_ShouldFail()
        {
            _context.Users.Add(new User
            {
                Email = "test@test.com",
                Username = "existing",
                PasswordHash = "hash"
            });
            _context.SaveChanges();

            var request = new RegisterRequest
            {
                Username = "newuser",
                Email = "test@test.com",
                Password = "password123"
            };

            var result = _authService.Register(request);

            result.Success.Should().BeFalse();
            result.Message.Should().Be("Email already exists");
        }

        [Fact]
        public void Login_WrongPassword_ShouldFail()
        {
            var user = new User
            {
                Email = "test@test.com",
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct123"),
                IsActive = true
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var request = new LoginRequest
            {
                Email = "test@test.com",
                Password = "wrongpass"
            };

            var result = _authService.Login(request);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public void Login_ValidCredentials_ShouldSucceed()
        {
            var user = new User
            {
                Email = "test@test.com",
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsActive = true
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var request = new LoginRequest
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var result = _authService.Login(request);

            result.Success.Should().BeTrue();
            result.AccessToken.Should().NotBeNull();
            result.RefreshToken.Should().NotBeNull();
        }

        [Fact]
        public void VerifyEmail_ValidToken_ShouldVerify()
        {
            var user = new User
            {
                Email = "test@test.com",
                EmailVerificationToken = "token123",
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(1)
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = _authService.VerifyEmail("token123");

            result.Should().BeTrue();
            _context.Users.First().EmailVerified.Should().BeTrue();
        }

        [Fact]
        public void ResetPassword_ValidToken_ShouldChangePassword()
        {
            var user = new User
            {
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpass"),
                PasswordResetToken = "reset123",
                PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1)
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var request = new ResetPasswordRequest
            {
                Token = "reset123",
                NewPassword = "newpassword123"
            };

            var result = _authService.ResetPassword(request);

            result.Should().BeTrue();
            BCrypt.Net.BCrypt.Verify(
                "newpassword123",
                _context.Users.First().PasswordHash
            ).Should().BeTrue();
        }

        [Fact]
        public void ChangePassword_CorrectCurrentPassword_ShouldSucceed()
        {
            var user = new User
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpass")
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var request = new ChangePasswordRequest
            {
                CurrentPassword = "oldpass",
                NewPassword = "newpass123"
            };

            var result = _authService.ChangePassword(user.UserId, request);

            result.Should().BeTrue();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
