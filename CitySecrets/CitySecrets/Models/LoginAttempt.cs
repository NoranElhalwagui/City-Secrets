using System;
using System.ComponentModel.DataAnnotations;

namespace CitySecrets.Models
{
    // 🛡️ LOGIN ATTEMPT MODEL
    // Tracks failed login attempts to prevent brute force attacks
    // If someone tries wrong password 5 times, lock them out for 15 minutes
    public class LoginAttempt
    {
        [Key]
        public int AttemptId { get; set; }

        public int UserId { get; set; }
        [Required]
        [MaxLength(100)]
        public User User { get; set; } = null!;
        // Email address that tried to login
        [Required]
        [MaxLength(100)]
        public required string Email { get; set; }

        // IP address of the person trying to login
        [Required]
        [MaxLength(50)]
        public required string IpAddress { get; set; }

        // Was this login successful?
        public bool IsSuccessful { get; set; }

        // What went wrong? (e.g., "Wrong password", "Account locked")
        [MaxLength(200)]
        public string? FailureReason { get; set; }

        // When did this attempt happen?
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        // What browser/device was used?
        [MaxLength(500)]
        public string? UserAgent { get; set; }
    }
}
