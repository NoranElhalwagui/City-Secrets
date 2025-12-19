using CitySecrets.Models;
namespace CitySecrets.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Review> CreateReviewAsync(Review review, int userId);
        Task<Review> UpdateReviewAsync(int reviewId, Review review, int userId);
        Task<bool> DeleteReviewAsync(int reviewId, int userId);
        Task<PagedResult<Review>> GetPlaceReviewsAsync(int placeId, Review filters);
        Task<PagedResult<Review>> GetUserReviewsAsync(int userId, int page, int pageSize);
        Task<bool> MarkReviewHelpfulAsync(int reviewId, int userId, bool isHelpful);
        Task<double> CalculateSentimentScoreAsync(string reviewText);
        Task<bool> FlagReviewAsync(int reviewId, int userId, string reason);
        Task<bool> ApproveReviewAsync(int reviewId, int adminUserId);
        Task<Review> GetReviewStatisticsAsync(int placeId);
    }
}