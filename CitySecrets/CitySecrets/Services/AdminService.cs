using System;
using System.Collections.Generic;
using System.Linq;
using CitySecrets.Models;
using CitySecrets.Data;

namespace CitySecrets.Services
{
    public class AdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
            {
                _context = context;
            }

            // ============ PLACE MANAGEMENT ============

            public Place CreatePlace(Place place)
            {
                place.IsActive = true;
                place.IsVerified = true;
                place.CreatedAt = DateTime.UtcNow;

                _context.Places.Add(place);
                _context.SaveChanges();

                LogAction(1, "CREATE", "Place", place.PlaceId, $"Created place: {place.Name}");

                return place;
            }

            public Place? UpdatePlace(int placeId, Place updatedPlace)
            {
                var place = _context.Places.Find(placeId);
                if (place == null || place.IsDeleted)
                    return null;

                place.Name = updatedPlace.Name;
                place.Description = updatedPlace.Description;
                place.Address = updatedPlace.Address;
                place.Latitude = updatedPlace.Latitude;
                place.Longitude = updatedPlace.Longitude;
                place.AveragePrice = updatedPlace.AveragePrice;
                place.PriceRange = updatedPlace.PriceRange;
                place.CategoryId = updatedPlace.CategoryId;
                place.PhoneNumber = updatedPlace.PhoneNumber;
                place.Website = updatedPlace.Website;
                place.Email = updatedPlace.Email;
                place.OpeningHours = updatedPlace.OpeningHours;
                place.IsHiddenGem = updatedPlace.IsHiddenGem;
                place.HiddenGemScore = updatedPlace.HiddenGemScore;
                place.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                LogAction(1, "UPDATE", "Place", placeId, $"Updated place: {place.Name}");

                return place;
            }

            public bool DeletePlace(int placeId, bool hardDelete = false)
            {
                var place = _context.Places.Find(placeId);
                if (place == null)
                    return false;

                if (hardDelete)
                {
                    _context.Places.Remove(place);
                    LogAction(1, "HARD_DELETE", "Place", placeId, $"Permanently deleted place: {place.Name}");
                }
                else
                {
                    place.IsDeleted = true;
                    place.IsActive = false;
                    place.UpdatedAt = DateTime.UtcNow;
                    LogAction(1, "SOFT_DELETE", "Place", placeId, $"Soft deleted place: {place.Name}");
                }

                _context.SaveChanges();
                return true;
            }

            public bool RestorePlace(int placeId)
            {
                var place = _context.Places.Find(placeId);
                if (place == null)
                    return false;

                place.IsDeleted = false;
                place.IsActive = true;
                place.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                LogAction(1, "RESTORE", "Place", placeId, $"Restored place: {place.Name}");

                return true;
            }

            public bool SetHiddenGemStatus(int placeId, bool isHiddenGem, int hiddenGemScore)
            {
                var place = _context.Places.Find(placeId);
                if (place == null || place.IsDeleted)
                    return false;

                place.IsHiddenGem = isHiddenGem;
                place.HiddenGemScore = hiddenGemScore;
                place.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                LogAction(1, "UPDATE_HIDDEN_GEM", "Place", placeId,
                    $"Updated hidden gem status for: {place.Name}");

                return true;
            }

            public List<Place> GetAllPlaces(bool includeDeleted = false)
            {
                if (includeDeleted)
                    return _context.Places.ToList();

                return _context.Places.Where(p => !p.IsDeleted).ToList();
            }

            public Place? GetPlaceById(int placeId)
            {
                return _context.Places.Find(placeId);
            }

            // ============ CATEGORY MANAGEMENT ============

            public Category CreateCategory(Category category)
            {
                category.CreatedAt = DateTime.UtcNow;

                _context.Categories.Add(category);
                _context.SaveChanges();

                LogAction(1, "CREATE", "Category", category.CategoryId,
                    $"Created category: {category.Name}");

                return category;
            }

            public Category? UpdateCategory(int categoryId, Category updatedCategory)
            {
                var category = _context.Categories.Find(categoryId);
                if (category == null)
                    return null;

                category.Name = updatedCategory.Name;
                category.Description = updatedCategory.Description;
                category.IconUrl = updatedCategory.IconUrl;

                _context.SaveChanges();

                LogAction(1, "UPDATE", "Category", categoryId,
                    $"Updated category: {category.Name}");

                return category;
            }

