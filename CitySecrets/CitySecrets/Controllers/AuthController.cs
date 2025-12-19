using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitySecrets.Services;
using CitySecrets.DTOs;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary> // way of commenting in c# where the IDEs can understand not only the humans
        /// Register a new user account
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _authService.Register(request);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            
            return Ok(result);
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _authService.Login(request);
            if (!result.Success)
                return Unauthorized(new { message = result.Message });
            
            return Ok(result);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _authService.RefreshToken(request.RefreshToken);
            if (!result.Success)
                return Unauthorized(new { message = "Invalid refresh token" });
            
            return Ok(result);
        }

        /// <summary>
        /// Verify email address with token
        /// </summary>
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var result = _authService.VerifyEmail(request.Token);
            if (!result)
                return BadRequest(new { message = "Invalid or expired verification token" });
            
            return Ok(new { message = "Email verified successfully" });
        }

        /// <summary>
        /// Request password reset link
        /// </summary>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _authService.ForgotPassword(request.Email);
            return Ok(new { message = "If the email exists, a reset link has been sent" });
        }

        /// <summary>
        /// Reset password with token
        /// </summary>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _authService.ResetPassword(request);
            if (!result)
                return BadRequest(new { message = "Invalid or expired reset token" });
            
            return Ok(new { message = "Password reset successfully" });
        }

        /// <summary>
        /// Get current authenticated user profile
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            var user = _authService.GetCurrentUser(userId);
            
            if (user == null)
                return NotFound(new { message = "User not found" });
            
            return Ok(user);
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            var result = _authService.UpdateProfile(userId, request);
            
            if (!result)
                return BadRequest(new { message = "Profile update failed" });
            
            return Ok(new { message = "Profile updated successfully" });
        }

        /// <summary>
        /// Logout current user (client should clear tokens)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // In production, invalidate refresh token in database
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            var result = _authService.ChangePassword(userId, request);
            
            if (!result)
                return BadRequest(new { message = "Current password is incorrect or new password is invalid" });
            
            return Ok(new { message = "Password changed successfully" });
        }
    }
}