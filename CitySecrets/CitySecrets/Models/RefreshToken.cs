using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySecrets.Models
{
    // 🔄 REFRESH TOKEN MODEL
    // Stores refresh tokens in database for security
    // Refresh tokens let users stay logged in without entering password again
    public class RefreshToken
    {
        [Key]
        public int TokenId { get; set; }

        // Which user does this token belong to?
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User? User { get; set; }

        // The actual token string (long random code)
        [Required]
        [MaxLength(500)]
        public required string Token { get; set; }

        // When does this token stop working?
        public DateTime ExpiresAt { get; set; }

        // When was this token created?
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Is this token still valid? (false if user logged out or token was revoked)
        public bool IsActive { get; set; } = true;

        // What device/browser created this token?
        [MaxLength(200)]
        public string? DeviceInfo { get; set; }

        // IP address that created this token
        [MaxLength(50)]
        public string? IpAddress { get; set; }

        // When was this token last used?
        public DateTime? LastUsedAt { get; set; }
    }
}
