import { apiClient } from './api';
import { ApiResponse, PagedResponse, PaginationParams } from '@/types/api';
import { Assignment, AssignmentStatus, Submission, SubmissionStatus } from '@/types/domain';

export interface CreateAssignmentPayload {
  title: string;
  description: string;
  dueDate: string;
  totalMarks: number;
  classId: string;
  subjectId: string;
  publishNow?: boolean;
}

export interface UpdateAssignmentPayload {
  title?: string;
  description?: string;
  dueDate?: string;
  totalMarks?: number;
  classId?: string;
  subjectId?: string;
}

export interface GradeSubmissionPayload {
  marksObtained: number;
  feedback?: string;
  status?: SubmissionStatus;
}

export interface ChangeSubmissionStatusPayload {
  status: SubmissionStatus;
  feedback?: string;
}

export const teacherService = {
  // ── Assignments ───────────────────────────────────────────────────────────
  getAssignments: async (params?: PaginationParams & { classId?: string; subjectId?: string; status?: AssignmentStatus }) => {
    const res = await apiClient.get<ApiResponse<PagedResponse<Assignment>>>('/teacher/assignments', { params });
    return res.data.data;
  },

  getAssignmentById: async (id: string) => {
    const res = await apiClient.get<ApiResponse<Assignment>>(`/teacher/assignments/${id}`);
    return res.data.data;
  },

  createAssignment: async (payload: CreateAssignmentPayload) => {
    const res = await apiClient.post<ApiResponse<Assignment>>('/teacher/assignments', payload);
    return res.data.data;
  },

  updateAssignment: async (id: string, payload: UpdateAssignmentPayload) => {
    const res = await apiClient.put<ApiResponse<Assignment>>(`/teacher/assignments/${id}`, payload);
    return res.data.data;
  },

  publishAssignment: async (id: string) => {
    const res = await apiClient.patch<ApiResponse<Assignment>>(`/teacher/assignments/${id}/publish`);
    return res.data.data;
  },

  saveDraft: async (id: string) => {
    const res = await apiClient.patch<ApiResponse<Assignment>>(`/teacher/assignments/${id}/draft`);
    return res.data.data;
  },

  deleteAssignment: async (id: string) => {
    const res = await apiClient.delete<ApiResponse<null>>(`/teacher/assignments/${id}`);
    return res.data.data;
  },

  // ── Submissions Review & Grading ───────────────────────────────────────────
  getSubmissionsForAssignment: async (assignmentId: string, params?: PaginationParams) => {
    const res = await apiClient.get<ApiResponse<PagedResponse<Submission>>>(`/teacher/assignments/${assignmentId}/submissions`, { params });
    return res.data.data;
  },

  getSubmissionById: async (submissionId: string) => {
    const res = await apiClient.get<ApiResponse<Submission>>(`/teacher/submissions/${submissionId}`);
    return res.data.data;
  },

  gradeSubmission: async (submissionId: string, payload: GradeSubmissionPayload) => {
    const res = await apiClient.post<ApiResponse<Submission>>(`/teacher/submissions/${submissionId}/grade`, payload);
    return res.data.data;
  },

  changeStatus: async (submissionId: string, payload: ChangeSubmissionStatusPayload) => {
    const res = await apiClient.patch<ApiResponse<Submission>>(`/teacher/submissions/${submissionId}/status`, payload);
    return res.data.data;
  },
};
