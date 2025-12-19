using CitySecrets.Models;
using CitySecrets.DTOs;

namespace CitySecrets.Services.Interfaces
{
    public interface IAdminService
    {
        // Place Management
        Place CreatePlace(Place place);
        Place? UpdatePlace(int placeId, Place updatedPlace);
        bool DeletePlace(int placeId, bool hardDelete = false);
        bool VerifyPlace(int placeId, int adminUserId);
        bool RestorePlace(int placeId);
        bool SetHiddenGemStatus(int placeId, bool isHiddenGem, int hiddenGemScore);
        List<Place> GetAllPlaces(bool includeDeleted = false);
        Place? GetPlaceById(int placeId);
        object GetPendingPlaces(int page, int pageSize);
        int BulkDeletePlaces(List<int> placeIds, int adminUserId);
        
        // Category Management
        Category CreateCategory(Category category);
        Category? UpdateCategory(int categoryId, Category updatedCategory);
        bool DeleteCategory(int categoryId);
        List<Category> GetAllCategories();
        
        // Review Management
        bool ApproveReview(int reviewId);
        bool RejectReview(int reviewId, string reason);
        bool DeleteReview(int reviewId, bool hardDelete = false);
        List<Review> GetAllReviews(bool includeDeleted = false);
        object GetFlaggedReviews(int page, int pageSize);
        int BulkApproveReviews(List<int> reviewIds, int adminUserId);
        
        // User Management
        bool BanUser(int userId, int adminUserId, string reason);
        bool UnbanUser(int userId, int adminUserId);
        bool MakeAdmin(int userId);
        bool RemoveAdmin(int userId);
        object GetAllUsers(int page, int pageSize, string? searchTerm, bool? isActive);
        
        // Dashboard & Analytics
        object GetDashboardStats();
        object GetLogs(int page, int pageSize, string? actionType, DateTime? startDate, DateTime? endDate);
        object GetAnalytics(DateTime start, DateTime end);
        object GetSystemHealth();
    }
}
