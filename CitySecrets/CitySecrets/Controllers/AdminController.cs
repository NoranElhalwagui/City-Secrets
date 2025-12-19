using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitySecrets.Models;
using CitySecrets.Services;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // This controller handles ALL admin operations for the City Secrets platform
    // Only users with admin role can access these endpoints
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]  // 🔒 Only admins can use these functions
    public class AdminController : ControllerBase
    {
        // This is our connection to the admin service that does all the work
        private readonly IAdminService _adminService;
        
        // Constructor: Sets up the admin service when this controller is created
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // 📊 GET: api/admin/dashboard
        // What it does: Shows admin the main statistics
        // Returns: Total places, users, reviews, etc.
        /// <summary>
        /// Get dashboard statistics for admin panel
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(DashboardStatsDto), 200)]
        public IActionResult GetDashboardStats()
        {
            // Ask the service to get all the stats
            var stats = _adminService.GetDashboardStats();
            // Send the stats back to whoever asked
            return Ok(stats);
        }

        // 🏢 GET: api/admin/pending-places
        // What it does: Gets places that are waiting for admin approval
        // Why: Users submit places, admin needs to verify them before they go live
        /// <summary>
        /// Get paginated list of pending places awaiting verification
        /// </summary>
        [HttpGet("pending-places")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetPendingPlaces([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            // Check if page numbers make sense (can't have page 0 or show 1000 items!)
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            // Get the pending places from the service
            var places = _adminService.GetPendingPlaces(page, pageSize);
            return Ok(places);
        }

        // 🚩 GET: api/admin/flagged-reviews
        // What it does: Shows reviews that users reported as inappropriate
        // Why: Users can flag bad reviews, admin checks and decides what to do
        /// <summary>
        /// Get paginated list of flagged reviews for moderation
        /// </summary>
        [HttpGet("flagged-reviews")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetFlaggedReviews([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            // Validate the page settings
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            // Get the flagged reviews to moderate
            var reviews = _adminService.GetFlaggedReviews(page, pageSize);
            return Ok(reviews);
        }

        // 🚫 POST: api/admin/users/{id}/ban
        // What it does: Bans a user who broke the rules
        // Example: POST api/admin/users/123/ban with reason "Spam"
        /// <summary>
        /// Ban a user from the platform
        /// </summary>
        [HttpPost("users/{id}/ban")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult BanUser(int id, [FromBody] BanRequest request)
        {
            // Check if the ban request has all required info (like reason)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the admin's ID (who is doing the banning)
            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Ban the user and record who did it and why
            var result = _adminService.BanUser(id, adminUserId, request.Reason);
            
            // If user doesn't exist, tell them
            if (!result)
                return NotFound(new { message = "User not found" });
            
            return Ok(new { message = "User banned successfully", userId = id });
        }

        // ✅ POST: api/admin/users/{id}/unban
        // What it does: Gives a banned user another chance
        // Example: POST api/admin/users/123/unban
        /// <summary>
        /// Unban a previously banned user
        /// </summary>
        [HttpPost("users/{id}/unban")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult UnbanUser(int id)
        {
            // Get who is unbanning (which admin)
            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Unban the user and track who unbanned them
            var result = _adminService.UnbanUser(id, adminUserId);
            
            // If user doesn't exist
            if (!result)
                return NotFound(new { message = "User not found" });
            
            return Ok(new { message = "User unbanned successfully", userId = id });
        }

        // 📝 GET: api/admin/logs
        // What it does: Shows history of what admins did (audit trail)
        // Example: Who banned who, who approved what, when it happened
        // Can filter by: action type (BAN, APPROVE, etc), date range
        /// <summary>
        /// Get paginated admin action logs with optional filtering
        /// </summary>
        [HttpGet("logs")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? actionType = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            // Validate pagination
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            // Get the logs with optional filters (action type, dates)
            var logs = _adminService.GetLogs(page, pageSize, actionType, startDate, endDate);
            return Ok(logs);
        }

        /// <summary>
        /// Get analytics data for a specified date range
        /// </summary>
        [HttpGet("analytics")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            
            if (end < start)
                return BadRequest(new { message = "End date must be after start date" });
            
            if ((end - start).TotalDays > 365)
                return BadRequest(new { message = "Date range cannot exceed 365 days" });
            
            var analytics = _adminService.GetAnalytics(start, end);
            return Ok(analytics);
        }

        /// <summary>
        /// Get all users with pagination and optional search/filter
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            var users = _adminService.GetAllUsers(page, pageSize, searchTerm, isActive);
            return Ok(users);
        }

        [HttpPost("places/bulk-delete")]
        public IActionResult BulkDeletePlaces([FromBody] BulkDeleteRequest request)
        {
            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var count = _adminService.BulkDeletePlaces(request.PlaceIds, adminUserId);
            return Ok(new { message = $"{count} places deleted successfully", count });
        }

        [HttpPost("reviews/bulk-approve")]
        public IActionResult BulkApproveReviews([FromBody] BulkApproveRequest request)
        {
            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var count = _adminService.BulkApproveReviews(request.ReviewIds, adminUserId);
            return Ok(new { message = $"{count} reviews approved successfully", count });
        }

        [HttpGet("health")]
        public IActionResult GetSystemHealth()
        {
            var health = _adminService.GetSystemHealth();
            return Ok(health);
        }

        [HttpGet("places")]
        public IActionResult GetAllPlaces([FromQuery] bool includeDeleted = false)
        {
            var places = _adminService.GetAllPlaces(includeDeleted);
            return Ok(places);
        }

        [HttpGet("places/{id}")]
        public IActionResult GetPlace(int id)
        {
            var place = _adminService.GetPlaceById(id);
            if (place == null)
                return NotFound("Place not found");
            
            return Ok(place);
        }

        [HttpPost("places")]
        public IActionResult CreatePlace([FromBody] Place place)
        {
            var created = _adminService.CreatePlace(place);
            return Ok(created);
        }

        [HttpPut("places/{id}")]
        public IActionResult UpdatePlace(int id, [FromBody] Place place)
        {
            var updated = _adminService.UpdatePlace(id, place);
            if (updated == null)
                return NotFound("Place not found");
            
            return Ok(updated);
        }

        [HttpDelete("places/{id}")]
        public IActionResult DeletePlace(int id, [FromQuery] bool hardDelete = false)
        {
            var result = _adminService.DeletePlace(id, hardDelete);
            if (!result)
                return NotFound("Place not found");
            
            return Ok(new { message = "Place deleted successfully" });
        }

        [HttpPost("places/{id}/restore")]
        public IActionResult RestorePlace(int id)
        {
            var result = _adminService.RestorePlace(id);
            if (!result)
                return NotFound("Place not found");
            
            return Ok(new { message = "Place restored successfully" });
        }

        [HttpPost("places/{id}/verify")]
        public IActionResult VerifyPlace(int id)
        {
            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = _adminService.VerifyPlace(id, adminUserId);
            
            if (!result)
                return NotFound("Place not found");
            
            return Ok(new { message = "Place verified successfully" });
        }

        [HttpPost("places/{id}/hidden-gem")]
        public IActionResult SetHiddenGem(int id, [FromBody] HiddenGemRequest request)
        {
            var result = _adminService.SetHiddenGemStatus(id, request.IsHiddenGem, request.Score);
            if (!result)
                return NotFound("Place not found");
            
            return Ok(new { message = "Hidden gem status updated" });
        }

        [HttpGet("categories")]
        public IActionResult GetAllCategories()
        {
            var categories = _adminService.GetAllCategories();
            return Ok(categories);
        }

        [HttpPost("categories")]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            var created = _adminService.CreateCategory(category);
            return Ok(created);
        }

        [HttpPut("categories/{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            var updated = _adminService.UpdateCategory(id, category);
            if (updated == null)
                return NotFound("Category not found");
            
            return Ok(updated);
        }

        [HttpDelete("categories/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var result = _adminService.DeleteCategory(id);
                if (!result)
                    return NotFound("Category not found");
                
                return Ok(new { message = "Category deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("reviews")]
        public IActionResult GetAllReviews([FromQuery] bool includeDeleted = false)
        {
            var reviews = _adminService.GetAllReviews(includeDeleted);
            return Ok(reviews);
        }

        [HttpPost("reviews/{id}/approve")]
        public IActionResult ApproveReview(int id)
        {
            var result = _adminService.ApproveReview(id);
            if (!result)
                return NotFound("Review not found");
            
            return Ok(new { message = "Review approved" });
        }

        [HttpPost("reviews/{id}/reject")]
        public IActionResult RejectReview(int id, [FromBody] RejectRequest request)
        {
            var result = _adminService.RejectReview(id, request.Reason);
            if (!result)
                return NotFound("Review not found");
            
            return Ok(new { message = "Review rejected" });
        }

        [HttpDelete("reviews/{id}")]
        public IActionResult DeleteReview(int id, [FromQuery] bool hardDelete = false)
        {
            var result = _adminService.DeleteReview(id, hardDelete);
            if (!result)
                return NotFound("Review not found");
            
            return Ok(new { message = "Review deleted" });
        }

        [HttpPost("users/{id}/make-admin")]
        public IActionResult MakeAdmin(int id)
        {
            var result = _adminService.MakeAdmin(id);
            if (!result)
                return NotFound("User not found");
            
            return Ok(new { message = "User promoted to admin" });
        }

        [HttpPost("users/{id}/remove-admin")]
        public IActionResult RemoveAdmin(int id)
        {
            var result = _adminService.RemoveAdmin(id);
            if (!result)
                return NotFound("User not found");
            
            return Ok(new { message = "User demoted from admin" });
        }
    }

    public class HiddenGemRequest
    {
        public bool IsHiddenGem { get; set; }
        public int Score { get; set; }
    }

    public class RejectRequest
    {
        public string Reason { get; set; } = "";
    }

    public class BanRequest
    {
        public string Reason { get; set; } = "";
    }

    public class BulkDeleteRequest
    {
        public List<int> PlaceIds { get; set; } = new();
    }

    public class BulkApproveRequest
    {
        public List<int> ReviewIds { get; set; } = new();
    }
}