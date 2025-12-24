import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5293/api" // 🔁 adjust port if needed
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
