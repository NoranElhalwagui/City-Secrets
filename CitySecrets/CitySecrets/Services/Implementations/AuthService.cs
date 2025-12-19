using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CitySecrets.Models;
using CitySecrets.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CitySecrets.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

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
            if (_context.Users.Any(u => u.Email == request.Email))
                return new AuthResult { Success = false, Message = "Email already exists" };

            // Check if username already exists
            if (_context.Users.Any(u => u.Username == request.Username))
                return new AuthResult { Success = false, Message = "Username already exists" };

            // Hash password
            var passwordHash = HashPassword(request.Password);

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
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthResult
            {
                Success = true,
                Message = "Registration successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = MapUserToDto(user)
            };
        }

        public AuthResult Login(LoginRequest request)
        {
            // Find user by email
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email && !u.IsDeleted);

            if (user == null)
                return new AuthResult { Success = false, Message = "Invalid email or password" };

            // Verify password
            if (!VerifyPassword(request.Password, user.PasswordHash))
                return new AuthResult { Success = false, Message = "Invalid email or password" };

            // Check if account is active
            if (!user.IsActive)
                return new AuthResult { Success = false, Message = "Account is deactivated" };

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            _context.SaveChanges();

            // Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthResult
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = MapUserToDto(user)
            };
        }

        public AuthResult RefreshToken(string refreshToken)
        {
            // In production, store refresh tokens in database with expiration
            // For now, return error - implement token storage later
            return new AuthResult { Success = false, Message = "Refresh token functionality requires database implementation" };
        }

        public bool VerifyEmail(string token)
        {
            // Implement email verification logic
            // In production, store verification tokens in database
            return false;
        }

        public void ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
            if (user == null)
                return; // Don't reveal if email exists

            // In production:
            // 1. Generate reset token
            // 2. Store token in database with expiration
            // 3. Send email with reset link
        }

        public bool ResetPassword(ResetPasswordRequest request)
        {
            // In production:
            // 1. Validate reset token
            // 2. Check if token is expired
            // 3. Update user password
            // 4. Invalidate token
            return false;
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

            // Verify current password
            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
                return false;

            // Validate new password
            if (request.NewPassword.Length < 6)
                return false;

            // Update password
            user.PasswordHash = HashPassword(request.NewPassword);
            _context.SaveChanges();
            return true;
        }

        // Helper methods

        private string HashPassword(string password)
        {
            // In production, use BCrypt or ASP.NET Core Identity
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "CitySecrets",
                audience: _configuration["Jwt:Audience"] ?? "CitySecretsUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

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
