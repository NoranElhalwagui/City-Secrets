import api from './api';

// Authentication Services
export const authService = {
  // Register new user
  register: async (userData) => {
    try {
      const response = await api.post('/auth/register', userData);
      return {
        success: true,
        data: response.data,
        message: 'Registration successful! Please check your email.'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Registration failed. Please try again.',
        error: error.response?.data
      };
    }
  },

  // Login user
  login: async (email, password) => {
    try {
      const response = await api.post('/auth/login', { email, password });
      
      // Store tokens and user data
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('user', JSON.stringify(response.data.user));
      
      return {
        success: true,
        user: response.data.user,
        message: 'Login successful!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Login failed. Please check your credentials.',
        error: error.response?.data
      };
    }
  },

  // Logout user
  logout: async () => {
    try {
      await api.post('/auth/logout');
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      // Clear local storage regardless
      localStorage.clear();
      window.location.href = '/';
    }
  },

  // Get current user profile
  getCurrentUser: async () => {
    try {
      const response = await api.get('/auth/me');
      localStorage.setItem('user', JSON.stringify(response.data));
      return {
        success: true,
        user: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to get user data'
      };
    }
  },

  // Update user profile
  updateProfile: async (profileData) => {
    try {
      const response = await api.put('/auth/profile', profileData);
      localStorage.setItem('user', JSON.stringify(response.data));
      return {
        success: true,
        user: response.data,
        message: 'Profile updated successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to update profile'
      };
    }
  },

  // Change password
  changePassword: async (oldPassword, newPassword) => {
    try {
      await api.post('/auth/change-password', { oldPassword, newPassword });
      return {
        success: true,
        message: 'Password changed successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to change password'
      };
    }
  },

  // Forgot password
  forgotPassword: async (email) => {
    try {
      await api.post('/auth/forgot-password', { email });
      return {
        success: true,
        message: 'Password reset instructions sent to your email.'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to process request'
      };
    }
  },

  // Reset password
  resetPassword: async (token, newPassword) => {
    try {
      await api.post('/auth/reset-password', { token, newPassword });
      return {
        success: true,
        message: 'Password reset successful! You can now login.'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to reset password'
      };
    }
  },

  // Verify email
  verifyEmail: async (token) => {
    try {
      await api.post('/auth/verify-email', { token });
      return {
        success: true,
        message: 'Email verified successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to verify email'
      };
    }
  },

  // Check if user is authenticated
  isAuthenticated: () => {
    const token = localStorage.getItem('accessToken');
    return !!token;
  },

  // Get current user from localStorage
  getCurrentUserFromStorage: () => {
    try {
      const userStr = localStorage.getItem('user');
      return userStr ? JSON.parse(userStr) : null;
    } catch {
      return null;
    }
  },

  // Check if user is admin
  isAdmin: () => {
    const user = authService.getCurrentUserFromStorage();
    return user?.role === 'Admin';
  }
};

// Export default for easier imports
export default authService;