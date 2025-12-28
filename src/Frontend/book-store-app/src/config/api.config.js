import axios from 'axios';

// API Base URL Configuration
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/api';

// Axios interceptor for handling authentication errors
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      console.warn('401 Unauthorized - clearing authentication data, redirecting to login');
      // Clear authentication data
      localStorage.removeItem('access');
      localStorage.removeItem('role');
      localStorage.removeItem('userId');
      localStorage.removeItem('authToken');
      // Redirect to login (cart will be cleared via backend logout endpoint)
      window.location.href = '/login';
      return Promise.reject(error);
    }
    return Promise.reject(error);
  }
);

export default API_BASE_URL;
