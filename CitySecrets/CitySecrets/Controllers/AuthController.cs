using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitySecrets.Services;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // 🔐 AUTHENTICATION CONTROLLER
    // This handles user sign up, login, and account management
    // Anyone can access these (no login required for register/login)
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Connection to the auth service that handles passwords, tokens, etc.
        private readonly IAuthService _authService;
        
        // Constructor: Sets up the auth service
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // 👤 POST: api/auth/register
        // What it does: Creates a new user account
        // Needs: username, email, password, full name
        // Returns: User info + login token
        /// <summary>
        /// Register a new user account
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Check if all required fields are filled correctly
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Try to create the account
            var result = _authService.Register(request);
            // If registration failed (like email already exists)
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            
            // Success! Send back user info and token
            return Ok(result);
        }

        // 🔓 POST: api/auth/login
        // 🔓 POST: api/auth/login
        // What it does: Logs in an existing user
        // Needs: email and password
        // Returns: User info + access token (like a key to use the app)
        // Security: Tracks failed attempts, locks account after 5 failures
        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Check if email and password are provided
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 🛡️ Add IP address and user agent for security tracking
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            // Try to login
            var result = _authService.Login(request);
            // If login failed (wrong password, email not found, account locked, etc.)
            if (!result.Success)
                return Unauthorized(new { message = result.Message });
            
            // Success! Send back user info and tokens to access the app
            return Ok(result);
        }

        // 🔄 POST: api/auth/refresh-token
        // What it does: Gets a new access token when the old one expires
        // Why: Tokens expire after 24 hours, this renews without re-login
        // Needs: refresh token (lasts 30 days)
        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            // Check if refresh token is provided
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Try to get a new access token
            var result = _authService.RefreshToken(request.RefreshToken);
            // If refresh token is invalid or expired
            if (!result.Success)
                return Unauthorized(new { message = result.Message });
            
            // Success! Send back new access token
            return Ok(result);
        }

        // ✉️ POST: api/auth/verify-email
        // What it does: Confirms user's email address
        // Why: Makes sure the email is real and belongs to them
        // Needs: verification token (sent to their email)
        // Note: Check email for verification link after registration
        /// <summary>
        /// Verify email address with token
        /// </summary>
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            // Try to verify the email with the token
            var result = _authService.VerifyEmail(request.Token);
            // If token is invalid or expired
            if (!result)
                return BadRequest(new { message = "Invalid or expired verification token. Please request a new one." });
            
            // Success! Email is verified, user can now write reviews
            return Ok(new { message = "Email verified successfully! You can now write reviews." });
        }

        // 🔑 POST: api/auth/forgot-password
        // What it does: Sends a password reset link to user's email
        // Why: Helps users who forgot their password
        // Needs: email address
        // Security: Doesn't reveal if email exists
        /// <summary>
        /// Request password reset link
        /// </summary>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            // Check if email is provided
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Send reset link if email exists (doesn't reveal if email exists for security)
            _authService.ForgotPassword(request.Email);
            
            // Always return success (even if email doesn't exist) for security
            return Ok(new { message = "If the email exists, a password reset link has been sent. Check your inbox!" });
        }

        // 🔄 POST: api/auth/reset-password
        // What it does: Sets a new password using reset token
        // Why: Allows user to create new password after forgot-password
        // Needs: reset token (from email) + new password
        // Link expires: After 1 hour
        /// <summary>
        /// Reset password with token
        /// </summary>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // Check if all fields are filled
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Try to reset password with the token
            var result = _authService.ResetPassword(request);
            // If token is invalid or expired
            if (!result)
                return BadRequest(new { message = "Invalid or expired reset token. Please request a new password reset." });
            
            // Success! Password changed, all devices logged out for security
            return Ok(new { message = "Password reset successfully! Please login with your new password." });
        }

        // 👤 GET: api/auth/me
        // What it does: Gets the logged-in user's profile info
        // Why: To show user their own data (name, email, preferences)
        // Requires: User must be logged in (needs valid token)
        /// <summary>
        /// Get current authenticated user profile
        /// </summary>
        [HttpGet("me")]
        [Authorize]  // Must be logged in
        public IActionResult GetCurrentUser()
        {
            // Get the user ID from their login token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // If token is invalid or doesn't have user ID
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            // Get the user's info from database
            var user = _authService.GetCurrentUser(userId);
            
            // If user doesn't exist anymore
            if (user == null)
                return NotFound(new { message = "User not found" });
            
            // Send back user's profile
            return Ok(user);
        }

        // ✏️ PUT: api/auth/profile
        // What it does: Updates user's profile information
        // Why: Lets users change their name, bio, preferences, etc.
        // Requires: User must be logged in
        // Needs: Updated profile data (full name, bio, etc.)
        /// <summary>
        /// Update user profile information
        /// </summary>
        [HttpPut("profile")]
        [Authorize]  // Must be logged in
        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            // Check if all fields are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the user ID from their login token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            // Try to update the profile
            var result = _authService.UpdateProfile(userId, request);
            
            // If update failed
            if (!result)
                return BadRequest(new { message = "Profile update failed" });
            
            // Success! Profile updated
            return Ok(new { message = "Profile updated successfully" });
        }

        // 🚪 POST: api/auth/logout
        // What it does: Logs out the user
        // Why: For security - ends the user's session
        // Note: For JWT tokens, the app just deletes the token from storage
        // Requires: User must be logged in
        /// <summary>
        /// Logout current user (client should clear tokens)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]  // Must be logged in
        public IActionResult Logout()
        {
            // In production, invalidate refresh token in database
            return Ok(new { message = "Logged out successfully" });
        }

        // 🔒 POST: api/auth/change-password
        // What it does: Changes user's password
        // Why: Allows users to update their password for security
        // Requires: User must be logged in
        // Needs: current password (for security) + new password
        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]  // Must be logged in
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Check if all fields are filled
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the user ID from their login token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid token" });

            // Try to change the password (verifies current password first)
            var result = _authService.ChangePassword(userId, request);
            
            // If current password is wrong or new password is invalid
            if (!result)
                return BadRequest(new { message = "Current password is incorrect or new password is invalid" });
            
            // Success! Password changed
            return Ok(new { message = "Password changed successfully" });
        }
    }
}