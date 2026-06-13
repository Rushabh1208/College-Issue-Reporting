import axios from "axios";
import { API_BASE_URL } from "../constants/api";
import { useAuthStore } from "../../features/auth/store/authStore";

export class ApiError extends Error {
  constructor(message, status, details) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.details = details;
  }
}

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 20000,
  headers: {
    Accept: "application/json"
  }
});

apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const status = error.response?.status;
    const data = error.response?.data;

    if (status === 401) {
      useAuthStore.getState().logout();
    }

    let message = "Something went wrong. Please try again.";
    if (Array.isArray(data)) message = data.join(", ");
    else if (typeof data === "string" && data.trim()) message = data;
    else if (data?.message) message = data.message;
    else if (data?.title) message = data.title;
    else if (status === 403) message = "You do not have permission to perform this action.";
    else if (status === 429) message = "Too many requests. Please wait a minute and try again.";
    else if (error.code === "ECONNABORTED") message = "The request timed out. Please retry.";
    else if (!error.response) message = "Network connection failed. Check your internet and retry.";

    return Promise.reject(new ApiError(message, status, data));
  }
);
