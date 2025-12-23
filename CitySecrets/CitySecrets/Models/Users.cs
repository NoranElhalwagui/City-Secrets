//using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }

        // User preferences
        public string? PreferredCategories { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? PreferredPriceMin { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? PreferredPriceMax { get; set; }

        public double? PreferredMaxDistance { get; set; } // in km

        // User's default location
        public double? DefaultLatitude { get; set; }
        public double? DefaultLongitude { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        // Account status
        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; } = false;
        public bool EmailVerified { get; set; } = false; //blocks adding reviews for spam or fake accounts

        // ✉️ Email Verification - To confirm user's email is real
        [MaxLength(500)]
        public string? EmailVerificationToken { get; set; }  // Secret code sent to email
        public DateTime? EmailVerificationTokenExpiry { get; set; }  // When code expires

        // 🔑 Password Reset - To help users who forgot password
        [MaxLength(500)]
        public string? PasswordResetToken { get; set; }  // Secret code for password reset
        public DateTime? PasswordResetTokenExpiry { get; set; }  // When code expires

        // 🛡️ Account Security - Prevent brute force attacks
        public int FailedLoginAttempts { get; set; } = 0;  // How many wrong passwords?
        public DateTime? AccountLockedUntil { get; set; }  // Locked out until this time

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;//automatically sets it to the current time when the user is created
        public DateTime? LastLoginAt { get; set; }

        public bool IsDeleted { get; set; } = false; //hide what the user have deleted instead of deleting it from the DB

        // Navigation properties - Collections
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<PlaceView>? PlaceViews { get; set; }
        public ICollection<SearchHistory>? SearchHistories { get; set; }
        public ICollection<ReviewHelpfulness>? ReviewHelpfulness { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public UserPreference? UserPreference { get; set; }
        public ICollection<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();
    }
}