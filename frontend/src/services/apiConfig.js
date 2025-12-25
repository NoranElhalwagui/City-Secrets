import axios from "axios";

// Use environment variable for production, localhost for development
const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || "http://localhost:5293/api"
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
