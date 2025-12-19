using CitySecrets.Models;
using CitySecrets.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CitySecrets.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateReviewAsync(Review review, int userId)
        {
            var sentimentScore = await CalculateSentimentScoreAsync(review.Comment);

            review.UserId = userId;
            review.SentimentScore = sentimentScore;
            review.CreatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return review;
        }

        public async Task<Review> UpdateReviewAsync(int reviewId, Review updatedReview, int userId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) throw new Exception("Not found");

            review.Comment = updatedReview.Comment;
            review.Rating = updatedReview.Rating;
            review.SentimentScore = await CalculateSentimentScoreAsync(updatedReview.Comment);

            await _context.SaveChangesAsync();

            return review;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            review.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public Task<PagedResult<Review>> GetPlaceReviewsAsync(int placeId, Review filters)
        {
            return Task.FromResult(new PagedResult<Review>(new List<Review>(), 0, 1, 10));
        }

        public Task<PagedResult<Review>> GetUserReviewsAsync(int userId, int page, int pageSize)
        {
            return Task.FromResult(new PagedResult<Review>(new List<Review>(), 0, page, pageSize));
        }

        public Task<bool> MarkReviewHelpfulAsync(int reviewId, int userId, bool isHelpful)
            => Task.FromResult(true);

        public Task<double> CalculateSentimentScoreAsync(string reviewText)
            => Task.FromResult(0.5); // placeholder

        public Task<bool> FlagReviewAsync(int reviewId, int userId, string reason)
            => Task.FromResult(true);

        public Task<bool> ApproveReviewAsync(int reviewId, int adminUserId)
            => Task.FromResult(true);

        public Task<Review> GetReviewStatisticsAsync(int placeId)
        {
            return Task.FromResult(result: new Review());
        }
    }
}
