import axios from 'axios';

const API_BASE_URL = 'https://api-townly.runasp.net/';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token and ngrok bypass header to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  
  // Add ngrok bypass header
  config.headers['ngrok-skip-browser-warning'] = 'true';
  
  // Add authorization if token exists
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  
  return config;
});

// Handle token expiration
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;