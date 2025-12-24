using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CitySecrets.Models;
using CitySecrets.DTOs;
using CitySecrets.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;  // 🔒 For secure password hashing

namespace CitySecrets.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        // ⚙️ Security Settings - How strong is our security?
        private const int MaxFailedAttempts = 5;  // Lock account after 5 wrong passwords
        private const int LockoutMinutes = 15;  // Lock for 15 minutes
        private const int VerificationTokenHours = 24;  // Email verification link expires in 24 hours
        private const int ResetTokenHours = 1;  // Password reset link expires in 1 hour
        
        // 🔄 Read token expiration from appsettings.json (so you can change without recompiling)
        private int RefreshTokenDays => _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays", 30);

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public AuthResult Register(RegisterRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return new AuthResult { Success = false, Message = "Email and password are required" };

            if (request.Password.Length < 6)
                return new AuthResult { Success = false, Message = "Password must be at least 6 characters" };

            // Check if email already exists
            if (_context.Users.Any(u => u.Email == request.Email && !u.IsDeleted))
                return new AuthResult { Success = false, Message = "Email already exists" };

            // Check if username already exists
            if (_context.Users.Any(u => u.Username == request.Username && !u.IsDeleted))
                return new AuthResult { Success = false, Message = "Username already exists" };

            // 🔒 Hash password with BCrypt (super secure!)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // ✉️ Generate email verification token
            var verificationToken = GenerateSecureToken();

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                IsActive = true,
                IsAdmin = false,
                EmailVerified = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(VerificationTokenHours),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // 🎫 Generate access token and refresh token
            var accessToken = GenerateAccessToken(user);
            var refreshToken = CreateRefreshToken(user.UserId, null, null);

            // TODO: Send verification email to user.Email with verificationToken
            // Example: "Click here to verify: https://yourapp.com/verify?token={verificationToken}"

            return new AuthResult
            {
                Success = true,
                Message = "Registration successful! Please check your email to verify your account.",
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = MapUserToDto(user)
            };
        }


        public AuthResult Login(LoginRequest request)
        {
            // Find user by email
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email && !u.IsDeleted);

            if (user == null)
            {
                // Track failed login attempt (even if user doesn't exist)
                TrackLoginAttempt(request.Email, false, "User not found", request.IpAddress, request.UserAgent);
                return new AuthResult { Success = false, Message = "Invalid email or password" };
            }

            // 🛡️ Check if account is locked due to too many failed attempts
            if (user.AccountLockedUntil.HasValue && user.AccountLockedUntil > DateTime.UtcNow)
            {
                var remainingMinutes = (int)(user.AccountLockedUntil.Value - DateTime.UtcNow).TotalMinutes;
                return new AuthResult
                {
                    Success = false,
                    Message = $"Account is locked due to too many failed login attempts. Try again in {remainingMinutes} minutes."
                };
            }

            // 🔓 Check if account is active
            if (!user.IsActive)
            {
                TrackLoginAttempt(request.Email, false, "Account deactivated", request.IpAddress, request.UserAgent);
                return new AuthResult { Success = false, Message = "Account is deactivated" };
            }

            // 🔑 Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                // Wrong password! Increase failed attempts counter
                user.FailedLoginAttempts++;

                // Lock account if too many failures
                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.AccountLockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    _context.SaveChanges();
                    TrackLoginAttempt(request.Email, false, "Too many failed attempts - Account locked", request.IpAddress, request.UserAgent);
                    return new AuthResult
                    {
                        Success = false,
                        Message = $"Too many failed login attempts. Account locked for {LockoutMinutes} minutes."
                    };
                }

                _context.SaveChanges();
                TrackLoginAttempt(request.Email, false, "Wrong password", request.IpAddress, request.UserAgent);
                return new AuthResult { Success = false, Message = "Invalid email or password" };
            }

            // ✅ Login successful! Reset failed attempts
            user.FailedLoginAttempts = 0;
            user.AccountLockedUntil = null;
            user.LastLoginAt = DateTime.UtcNow;
            _context.SaveChanges();

            // Track successful login
            TrackLoginAttempt(request.Email, true, null, request.IpAddress, request.UserAgent);

            // 🎫 Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = CreateRefreshToken(user.UserId, request.IpAddress, request.UserAgent);

            return new AuthResult
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = MapUserToDto(user)
            };
        }

        public AuthResult RefreshToken(string refreshToken)
        {
            // 🔍 Find the refresh token in database
            var tokenRecord = _context.RefreshTokens
                .FirstOrDefault(t => t.Token == refreshToken && t.IsActive);

            // Token not found or expired?
            if (tokenRecord == null || tokenRecord.ExpiresAt < DateTime.UtcNow)
            {
                return new AuthResult { Success = false, Message = "Invalid or expired refresh token" };
            }

            // Get the user
            var user = _context.Users.FirstOrDefault(u => u.UserId == tokenRecord.UserId && !u.IsDeleted);
            if (user == null || !user.IsActive)
            {
                return new AuthResult { Success = false, Message = "User not found or inactive" };
            }

            // 🔄 Update token last used time
            tokenRecord.LastUsedAt = DateTime.UtcNow;
            _context.SaveChanges();

            // 🎫 Generate new access token
            var newAccessToken = GenerateAccessToken(user);

            return new AuthResult
            {
                Success = true,
                Message = "Token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = refreshToken,  // Same refresh token
                User = MapUserToDto(user)
            };
        }

        public bool VerifyEmail(string token)
        {
            // 🔍 Find user with this verification token
            var user = _context.Users.FirstOrDefault(u =>
                u.EmailVerificationToken == token &&
                u.EmailVerificationTokenExpiry > DateTime.UtcNow &&
                !u.IsDeleted);

            if (user == null)
                return false;  // Token invalid or expired

            // ✅ Mark email as verified
            user.EmailVerified = true;
            user.EmailVerificationToken = null;  // Remove token (can't use again)
            user.EmailVerificationTokenExpiry = null;
            _context.SaveChanges();

            return true;
        }

        public void ForgotPassword(string email)
        {
            // Find user
            var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);

            // 🤐 Don't reveal if email exists (security best practice)
            if (user == null)
                return;

            // 🔑 Generate password reset token
            var resetToken = GenerateSecureToken();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(ResetTokenHours);
            _context.SaveChanges();

            // TODO: Send email with reset link
            // Example: "Reset your password: https://yourapp.com/reset-password?token={resetToken}"
            // The email should say: "This link expires in 1 hour"
        }

        public bool ResetPassword(ResetPasswordRequest request)
        {
            // 🔍 Find user with this reset token
            var user = _context.Users.FirstOrDefault(u =>
                u.PasswordResetToken == request.Token &&
                u.PasswordResetTokenExpiry > DateTime.UtcNow &&
                !u.IsDeleted);

            if (user == null)
                return false;  // Token invalid or expired

            // Validate new password
            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
                return false;

            // 🔒 Hash new password with BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // 🧹 Clear reset token (can't use again)
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            // 🛡️ Reset failed login attempts
            user.FailedLoginAttempts = 0;
            user.AccountLockedUntil = null;

            // 🔒 Revoke all existing refresh tokens (force re-login on all devices)
            var userTokens = _context.RefreshTokens.Where(t => t.UserId == user.UserId && t.IsActive);
            foreach (var token in userTokens)
            {
                token.IsActive = false;
            }

            _context.SaveChanges();
            return true;
        }

        public UserDto? GetCurrentUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId && !u.IsDeleted);
            return user == null ? null : MapUserToDto(user);
        }

        public bool UpdateProfile(int userId, UpdateProfileRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId && !u.IsDeleted);
            if (user == null)
                return false;

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.ProfilePicture))
                user.ProfileImageUrl = request.ProfilePicture;

            _context.SaveChanges();
            return true;
        }

        public bool ChangePassword(int userId, ChangePasswordRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId && !u.IsDeleted);
            if (user == null)
                return false;

            // 🔑 Verify current password with BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return false;

            // Validate new password
            if (request.NewPassword.Length < 6)
                return false;

            // 🔒 Update password with BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _context.SaveChanges();
            return true;
        }

        // 🔧 HELPER METHODS - Private methods used by functions above

        /// <summary>
        /// 🔄 Creates a refresh token and saves it to database
        /// Refresh tokens let users stay logged in for 30 days
        /// </summary>
        private RefreshToken CreateRefreshToken(int userId, string? ipAddress, string? userAgent)
        {
            // Generate random secure token
            var tokenString = GenerateSecureToken();

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = tokenString,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IpAddress = ipAddress,
                DeviceInfo = userAgent
            };

            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            return refreshToken;
        }

        /// <summary>
        /// 🎲 Generates a secure random token (for refresh tokens, verification, password reset)
        /// </summary>
        private string GenerateSecureToken()
        {
            var randomBytes = new byte[64];  // 64 bytes = 512 bits (super secure!)
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        /// <summary>
        /// 🎫 Generates JWT access token
        /// This token is sent with every API request to prove user is logged in
        /// Expiration time is read from appsettings.json
        /// </summary>
        private string GenerateAccessToken(User user)
        {
            // Claims = Information stored in the token (used for authorization policies)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new Claim("EmailVerified", user.EmailVerified.ToString())  // For VerifiedUser policy
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 🕐 Read expiration time from config (default 1440 minutes = 24 hours)
            var expirationMinutes = _configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes", 1440);
            
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "CitySecrets",
                audience: _configuration["Jwt:Audience"] ?? "CitySecretsUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 📊 Tracks login attempts for security monitoring and rate limiting
        /// Helps detect brute force attacks
        /// </summary>
        private void TrackLoginAttempt(string email, bool isSuccessful, string? failureReason, string ipAddress, string userAgent)
        {
            // Try to find user, but it's OK if they don't exist
            var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
            if (user == null && !isSuccessful)
            {
                return; // Don't track failed attempts for non-existent users
            }
            var attempt = new LoginAttempt
            {
                Email = email,
                IsSuccessful = isSuccessful,
                FailureReason = failureReason,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                AttemptedAt = DateTime.UtcNow,
                UserId = user?.UserId      // <-- null if user not found, safe now
            };

            _context.LoginAttempts.Add(attempt);

            _context.SaveChanges();
        }
        /// <summary>
        /// 👤 Converts User model to UserDto (removes sensitive info like password)
        /// </summary>
        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                ProfileImageUrl = user.ProfileImageUrl,
                IsAdmin = user.IsAdmin,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }

}
