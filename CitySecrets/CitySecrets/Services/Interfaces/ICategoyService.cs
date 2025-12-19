using CitySecrets.Models;
using CitySecrets.DTOs;

namespace CitySecrets.Services.Interfaces
{
    public interface ICategoryService
    {
        // Get all active categories
        List<CategoryDto> GetAllCategories();
        
        // Get all categories with place counts (useful for filtering UI)
        List<CategoryWithCountDto> GetCategoriesWithPlaceCount();
        
        // Get category by ID
        CategoryDto? GetCategoryById(int categoryId);
        
        // Create new category (admin only)
        CategoryDto CreateCategory(CreateCategoryRequest request, int adminUserId);
        
        // Update category (admin only)
        CategoryDto? UpdateCategory(int categoryId, UpdateCategoryRequest request, int adminUserId);
        
        // Delete category (admin only, soft delete)
        bool DeleteCategory(int categoryId, int adminUserId);
    }
}
