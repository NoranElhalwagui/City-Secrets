using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CitySecrets.DTOs;
using CitySecrets.Services.Interfaces;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // 🏷️ CATEGORIES CONTROLLER
    // This handles categories for places (Restaurant, Cafe, Museum, etc.)
    // Anyone can view categories, only admins can create/edit/delete
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        
        // Constructor: Sets up the category service
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // 📋 GET: api/categories
        // What it does: Gets all categories
        // Why: Shows available place types (for filtering, creating places, etc.)
        // Returns: List of all active categories
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryService.GetAllCategories();
            return Ok(categories);
        }

        // � GET: api/categories/with-counts
        // What it does: Gets all categories with place counts
        // Why: Shows how many places exist in each category (useful for filter UI)
        // Returns: Categories with place counts
        // Example: "Restaurants (45)", "Cafes (23)"
        [HttpGet("with-counts")]
        public IActionResult GetCategoriesWithCounts()
        {
            var categoriesWithCounts = _categoryService.GetCategoriesWithPlaceCount();
            return Ok(categoriesWithCounts);
        }

        // �🔍 GET: api/categories/{id}
        // What it does: Gets details for one specific category
        // Why: Shows category info (name, description, icon, etc.)
        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            // If category doesn't exist or was deleted
            if (category == null)
                return NotFound(new { message = "Category not found" });
            
            return Ok(category);
        }

        // ➕ POST: api/categories
        // What it does: Creates a new category
        // Why: Admins can add new place types (like "Bakery", "Art Gallery")
        // Requires: Admin only
        // Needs: name, optional description and icon URL
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult CreateCategory([FromBody] CreateCategoryRequest request)
        {
            // Check if all required fields are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the admin ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Create the category
            var created = _categoryService.CreateCategory(request, userId);
            return CreatedAtAction(nameof(GetCategoryById), new { id = created.CategoryId }, created);
        }

        // ✏️ PUT: api/categories/{id}
        // What it does: Updates an existing category
        // Why: Admins can edit category name, description, icon, or active status
        // Requires: Admin only
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            // Check if all required fields are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the admin ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Update the category
            var updated = _categoryService.UpdateCategory(id, request, userId);
            
            // If category doesn't exist
            if (updated == null)
                return NotFound(new { message = "Category not found" });
            
            return Ok(updated);
        }

        // 🗑️ DELETE: api/categories/{id}
        // What it does: Deletes a category (soft delete - just hides it)
        // Why: Admins can remove unused or inappropriate categories
        // Requires: Admin only
        // Note: Existing places with this category remain unchanged
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteCategory(int id)
        {
            // Get the admin ID from their login token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            // Soft delete the category
            var result = _categoryService.DeleteCategory(id, userId);
            
            // If category doesn't exist
            if (!result)
                return NotFound(new { message = "Category not found" });
            
            return Ok(new { message = "Category deleted successfully" });
        }
    }
}