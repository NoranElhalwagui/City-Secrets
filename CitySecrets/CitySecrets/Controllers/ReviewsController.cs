using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitySecrets.Services.Interfaces;
using CitySecrets.DTOs;
using CitySecrets.Models;
using System.Security.Claims;

namespace CitySecrets.Controllers
{
    // ⭐ REVIEWS CONTROLLER
    // This handles all operations for reviews and ratings
    // Users can read reviews, write reviews, rate places, and mark helpful reviews
    /// <summary>
    /// Controller for managing reviews and ratings
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        // Connections to services and database
        private readonly IReviewService _reviewService;
        private readonly AppDbContext _context;
        
        // Constructor: Sets up the review service and database connection
        public ReviewsController(IReviewService reviewService, AppDbContext context)
        {
            _reviewService = reviewService;
            _context = context;
        }

        // 📋 GET: api/reviews/place/{placeId}
        // What it does: Gets all reviews for a specific place
        // Why: Shows what people think about a place (ratings, comments, experiences)
        // Can filter by: rating (1-5 stars), date, verified visits
        // Can sort by: newest, oldest, highest/lowest rating, most helpful
        // Returns: Paginated list of reviews
        /// <summary>
        /// Get all reviews for a specific place with filtering and pagination
        /// </summary>
        [HttpGet("place/{placeId}")]
        [ProducesResponseType(typeof(PaginatedReviewsResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPlaceReviews(int placeId, [FromQuery] ReviewFilterRequest filters)
        {
            // Check if filter parameters are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if the place exists and is not deleted
            var place = await _context.Place.FindAsync(placeId);
            if (place == null || place.IsDeleted)
                return NotFound(new { message = "Place not found" });

            // Get only approved and non-deleted reviews for this place
            var query = _context.Reviews
                .Where(r => r.PlaceId == placeId && !r.IsDeleted && r.IsApproved);

            // Apply rating filters
            if (filters.MinRating.HasValue)
                query = query.Where(r => r.Rating >= filters.MinRating.Value);
            
            if (filters.MaxRating.HasValue)
                query = query.Where(r => r.Rating <= filters.MaxRating.Value);
            
            // Filter for verified visits (reviews with visit date)
            if (filters.IsVerifiedVisit.HasValue)
                query = query.Where(r => r.VisitDate.HasValue == filters.IsVerifiedVisit.Value);
            
            // Filter by date range
            if (filters.FromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= filters.FromDate.Value);
            
            if (filters.ToDate.HasValue)
                query = query.Where(r => r.CreatedAt <= filters.ToDate.Value);

            // Sort reviews based on user preference
            query = filters.SortBy?.ToLower() switch
            {
                "oldest" => query.OrderBy(r => r.CreatedAt),  // Oldest first
                "highest" => query.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt),  // Best ratings first
                "lowest" => query.OrderBy(r => r.Rating).ThenByDescending(r => r.CreatedAt),  // Worst ratings first
                "mosthelpful" => query.OrderByDescending(r =>   // Most helpful reviews first
                    _context.ReviewHelpfulness.Count(h => h.ReviewId == r.ReviewId && h.IsHelpful))
                    .ThenByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt) // Default: Newest first
            };

            var total = await query.CountAsync();
            var reviews = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    PlaceId = r.PlaceId,
                    PlaceName = place.Name,
                    UserId = r.UserId,
                    Username = r.User != null ? r.User.Username : "Unknown",
                    UserProfileImage = r.User != null ? r.User.ProfileImageUrl : null,
                    Rating = r.Rating,
                    Title = r.Title,
                    Comment = r.Comment,
                    VisitDate = r.VisitDate,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    IsApproved = r.IsApproved,
                    IsFlagged = r.IsFlagged,
                    HelpfulCount = _context.ReviewHelpfulness.Count(h => h.ReviewId == r.ReviewId && h.IsHelpful),
                    NotHelpfulCount = _context.ReviewHelpfulness.Count(h => h.ReviewId == r.ReviewId && !h.IsHelpful),
                    SentimentScore = r.SentimentScore
                })
                .ToListAsync();

