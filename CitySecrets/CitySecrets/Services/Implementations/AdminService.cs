using System;
using System.Collections.Generic;
using System.Linq;
using CitySecrets.Models;
using CitySecrets.Services.Interfaces;

namespace CitySecrets.Services
{
    public class AdminService : IAdminService
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

            _context.Place.Add(place);
                _context.SaveChanges();

                LogAction(1, "CREATE", "Place", place.PlaceId, $"Created place: {place.Name}");

                return place;
            }

            public Place? UpdatePlace(int placeId, Place updatedPlace)
            {
                var place = _context.Place.Find(placeId);
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
                var place = _context.Place.Find(placeId);
                if (place == null)
                    return false;

                if (hardDelete)
                {
                    _context.Place.Remove(place);
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
                var place = _context.Place.Find(placeId);
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
                var place = _context.Place.Find(placeId);
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
                    return _context.Place.ToList();

                return _context.Place.Where(p => !p.IsDeleted).ToList();
            }

            public Place? GetPlaceById(int placeId)
            {
                return _context.Place.Find(placeId);
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

                var placesCount = _context.Place
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

            public object GetFlaggedReviews(int page, int pageSize)
            {
                var query = _context.Reviews
                    .Where(r => r.IsFlagged && !r.IsDeleted)
                    .OrderByDescending(r => r.CreatedAt);

                var total = query.Count();
                var reviews = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new
                {
                    Data = reviews,
                    TotalCount = total,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize)
                };
            }

            public List<Review> GetAllReviews(bool includeDeleted = false)
            {
                if (includeDeleted)
                    return _context.Reviews.ToList();

                return _context.Reviews.Where(r => !r.IsDeleted).ToList();
            }

            // ============ USER MANAGEMENT ============

            public bool BanUser(int userId, int adminUserId, string reason)
            {
                var user = _context.Users.Find(userId);
                if (user == null)
                    return false;

                user.IsActive = false;

                _context.SaveChanges();

                LogAction(adminUserId, "BAN", "User", userId,
                    $"Banned user: {user.Username}. Reason: {reason}");

                return true;
            }

            public bool UnbanUser(int userId, int adminUserId)
            {
                var user = _context.Users.Find(userId);
                if (user == null)
                    return false;

                user.IsActive = true;

                _context.SaveChanges();

                LogAction(adminUserId, "UNBAN", "User", userId, $"Unbanned user: {user.Username}");

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

            public object GetAllUsers(int page, int pageSize, string? searchTerm, bool? isActive)
            {
                var query = _context.Users.Where(u => !u.IsDeleted);

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.ToLower();
                    query = query.Where(u => 
                        u.Username.ToLower().Contains(term) || 
                        u.Email.ToLower().Contains(term) ||
                        (u.FullName != null && u.FullName.ToLower().Contains(term)));
                }

                // Apply active filter
                if (isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == isActive.Value);
                }

                var total = query.Count();
                var users = query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new
                {
                    Data = users,
                    TotalCount = total,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize)
                };
            }

            // ============ STATISTICS ============

            public object GetDashboardStats()
            {
                return new
                {
                    TotalPlaces = _context.Place.Count(),
                    ActivePlaces = _context.Place.Count(p => p.IsActive && !p.IsDeleted),
                    DeletedPlaces = _context.Place.Count(p => p.IsDeleted),
                    HiddenGems = _context.Place.Count(p => p.IsHiddenGem && !p.IsDeleted),

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

            // ============ PENDING PLACES ============

            public object GetPendingPlaces(int page, int pageSize)
            {
                var query = _context.Place
                    .Where(p => !p.IsVerified && !p.IsDeleted)
                    .OrderByDescending(p => p.CreatedAt);

                var total = query.Count();
                var places = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new
                {
                    Data = places,
                    TotalCount = total,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize)
                };
            }

            public bool VerifyPlace(int placeId, int adminUserId)
            {
                var place = _context.Place.Find(placeId);
                if (place == null || place.IsDeleted)
                    return false;

                place.IsVerified = true;
                place.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                LogAction(adminUserId, "VERIFY", "Place", placeId, 
                    $"Verified place: {place.Name}");

                return true;
            }

            // ============ LOGS & ANALYTICS ============

            public object GetLogs(int page, int pageSize, string? actionType, 
                DateTime? startDate, DateTime? endDate)
            {
                var query = _context.AdminActionLogs.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(actionType))
                {
                    query = query.Where(log => log.ActionType == actionType);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(log => log.CreatedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(log => log.CreatedAt <= endDate.Value);
                }

                var total = query.Count();
                var logs = query
                    .OrderByDescending(log => log.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new
                {
                    Data = logs,
                    TotalCount = total,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(total / (double)pageSize)
                };
            }

            public object GetAnalytics(DateTime start, DateTime end)
            {
                var places = _context.Place
                    .Where(p => p.CreatedAt >= start && p.CreatedAt <= end);
                
                var reviews = _context.Reviews
                    .Where(r => r.CreatedAt >= start && r.CreatedAt <= end);
                
                var users = _context.Users
                    .Where(u => u.CreatedAt >= start && u.CreatedAt <= end);

                var placeViews = _context.PlaceViews
                    .Where(pv => pv.ViewedAt >= start && pv.ViewedAt <= end);

                return new
                {
                    Period = new { Start = start, End = end },
                    
                    Places = new
                    {
                        Total = places.Count(),
                        Verified = places.Count(p => p.IsVerified),
                        HiddenGems = places.Count(p => p.IsHiddenGem),
                        ByCategory = places
                            .GroupBy(p => p.CategoryId)
                            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                            .ToList()
                    },
                    
                    Reviews = new
                    {
                        Total = reviews.Count(),
                        Approved = reviews.Count(r => r.IsApproved),
                        Flagged = reviews.Count(r => r.IsFlagged),
                        AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0
                    },
                    
                    Users = new
                    {
                        NewUsers = users.Count(),
                        ActiveUsers = users.Count(u => u.IsActive),
                        TotalUsers = _context.Users.Count(u => !u.IsDeleted)
                    },
                    
                    Engagement = new
                    {
                        TotalViews = placeViews.Count(),
                        UniqueVisitors = placeViews.Select(pv => pv.UserId).Distinct().Count(),
                        AverageViewDuration = placeViews.Any() 
                            ? placeViews.Average(pv => pv.DurationSeconds ?? 0) 
                            : 0
                    }
                };
            }

            // ============ BULK OPERATIONS ============

            public int BulkDeletePlaces(List<int> placeIds, int adminUserId)
            {
                var places = _context.Place
                    .Where(p => placeIds.Contains(p.PlaceId) && !p.IsDeleted)
                    .ToList();

                foreach (var place in places)
                {
                    place.IsDeleted = true;
                    place.IsActive = false;
                    place.UpdatedAt = DateTime.UtcNow;
                }

                _context.SaveChanges();

                LogAction(adminUserId, "BULK_DELETE", "Place", null, 
                    $"Bulk deleted {places.Count} places");

                return places.Count;
            }

            public int BulkApproveReviews(List<int> reviewIds, int adminUserId)
            {
                var reviews = _context.Reviews
                    .Where(r => reviewIds.Contains(r.ReviewId) && !r.IsDeleted)
                    .ToList();

                foreach (var review in reviews)
                {
                    review.IsApproved = true;
                    review.IsFlagged = false;
                    review.UpdatedAt = DateTime.UtcNow;
                }

                _context.SaveChanges();

                LogAction(adminUserId, "BULK_APPROVE", "Review", null, 
                    $"Bulk approved {reviews.Count} reviews");

                return reviews.Count;
            }

            // ============ SYSTEM HEALTH ============

            public object GetSystemHealth()
            {
                var dbConnectionOk = false;
                try
                {
                    // Test database connection
                    var testQuery = _context.Users.Take(1).ToList();
                    dbConnectionOk = true;
                }
                catch
                {
                    dbConnectionOk = false;
                }

                var now = DateTime.UtcNow;
                var last24Hours = now.AddHours(-24);
                var last7Days = now.AddDays(-7);

                return new
                {
                    Status = dbConnectionOk ? "Healthy" : "Unhealthy",
                    Timestamp = now,
                    
                    Database = new
                    {
                        Connected = dbConnectionOk,
                        TotalRecords = new
                        {
                            Places = _context.Place.Count(),
                            Users = _context.Users.Count(),
                            Reviews = _context.Reviews.Count(),
                            Categories = _context.Categories.Count()
                        }
                    },
                    
                    Activity = new
                    {
                        Last24Hours = new
                        {
                            NewPlaces = _context.Place.Count(p => p.CreatedAt >= last24Hours),
                            NewReviews = _context.Reviews.Count(r => r.CreatedAt >= last24Hours),
                            NewUsers = _context.Users.Count(u => u.CreatedAt >= last24Hours),
                            AdminActions = _context.AdminActionLogs.Count(a => a.CreatedAt >= last24Hours)
                        },
                        Last7Days = new
                        {
                            NewPlaces = _context.Place.Count(p => p.CreatedAt >= last7Days),
                            NewReviews = _context.Reviews.Count(r => r.CreatedAt >= last7Days),
                            NewUsers = _context.Users.Count(u => u.CreatedAt >= last7Days)
                        }
                    },
                    
                    Issues = new
                    {
                        PendingPlaces = _context.Place.Count(p => !p.IsVerified && !p.IsDeleted),
                        FlaggedReviews = _context.Reviews.Count(r => r.IsFlagged && !r.IsDeleted),
                        InactiveUsers = _context.Users.Count(u => !u.IsActive && !u.IsDeleted)
                    }
                };
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