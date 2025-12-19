using CitySecrets.Models;
using CitySecrets.DTOs;
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

        public List<CategoryDto> GetAllCategories()
        {
            return _context.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    IconUrl = c.IconUrl,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToList();
        }

        public List<CategoryWithCountDto> GetCategoriesWithPlaceCount()
        {
            return _context.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .Select(c => new CategoryWithCountDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    IconUrl = c.IconUrl,
                    PlaceCount = _context.Place.Count(p => p.CategoryId == c.CategoryId && !p.IsDeleted && p.IsApproved)
                })
                .OrderBy(c => c.Name)
                .ToList();
        }

        public CategoryDto? GetCategoryById(int categoryId)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId && !c.IsDeleted);

            if (category == null)
                return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                IconUrl = category.IconUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public CategoryDto CreateCategory(CreateCategoryRequest request, int adminUserId)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                IconUrl = request.IconUrl,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                IconUrl = category.IconUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public CategoryDto? UpdateCategory(int categoryId, UpdateCategoryRequest request, int adminUserId)
        {
            var existing = _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId && !c.IsDeleted);

            if (existing == null)
                return null;

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.IconUrl = request.IconUrl;
            existing.IsActive = request.IsActive;

            _context.SaveChanges();

            return new CategoryDto
            {
                CategoryId = existing.CategoryId,
                Name = existing.Name,
                Description = existing.Description,
                IconUrl = existing.IconUrl,
                IsActive = existing.IsActive,
                CreatedAt = existing.CreatedAt
            };
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
