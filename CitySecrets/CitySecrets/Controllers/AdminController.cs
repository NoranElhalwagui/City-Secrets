using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using CitySecrets.Models;
using CitySecrets.Services;

namespace CitySecrets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        
        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        // ============ PLACE ENDPOINTS ============
        
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

        [HttpPost("places/{id}/hidden-gem")]
        public IActionResult SetHiddenGem(int id, [FromBody] HiddenGemRequest request)
        {
            var result = _adminService.SetHiddenGemStatus(id, request.IsHiddenGem, request.Score);
            if (!result)
                return NotFound("Place not found");
            
            return Ok(new { message = "Hidden gem status updated" });
        }

        // ============ CATEGORY ENDPOINTS ============
        
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

        // ============ REVIEW ENDPOINTS ============
        
        [HttpGet("reviews")]
        public IActionResult GetAllReviews([FromQuery] bool includeDeleted = false)
        {
            var reviews = _adminService.GetAllReviews(includeDeleted);
            return Ok(reviews);
        }

        [HttpGet("reviews/flagged")]
        public IActionResult GetFlaggedReviews()
        {
            var reviews = _adminService.GetFlaggedReviews();
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

        // ============ USER ENDPOINTS ============
        
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var users = _adminService.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("users/{id}/ban")]
        public IActionResult BanUser(int id, [FromBody] BanRequest request)
        {
            var result = _adminService.BanUser(id, request.Reason);
            if (!result)
                return NotFound("User not found");
            
            return Ok(new { message = "User banned" });
        }

        [HttpPost("users/{id}/unban")]
        public IActionResult UnbanUser(int id)
        {
            var result = _adminService.UnbanUser(id);
            if (!result)
                return NotFound("User not found");
            
            return Ok(new { message = "User unbanned" });
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

        // ============ STATISTICS ENDPOINTS ============
        
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var stats = _adminService.GetDashboardStats();
            return Ok(stats);
        }

        [HttpGet("logs")]
        public IActionResult GetLogs([FromQuery] int count = 50)
        {
            var logs = _adminService.GetRecentActions(count);
            return Ok(logs);
        }
    }

    // ============ REQUEST MODELS ============
    
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
}