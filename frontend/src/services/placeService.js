import api from './api';

export const placeService = {
  // Get all places
  getAllPlaces: async (params = {}) => {
    try {
      const response = await api.get('/places', { params });
      return {
        success: true,
        data: response.data,
        total: response.headers['x-total-count'] || response.data.length
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch places',
        error: error.response?.data
      };
    }
  },

  // Get place by ID
  getPlaceById: async (id) => {
    try {
      const response = await api.get(`/places/${id}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch place details',
        error: error.response?.data
      };
    }
  },

  // Create new place
  createPlace: async (placeData) => {
    try {
      // Handle FormData for file uploads
      let formData;
      let headers = {};
      
      if (placeData.images && placeData.images.length > 0) {
        formData = new FormData();
        Object.keys(placeData).forEach(key => {
          if (key === 'images') {
            placeData.images.forEach((image, index) => {
              if (image.file) {
                formData.append('images', image.file);
              }
            });
          } else {
            formData.append(key, placeData[key]);
          }
        });
        headers['Content-Type'] = 'multipart/form-data';
      } else {
        formData = placeData;
      }

      const response = await api.post('/places', formData, { headers });
      return {
        success: true,
        data: response.data,
        message: 'Place created successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to create place',
        error: error.response?.data
      };
    }
  },

  // Update place
  updatePlace: async (id, placeData) => {
    try {
      const response = await api.put(`/places/${id}`, placeData);
      return {
        success: true,
        data: response.data,
        message: 'Place updated successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to update place',
        error: error.response?.data
      };
    }
  },

  // Delete place
  deletePlace: async (id) => {
    try {
      await api.delete(`/places/${id}`);
      return {
        success: true,
        message: 'Place deleted successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to delete place',
        error: error.response?.data
      };
    }
  },

  // Search places
  searchPlaces: async (query, filters = {}) => {
    try {
      const response = await api.get('/places/search', {
        params: { query, ...filters }
      });
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to search places',
        error: error.response?.data
      };
    }
  },

  // Get places by category
  getPlacesByCategory: async (categoryId) => {
    try {
      const response = await api.get(`/places/category/${categoryId}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch places by category',
        error: error.response?.data
      };
    }
  },

  // Get nearby places
  getNearbyPlaces: async (latitude, longitude, radius = 10) => {
    try {
      const response = await api.get('/places/nearby', {
        params: { latitude, longitude, radius }
      });
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to fetch nearby places',
        error: error.response?.data
      };
    }
  },

  // Add review to place
  addReview: async (placeId, reviewData) => {
    try {
      const response = await api.post(`/places/${placeId}/reviews`, reviewData);
      return {
        success: true,
        data: response.data,
        message: 'Review added successfully!'
      };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to add review',
        error: error.response?.data
      };
    }
  },

  // Like/Unlike place
  toggleLike: async (placeId) => {
    try {
      const response = await api.post(`/places/${placeId}/like`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to update like',
        error: error.response?.data
      };
    }
  },

  // Add to favorites
  addToFavorites: async (placeId) => {
    try {
      const response = await api.post(`/places/${placeId}/favorite`);
      return {
        success: true,
        data: response.data,
        message: 'Added to favorites!'
      };
    } catch (error) {
      return {
        success: false,
        message: 'Failed to add to favorites',
        error: error.response?.data
      };
    }
  }
};

export default placeService;