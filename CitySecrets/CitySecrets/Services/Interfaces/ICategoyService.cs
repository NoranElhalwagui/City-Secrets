using CitySecrets.Models;

namespace CitySecrets.Services.Interfaces
{
    public interface ICategoryService
    {
        // Get all active categories
        List<Category> GetAllCategories();
        
        // Get category by ID
        Category? GetCategoryById(int categoryId);
        
        // Create new category (admin only)
        Category CreateCategory(Category category, int adminUserId);
        
        // Update category (admin only)
        Category? UpdateCategory(int categoryId, Category category, int adminUserId);
        
        // Delete category (admin only, soft delete)
        bool DeleteCategory(int categoryId, int adminUserId);
    }
}
