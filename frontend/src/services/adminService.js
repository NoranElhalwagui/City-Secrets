
import api from './api';

export const adminService = {
  // Dashboard statistics
  getDashboardStats: async () => {
    try {
      const response = await api.get('/admin/dashboard');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch dashboard stats',
        error: error.response?.data
      };
    }
  },

  // Pending places for approval
  getPendingPlaces: async () => {
    try {
      const response = await api.get('/admin/pending-places');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch pending places',
        error: error.response?.data
      };
    }
  },

  // Get all users
  getAllUsers: async () => {
    try {
      const response = await api.get('/admin/users');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch users',
        error: error.response?.data
      };
    }
  },

  // Get all places (admin view)
  getAllPlacesAdmin: async () => {
    try {
      const response = await api.get('/admin/places');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch places',
        error: error.response?.data
      };
    }
  },

  // Approve place
  approvePlace: async (placeId) => {
    try {
      const response = await api.post(`/admin/places/${placeId}/verify`);
      return {
        success: true,
        data: response.data,
        message: 'Place approved successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to approve place',
        error: error.response?.data
      };
    }
  },

  // Reject place
  rejectPlace: async (placeId) => {
    try {
      const response = await api.delete(`/admin/places/${placeId}`);
      return {
        success: true,
        message: 'Place rejected successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to reject place',
        error: error.response?.data
      };
    }
  },

  // Ban user
  banUser: async (userId) => {
    try {
      const response = await api.post(`/admin/users/${userId}/ban`);
      return {
        success: true,
        data: response.data,
        message: 'User banned successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to ban user',
        error: error.response?.data
      };
    }
  },

  // Unban user
  unbanUser: async (userId) => {
    try {
      const response = await api.post(`/admin/users/${userId}/unban`);
      return {
        success: true,
        data: response.data,
        message: 'User unbanned successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to unban user',
        error: error.response?.data
      };
    }
  },

  // Make user admin
  makeAdmin: async (userId) => {
    try {
      const response = await api.post(`/admin/users/${userId}/make-admin`);
      return {
        success: true,
        data: response.data,
        message: 'User is now an admin!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to make user admin',
        error: error.response?.data
      };
    }
  },

  // Remove admin privileges
  removeAdmin: async (userId) => {
    try {
      const response = await api.post(`/admin/users/${userId}/remove-admin`);
      return {
        success: true,
        data: response.data,
        message: 'Admin privileges removed!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to remove admin privileges',
        error: error.response?.data
      };
    }
  },

  // Get flagged reviews
  getFlaggedReviews: async () => {
    try {
      const response = await api.get('/admin/flagged-reviews');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch flagged reviews',
        error: error.response?.data
      };
    }
  },

  // Get analytics data
  getAnalytics: async (period = 'month') => {
    try {
      const response = await api.get('/admin/analytics', {
        params: { period }
      });
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch analytics',
        error: error.response?.data
      };
    }
  },

  // Get system health
  getSystemHealth: async () => {
    try {
      const response = await api.get('/admin/health');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch system health',
        error: error.response?.data
      };
    }
  },

  // Bulk delete places
  bulkDeletePlaces: async (placeIds) => {
    try {
      const response = await api.post('/admin/places/bulk-delete', { placeIds });
      return {
        success: true,
        data: response.data,
        message: 'Places deleted successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to delete places',
        error: error.response?.data
      };
    }
  }
};

export default adminService;