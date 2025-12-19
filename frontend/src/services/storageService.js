// Local Storage Service for consistent storage handling
export const storageService = {
  // Set item with expiration
  setItem: (key, value, expiresInMinutes = null) => {
    try {
      const item = {
        value,
        timestamp: Date.now(),
        expires: expiresInMinutes ? Date.now() + (expiresInMinutes * 60 * 1000) : null
      };
      localStorage.setItem(key, JSON.stringify(item));
    } catch (error) {
      console.error('Error saving to localStorage:', error);
    }
  },

  // Get item with expiration check
  getItem: (key) => {
    try {
      const itemStr = localStorage.getItem(key);
      if (!itemStr) return null;

      const item = JSON.parse(itemStr);
      
      // Check if item has expired
      if (item.expires && Date.now() > item.expires) {
        localStorage.removeItem(key);
        return null;
      }

      return item.value;
    } catch (error) {
      console.error('Error reading from localStorage:', error);
      return null;
    }
  },

  // Remove item
  removeItem: (key) => {
    try {
      localStorage.removeItem(key);
    } catch (error) {
      console.error('Error removing from localStorage:', error);
    }
  },

  // Clear all storage
  clear: () => {
    try {
      localStorage.clear();
    } catch (error) {
      console.error('Error clearing localStorage:', error);
    }
  },

  // Get user data
  getUser: () => {
    return storageService.getItem('user');
  },

  // Get access token
  getToken: () => {
    return storageService.getItem('accessToken');
  },

  // Set user data
  setUser: (user) => {
    storageService.setItem('user', user);
  },

  // Set tokens
  setTokens: (accessToken, refreshToken) => {
    // Access token expires in 15 minutes
    storageService.setItem('accessToken', accessToken, 15);
    // Refresh token expires in 7 days
    storageService.setItem('refreshToken', refreshToken, 7 * 24 * 60);
  },

  // Clear auth data
  clearAuth: () => {
    storageService.removeItem('accessToken');
    storageService.removeItem('refreshToken');
    storageService.removeItem('user');
  }
};

export default storageService;