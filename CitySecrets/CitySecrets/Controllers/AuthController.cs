using Microsoft.AspNetCore.Mvc;
using CitySecrets.Models;
using CitySecrets.Data;
using System.Linq;

namespace CitySecrets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Find user by email
            var user = _context.Users
                .FirstOrDefault(u => u.Email == request.Email && !u.IsDeleted);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Check password (in production, use proper hashing!)
            if (user.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Account is inactive" });
            }

            // Return user info including isAdmin status
            return Ok(new
            {
                userId = user.UserId,
                username = user.Username,
                email = user.Email,
                isAdmin = user.IsAdmin,  // THIS IS THE KEY - tells frontend if user is admin
                fullName = user.FullName,
                message = "Login successful"
            });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}