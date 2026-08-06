import { apiClient } from './api';
import { ApiResponse, PagedResponse, PaginationParams } from '@/types/api';
import { Assignment, Submission, StudentAssignmentDetail } from '@/types/domain';

export interface SubmitAssignmentPayload {
  assignmentId: string;
  content?: string;
  filePath?: string;
}

export interface UpdateSubmissionPayload {
  content?: string;
  filePath?: string;
}

export const studentService = {
  getPublishedAssignments: async (params?: PaginationParams & { subjectId?: string }) => {
    const res = await apiClient.get<ApiResponse<PagedResponse<Assignment>>>('/student/assignments', { params });
    return res.data.data;
  },

  getAssignmentDetails: async (id: string) => {
    const res = await apiClient.get<ApiResponse<StudentAssignmentDetail>>(`/student/assignments/${id}`);
    return res.data.data;
  },

  submitAssignment: async (payload: SubmitAssignmentPayload) => {
    const res = await apiClient.post<ApiResponse<Submission>>('/student/submissions', payload);
    return res.data.data;
  },

  updateSubmission: async (id: string, payload: UpdateSubmissionPayload) => {
    const res = await apiClient.put<ApiResponse<Submission>>(`/student/submissions/${id}`, payload);
    return res.data.data;
  },

  getMySubmissions: async () => {
    const res = await apiClient.get<ApiResponse<Submission[]>>('/student/submissions');
    return res.data.data;
  },
};
