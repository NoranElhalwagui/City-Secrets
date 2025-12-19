using System.ComponentModel.DataAnnotations;

namespace CitySecrets.DTOs
{
    // DTO for returning category data
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO for category with place count (for filtering UI)
    public class CategoryWithCountDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int PlaceCount { get; set; }
    }

    // DTO for creating a new category
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string Name { get; set; } = "";

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessage = "Icon URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Icon URL must be a valid URL")]
        public string? IconUrl { get; set; }
    }

    // DTO for updating a category
    public class UpdateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string Name { get; set; } = "";

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessage = "Icon URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Icon URL must be a valid URL")]
        public string? IconUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
