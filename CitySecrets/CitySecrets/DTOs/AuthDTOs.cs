using System;
using System.ComponentModel.DataAnnotations;

namespace CitySecrets.DTOs
{
    // Authentication Result
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public UserDto? User { get; set; }
    }

    // User DTO
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsAdmin { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    // Request DTOs with validation

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = "";

        [StringLength(100)]
        public string FullName { get; set; } = "";
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";

        // 🛡️ For rate limiting and security tracking
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = "";
    }

    public class VerifyEmailRequest
    {
        [Required(ErrorMessage = "Verification token is required")]
        public string Token { get; set; } = "";
    }

    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = "";
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Reset token is required")]
        public string Token { get; set; } = "";

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; } = "";
    }

    public class UpdateProfileRequest
    {
        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? ProfilePicture { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; } = "";
    }
}
