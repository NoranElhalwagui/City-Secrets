import api from './api';

export const reviewService = {
  // Get all reviews for a place
  getPlaceReviews: async (placeId) => {
    try {
      const response = await api.get(`/places/${placeId}/reviews`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch reviews',
        error: error.response?.data
      };
    }
  },

  // Create review
  createReview: async (placeId, reviewData) => {
    try {
      const response = await api.post(`/places/${placeId}/reviews`, reviewData);
      return {
        success: true,
        data: response.data,
        message: 'Review posted successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to post review',
        error: error.response?.data
      };
    }
  },

  // Update review
  updateReview: async (reviewId, reviewData) => {
    try {
      const response = await api.put(`/reviews/${reviewId}`, reviewData);
      return {
        success: true,
        data: response.data,
        message: 'Review updated successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to update review',
        error: error.response?.data
      };
    }
  },

  // Delete review
  deleteReview: async (reviewId) => {
    try {
      await api.delete(`/reviews/${reviewId}`);
      return {
        success: true,
        message: 'Review deleted successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to delete review',
        error: error.response?.data
      };
    }
  },

  // Like review
  likeReview: async (reviewId) => {
    try {
      const response = await api.post(`/reviews/${reviewId}/like`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to like review',
        error: error.response?.data
      };
    }
  },

  // Report/flag review
  flagReview: async (reviewId, reason) => {
    try {
      const response = await api.post(`/reviews/${reviewId}/flag`, { reason });
      return {
        success: true,
        data: response.data,
        message: 'Review reported. Thank you for your feedback.'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to report review',
        error: error.response?.data
      };
    }
  }
};

export default reviewService;