            public bool DeleteCategory(int categoryId)
            {
                var category = _context.Categories.Find(categoryId);
                if (category == null)
                    return false;

                var placesCount = _context.Places
                    .Count(p => p.CategoryId == categoryId && !p.IsDeleted);

                if (placesCount > 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot delete category. {placesCount} places are still using it.");
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();

                LogAction(1, "DELETE", "Category", categoryId,
                    $"Deleted category: {category.Name}");

                return true;
            }

            public List<Category> GetAllCategories()
            {
                return _context.Categories.ToList();
            }

            // ============ REVIEW MANAGEMENT ============

            public bool ApproveReview(int reviewId)
            {
                var review = _context.Reviews.Find(reviewId);
                if (review == null || review.IsDeleted)
                    return false;

                review.IsApproved = true;
                review.IsFlagged = false;
                review.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                LogAction(1, "APPROVE", "Review", reviewId, $"Approved review #{reviewId}");

                return true;
            }

            public bool RejectReview(int reviewId, string reason)
            {
                var review = _context.Reviews.Find(reviewId);
                if (review == null || review.IsDeleted)
                    return false;

                review.IsApproved = false;
                review.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                LogAction(1, "REJECT", "Review", reviewId,
                    $"Rejected review #{reviewId}. Reason: {reason}");

                return true;
            }

            public bool DeleteReview(int reviewId, bool hardDelete = false)
            {
                var review = _context.Reviews.Find(reviewId);
                if (review == null)
                    return false;

                if (hardDelete)
                {
                    _context.Reviews.Remove(review);
                    LogAction(1, "HARD_DELETE", "Review", reviewId,
                        $"Permanently deleted review #{reviewId}");
                }
                else
                {
                    review.IsDeleted = true;
                    review.IsApproved = false;
                    review.UpdatedAt = DateTime.UtcNow;
                    LogAction(1, "SOFT_DELETE", "Review", reviewId,
                        $"Soft deleted review #{reviewId}");
                }

                _context.SaveChanges();
                return true;
            }

            public List<Review> GetFlaggedReviews()
            {
                return _context.Reviews
                    .Where(r => r.IsFlagged && !r.IsDeleted)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
            }

            public List<Review> GetAllReviews(bool includeDeleted = false)
            {
                if (includeDeleted)
                    return _context.Reviews.ToList();

                return _context.Reviews.Where(r => !r.IsDeleted).ToList();
            }

            // ============ USER MANAGEMENT ============

            public bool BanUser(int userId, string reason)
            {
                var user = _context.Users.Find(userId);
                if (user == null)
                    return false;

                user.IsActive = false;

                _context.SaveChanges();

                LogAction(1, "BAN", "User", userId,
                    $"Banned user: {user.Username}. Reason: {reason}");

                return true;
            }

            public bool UnbanUser(int userId)
            {
                var user = _context.Users.Find(userId);
                if (user == null)
                    return false;

                user.IsActive = true;

                _context.SaveChanges();

                LogAction(1, "UNBAN", "User", userId, $"Unbanned user: {user.Username}");

                return true;
            }

            public bool MakeAdmin(int userId)
            {
                var user = _context.Users.Find(userId);
                if (user == null)
                    return false;

                user.IsAdmin = true;

                _context.SaveChanges();

                LogAction(1, "PROMOTE", "User", userId,
                    $"Promoted user to admin: {user.Username}");

                return true;
            }

            public bool RemoveAdmin(int userId)
            {
                var user = _context.Users.Find(userId);
                if (user == null)
                    return false;

                user.IsAdmin = false;

                _context.SaveChanges();

                LogAction(1, "DEMOTE", "User", userId,
                    $"Demoted user from admin: {user.Username}");

                return true;
            }

            public List<User> GetAllUsers()
            {
                return _context.Users.Where(u => !u.IsDeleted).ToList();
            }

            // ============ STATISTICS ============

            public object GetDashboardStats()
            {
                return new
                {
                    TotalPlaces = _context.Places.Count(),
                    ActivePlaces = _context.Places.Count(p => p.IsActive && !p.IsDeleted),
                    DeletedPlaces = _context.Places.Count(p => p.IsDeleted),
                    HiddenGems = _context.Places.Count(p => p.IsHiddenGem && !p.IsDeleted),

                    TotalUsers = _context.Users.Count(),
                    ActiveUsers = _context.Users.Count(u => u.IsActive && !u.IsDeleted),
                    BannedUsers = _context.Users.Count(u => !u.IsActive && !u.IsDeleted),

                    TotalReviews = _context.Reviews.Count(),
                    FlaggedReviews = _context.Reviews.Count(r => r.IsFlagged && !r.IsDeleted),

                    TotalCategories = _context.Categories.Count()
                };
            }

            public List<AdminActionLog> GetRecentActions(int count = 50)
            {
                return _context.AdminActionLogs
                    .OrderByDescending(log => log.CreatedAt)
                    .Take(count)
                    .ToList();
            }

            // ============ HELPER METHODS ============

            private void LogAction(int adminUserId, string actionType, string entityType,
                int? entityId, string description)
            {
                var log = new AdminActionLog
                {
                    AdminUserId = adminUserId,
                    ActionType = actionType,
                    EntityType = entityType,
                    EntityId = entityId,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AdminActionLogs.Add(log);
                _context.SaveChanges();
            }
        }
    }