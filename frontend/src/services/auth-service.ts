import { apiClient } from './api';
import { ApiResponse } from '@/types/api';
import { AuthResponse, User } from '@/types/auth';

export interface LoginParams {
  email: string;
  password: string;
}

export interface RegisterParams {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role?: string;
  classId?: string;
}

export const authService = {
  login: async (credentials: LoginParams): Promise<AuthResponse> => {
    const response = await apiClient.post<ApiResponse<AuthResponse>>('/auth/login', credentials);
    return response.data.data;
  },

  register: async (params: RegisterParams): Promise<AuthResponse> => {
    const response = await apiClient.post<ApiResponse<AuthResponse>>('/auth/register', params);
    return response.data.data;
  },

  getProfile: async (): Promise<User> => {
    const response = await apiClient.get<ApiResponse<User>>('/auth/me');
    return response.data.data;
  },

  revokeToken: async (refreshToken: string): Promise<void> => {
    await apiClient.post('/auth/revoke', { refreshToken });
  },
};
