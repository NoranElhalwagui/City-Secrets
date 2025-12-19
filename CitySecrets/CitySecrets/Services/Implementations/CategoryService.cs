using CitySecrets.Models;
using CitySecrets.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CitySecrets.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.Name)
                .ToList();
        }

        public Category? GetCategoryById(int categoryId)
        {
            return _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId && !c.IsDeleted);
        }

        public Category CreateCategory(Category category, int adminUserId)
        {
            category.CreatedAt = DateTime.UtcNow;
            category.IsDeleted = false;
            category.IsActive = true;

            _context.Categories.Add(category);
            _context.SaveChanges();

            return category;
        }

        public Category? UpdateCategory(int categoryId, Category category, int adminUserId)
        {
            var existing = _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId && !c.IsDeleted);

            if (existing == null)
                return null;

            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.IconUrl = category.IconUrl;
            existing.IsActive = category.IsActive;

            _context.SaveChanges();
            return existing;
        }

        public bool DeleteCategory(int categoryId, int adminUserId)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId && !c.IsDeleted);

            if (category == null)
                return false;

            // Soft delete
            category.IsDeleted = true;
            _context.SaveChanges();

            return true;
        }
    }
}
