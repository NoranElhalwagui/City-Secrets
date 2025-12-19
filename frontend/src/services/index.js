// Export all services from one file for easy imports
import authService from './authService';
import placeService from './placeService';
import adminService from './adminService';
import reviewService from './reviewService';
import api from './api';

// Centralized exports
export {
  authService,
  placeService,
  adminService,
  reviewService,
  api
};

// Default export with all services
export default {
  auth: authService,
  places: placeService,
  admin: adminService,
  reviews: reviewService,
  api
};