            var result = new PaginatedReviewsResult
            {
                Data = reviews,
                TotalCount = total,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)filters.PageSize)
            };

            return Ok(result);
        }

        /// <summary>
        /// Get all reviews created by a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(PaginatedReviewsResult), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUserReviews(
            int userId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Invalid pagination parameters" });

            var query = _context.Reviews
                .Where(r => r.UserId == userId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt);

            var total = await query.CountAsync();
            var reviews = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    PlaceId = r.PlaceId,
                    PlaceName = r.Place != null ? r.Place.Name : "Unknown",
                    UserId = r.UserId,
                    Username = r.User != null ? r.User.Username : "Unknown",
                    Rating = r.Rating,
                    Title = r.Title,
                    Comment = r.Comment,
                    VisitDate = r.VisitDate,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    IsApproved = r.IsApproved,
                    IsFlagged = r.IsFlagged,
                    HelpfulCount = _context.ReviewHelpfulness.Count(h => h.ReviewId == r.ReviewId && h.IsHelpful),
                    NotHelpfulCount = _context.ReviewHelpfulness.Count(h => h.ReviewId == r.ReviewId && !h.IsHelpful),
                    SentimentScore = r.SentimentScore
                })
                .ToListAsync();

            var result = new PaginatedReviewsResult
            {
                Data = reviews,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };

            return Ok(result);
        }

        /// <summary>
        /// Create a new review for a place
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ReviewDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if place exists
            var place = await _context.Place.FindAsync(request.PlaceId);
            if (place == null || place.IsDeleted)
                return NotFound(new { message = "Place not found" });

            // Check if user already reviewed this place
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.PlaceId == request.PlaceId && r.UserId == userId && !r.IsDeleted);
            
            if (existingReview != null)
                return BadRequest(new { message = "You have already reviewed this place. Please update your existing review instead." });

            try
            {
                var review = new Review
                {
                    PlaceId = request.PlaceId,
                    UserId = userId,
                    Rating = request.Rating,
                    Title = request.Title,
                    Comment = request.Comment,
                    VisitDate = request.VisitDate,
                    CreatedAt = DateTime.UtcNow,
                    IsApproved = false, // Requires admin approval
                    IsFlagged = false,
                    IsDeleted = false
                };

                var createdReview = await _reviewService.CreateReviewAsync(review, userId);

                var reviewDto = new ReviewDto
                {
                    ReviewId = createdReview.ReviewId,
                    PlaceId = createdReview.PlaceId,
                    PlaceName = place.Name,
                    UserId = userId,
                    Rating = createdReview.Rating,
                    Title = createdReview.Title,
                    Comment = createdReview.Comment,
                    VisitDate = createdReview.VisitDate,
                    CreatedAt = createdReview.CreatedAt,
                    IsApproved = createdReview.IsApproved,
                    SentimentScore = createdReview.SentimentScore
                };

                return CreatedAtAction(nameof(GetPlaceReviews), new { placeId = request.PlaceId }, reviewDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to create review: {ex.Message}" });
            }
        }

        /// <summary>
        /// Update an existing review (only by the review author)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var review = await _context.Reviews.Include(r => r.Place).FirstOrDefaultAsync(r => r.ReviewId == id);
            if (review == null || review.IsDeleted)
                return NotFound(new { message = "Review not found" });

            // Check if user owns this review
            if (review.UserId != userId)
                return Forbid();

            try
            {
                // Update fields if provided
                if (request.Rating.HasValue)
                    review.Rating = request.Rating.Value;
                
                if (!string.IsNullOrWhiteSpace(request.Title))
                    review.Title = request.Title;
                
                if (!string.IsNullOrWhiteSpace(request.Comment))
                {
                    review.Comment = request.Comment;
                    // Recalculate sentiment score
                    review.SentimentScore = await _reviewService.CalculateSentimentScoreAsync(request.Comment);
                }
                
                if (request.VisitDate.HasValue)
                    review.VisitDate = request.VisitDate;

                review.UpdatedAt = DateTime.UtcNow;
                review.IsApproved = false; // Re-approval required after edit

                await _context.SaveChangesAsync();

                var reviewDto = new ReviewDto
                {
                    ReviewId = review.ReviewId,
                    PlaceId = review.PlaceId,
                    PlaceName = review.Place?.Name ?? "Unknown",
                    UserId = review.UserId,
                    Rating = review.Rating,
                    Title = review.Title,
                    Comment = review.Comment,
                    VisitDate = review.VisitDate,
                    CreatedAt = review.CreatedAt,
                    UpdatedAt = review.UpdatedAt,
                    IsApproved = review.IsApproved,
                    SentimentScore = review.SentimentScore
                };

                return Ok(reviewDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to update review: {ex.Message}" });
            }
        }

        /// <summary>
        /// Delete a review (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.HasClaim("IsAdmin", "true");

            var review = await _context.Reviews.FindAsync(id);
            if (review == null || review.IsDeleted)
                return NotFound(new { message = "Review not found" });

            // Check if user owns this review or is admin
            if (review.UserId != userId && !isAdmin)
                return Forbid();

            try
            {
                var result = await _reviewService.DeleteReviewAsync(id, userId);
                if (!result)
                    return NotFound(new { message = "Review not found" });
                
                return Ok(new { message = "Review deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to delete review: {ex.Message}" });
            }
        }

        /// <summary>
        /// Mark a review as helpful or not helpful
        /// </summary>
        [HttpPost("{id}/helpful")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarkReviewHelpful(int id, [FromBody] MarkHelpfulRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var review = await _context.Reviews.FindAsync(id);
            if (review == null || review.IsDeleted)
                return NotFound(new { message = "Review not found" });

            try
            {
                // Check if user already voted
                var existingVote = await _context.ReviewHelpfulness
                    .FirstOrDefaultAsync(h => h.ReviewId == id && h.UserId == userId);

                if (existingVote != null)
                {
                    // Update existing vote
                    existingVote.IsHelpful = request.IsHelpful;
                    existingVote.VotedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new vote
                    var helpfulness = new ReviewHelpfulness
                    {
                        ReviewId = id,
                        UserId = userId,
                        IsHelpful = request.IsHelpful,
                        VotedAt = DateTime.UtcNow
                    };
                    _context.ReviewHelpfulness.Add(helpfulness);
                }

                await _context.SaveChangesAsync();

                var helpfulCount = await _context.ReviewHelpfulness.CountAsync(h => h.ReviewId == id && h.IsHelpful);
                var notHelpfulCount = await _context.ReviewHelpfulness.CountAsync(h => h.ReviewId == id && !h.IsHelpful);

                return Ok(new 
                { 
                    message = "Review helpfulness updated",
                    helpfulCount,
                    notHelpfulCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to update helpfulness: {ex.Message}" });
            }
        }

        /// <summary>
        /// Flag a review for moderation
        /// </summary>
        [HttpPost("{id}/flag")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> FlagReview(int id, [FromBody] FlagReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var review = await _context.Reviews.FindAsync(id);
            if (review == null || review.IsDeleted)
                return NotFound(new { message = "Review not found" });

            try
            {
                review.IsFlagged = true;
                review.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Log the flag action (you could create a separate FlagLog table)
                await _reviewService.FlagReviewAsync(id, userId, request.Reason);

                return Ok(new { message = "Review flagged for moderation. Our team will review it shortly." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to flag review: {ex.Message}" });
            }
        }

        /// <summary>
        /// Approve a review (Admin only)
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ApproveReview(int id)
        {
            var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var review = await _context.Reviews.FindAsync(id);
            if (review == null || review.IsDeleted)
                return NotFound(new { message = "Review not found" });

            try
            {
                review.IsApproved = true;
                review.IsFlagged = false;
                review.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await _reviewService.ApproveReviewAsync(id, adminUserId);

                return Ok(new { message = "Review approved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to approve review: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get comprehensive review statistics for a place
        /// </summary>
        [HttpGet("place/{placeId}/stats")]
        [ProducesResponseType(typeof(ReviewStatisticsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReviewStatistics(int placeId)
        {
            var place = await _context.Place.FindAsync(placeId);
            if (place == null || place.IsDeleted)
                return NotFound(new { message = "Place not found" });

            var reviews = _context.Reviews
                .Where(r => r.PlaceId == placeId && !r.IsDeleted && r.IsApproved);

            var totalReviews = await reviews.CountAsync();
            
            if (totalReviews == 0)
            {
                return Ok(new ReviewStatisticsDto
                {
                    TotalReviews = 0,
                    AverageRating = 0,
                    RatingDistribution = new Dictionary<int, int>
                    {
                        { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
                    }
                });
            }

            var averageRating = await reviews.AverageAsync(r => (double)r.Rating);
            
            var ratingDistribution = await reviews
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Rating, x => x.Count);

            // Fill in missing ratings with 0
            for (int i = 1; i <= 5; i++)
            {
                if (!ratingDistribution.ContainsKey(i))
                    ratingDistribution[i] = 0;
            }

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var reviewsLast30Days = await reviews.CountAsync(r => r.CreatedAt >= thirtyDaysAgo);

            var averageSentiment = totalReviews > 0 
                ? await reviews.AverageAsync(r => r.SentimentScore) 
                : 0;

            var verifiedVisitCount = await reviews.CountAsync(r => r.VisitDate.HasValue);

            var totalHelpfulVotes = await _context.ReviewHelpfulness
                .Where(h => reviews.Select(r => r.ReviewId).Contains(h.ReviewId) && h.IsHelpful)
                .CountAsync();

            var stats = new ReviewStatisticsDto
            {
                TotalReviews = totalReviews,
                AverageRating = Math.Round(averageRating, 2),
                RatingDistribution = ratingDistribution,
                ReviewsLast30Days = reviewsLast30Days,
                AverageSentimentScore = Math.Round(averageSentiment, 2),
                VerifiedVisitCount = verifiedVisitCount,
                TotalHelpfulVotes = totalHelpfulVotes
            };

            return Ok(stats);
        }

        /// <summary>
        /// Get a single review by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReviewDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Place)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewId == id && !r.IsDeleted);

            if (review == null)
                return NotFound(new { message = "Review not found" });

            var reviewDto = new ReviewDto
            {
                ReviewId = review.ReviewId,
                PlaceId = review.PlaceId,
                PlaceName = review.Place?.Name ?? "Unknown",
                UserId = review.UserId,
                Username = review.User?.Username ?? "Unknown",
                UserProfileImage = review.User?.ProfileImageUrl,
                Rating = review.Rating,
                Title = review.Title,
                Comment = review.Comment,
                VisitDate = review.VisitDate,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                IsApproved = review.IsApproved,
                IsFlagged = review.IsFlagged,
                HelpfulCount = await _context.ReviewHelpfulness.CountAsync(h => h.ReviewId == id && h.IsHelpful),
                NotHelpfulCount = await _context.ReviewHelpfulness.CountAsync(h => h.ReviewId == id && !h.IsHelpful),
                SentimentScore = review.SentimentScore
            };

            return Ok(reviewDto);
        }
    }
}