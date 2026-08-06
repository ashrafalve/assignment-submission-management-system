import { apiClient } from './api';
import { ApiResponse, PagedResponse, PaginationParams } from '@/types/api';
import { User, UserRole } from '@/types/auth';
import { SchoolClass, Subject, TeacherSubject } from '@/types/domain';

// User DTOs
export interface CreateUserPayload {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: UserRole;
  classId?: string;
}

export interface UpdateUserPayload {
  firstName?: string;
  lastName?: string;
  role?: UserRole;
  isActive?: boolean;
  classId?: string;
}

export interface ChangePasswordPayload {
  newPassword: string;
  confirmPassword: string;
}

// Class DTOs
export interface CreateClassPayload {
  name: string;
  academicYear: string;
  description?: string;
}

export interface UpdateClassPayload {
  name?: string;
  academicYear?: string;
  description?: string;
  isActive?: boolean;
}

// Subject DTOs
export interface CreateSubjectPayload {
  name: string;
  code: string;
  description?: string;
}

export interface UpdateSubjectPayload {
  name?: string;
  code?: string;
  description?: string;
  isActive?: boolean;
}

// Teacher Assignment DTOs
export interface AssignTeacherPayload {
  teacherId: string;
  subjectId: string;
  classId: string;
}

export const adminService = {
  // ── Users ─────────────────────────────────────────────────────────────────
  getUsers: async (params?: PaginationParams & { role?: string }) => {
    const res = await apiClient.get<ApiResponse<PagedResponse<User>>>('/admin/users', { params });
    return res.data.data;
  },

  getUserById: async (id: string) => {
    const res = await apiClient.get<ApiResponse<User>>(`/admin/users/${id}`);
    return res.data.data;
  },

  createUser: async (payload: CreateUserPayload) => {
    const res = await apiClient.post<ApiResponse<User>>('/admin/users', payload);
    return res.data.data;
  },

  updateUser: async (id: string, payload: UpdateUserPayload) => {
    const res = await apiClient.put<ApiResponse<User>>(`/admin/users/${id}`, payload);
    return res.data.data;
  },

  changePassword: async (id: string, payload: ChangePasswordPayload) => {
    const res = await apiClient.patch<ApiResponse<null>>(`/admin/users/${id}/password`, payload);
    return res.data.data;
  },

  deleteUser: async (id: string) => {
    const res = await apiClient.delete<ApiResponse<null>>(`/admin/users/${id}`);
    return res.data.data;
  },

  // ── Classes ───────────────────────────────────────────────────────────────
  getClasses: async (params?: PaginationParams) => {
    const res = await apiClient.get<ApiResponse<PagedResponse<SchoolClass>>>('/admin/classes', { params });
    return res.data.data;
  },

  getClassById: async (id: string) => {
    const res = await apiClient.get<ApiResponse<SchoolClass>>(`/admin/classes/${id}`);
    return res.data.data;
  },

  createClass: async (payload: CreateClassPayload) => {
    const res = await apiClient.post<ApiResponse<SchoolClass>>('/admin/classes', payload);
    return res.data.data;
  },

  updateClass: async (id: string, payload: UpdateClassPayload) => {
    const res = await apiClient.put<ApiResponse<SchoolClass>>(`/admin/classes/${id}`, payload);
    return res.data.data;
  },

  deleteClass: async (id: string) => {
    const res = await apiClient.delete<ApiResponse<null>>(`/admin/classes/${id}`);
    return res.data.data;
  },

  // ── Subjects ──────────────────────────────────────────────────────────────
  getSubjects: async (params?: PaginationParams) => {
    const res = await apiClient.get<ApiResponse<PagedResponse<Subject>>>('/admin/subjects', { params });
    return res.data.data;
  },

  getSubjectById: async (id: string) => {
    const res = await apiClient.get<ApiResponse<Subject>>(`/admin/subjects/${id}`);
    return res.data.data;
  },

  createSubject: async (payload: CreateSubjectPayload) => {
    const res = await apiClient.post<ApiResponse<Subject>>('/admin/subjects', payload);
    return res.data.data;
  },

  updateSubject: async (id: string, payload: UpdateSubjectPayload) => {
    const res = await apiClient.put<ApiResponse<Subject>>(`/admin/subjects/${id}`, payload);
    return res.data.data;
  },

  deleteSubject: async (id: string) => {
    const res = await apiClient.delete<ApiResponse<null>>(`/admin/subjects/${id}`);
    return res.data.data;
  },

  // ── Teacher Assignments ───────────────────────────────────────────────────
  getTeacherAssignments: async () => {
    const res = await apiClient.get<ApiResponse<TeacherSubject[]>>('/admin/teacher-subjects');
    return res.data.data;
  },

  assignTeacher: async (payload: AssignTeacherPayload) => {
    const res = await apiClient.post<ApiResponse<TeacherSubject>>('/admin/teacher-subjects', payload);
    return res.data.data;
  },

  removeTeacherAssignment: async (id: string) => {
    const res = await apiClient.delete<ApiResponse<null>>(`/admin/teacher-subjects/${id}`);
    return res.data.data;
  },
};
