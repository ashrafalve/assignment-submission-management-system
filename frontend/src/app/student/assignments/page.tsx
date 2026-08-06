'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { studentService, SubmitAssignmentPayload, UpdateSubmissionPayload } from '@/services/student-service';
import { DataTable, Column } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/form';
import { Modal } from '@/components/ui/modal';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/contexts/ToastContext';
import { Assignment, StudentAssignmentDetail } from '@/types/domain';
import { Clock, Send, Eye, Edit3, AlertTriangle, CheckCircle2 } from 'lucide-react';

export default function StudentAssignmentsPage() {
  const queryClient = useQueryClient();
  const { success, error: toastError } = useToast();

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');

  const [viewingAssignmentId, setViewingAssignmentId] = useState<string | null>(null);
  const [submittingAssignment, setSubmittingAssignment] = useState<Assignment | null>(null);
  const [updatingSubmissionId, setUpdatingSubmissionId] = useState<string | null>(null);

  const [submitForm, setSubmitForm] = useState({ content: '', filePath: '' });
  const [updateForm, setUpdateForm] = useState({ content: '', filePath: '' });

  // 1. Fetch published assignments for student's class
  const { data: assignmentsData, isLoading } = useQuery({
    queryKey: ['student-assignments', page, pageSize, searchTerm],
    queryFn: () => studentService.getPublishedAssignments({ pageNumber: page, pageSize, searchTerm }),
  });

  // 2. Fetch specific assignment details when modal is open
  const { data: detailData } = useQuery({
    queryKey: ['student-assignment-detail', viewingAssignmentId],
    queryFn: () => (viewingAssignmentId ? studentService.getAssignmentDetails(viewingAssignmentId) : null),
    enabled: !!viewingAssignmentId,
  });

  // 3. Fetch student's submissions list to cross-check submission state
  const { data: mySubmissions } = useQuery({
    queryKey: ['student-my-submissions'],
    queryFn: studentService.getMySubmissions,
  });

  const getExistingSubmission = (assignmentId: string) => {
    return mySubmissions?.find((s) => s.assignmentId === assignmentId);
  };

  // Submit Mutation
  const submitMutation = useMutation({
    mutationFn: studentService.submitAssignment,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['student-assignments'] });
      queryClient.invalidateQueries({ queryKey: ['student-my-submissions'] });
      success('Assignment submitted successfully!');
      setSubmittingAssignment(null);
      setSubmitForm({ content: '', filePath: '' });
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to submit assignment'),
  });

  // Update Mutation
  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateSubmissionPayload }) =>
      studentService.updateSubmission(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['student-assignments'] });
      queryClient.invalidateQueries({ queryKey: ['student-my-submissions'] });
      success('Submission updated successfully!');
      setUpdatingSubmissionId(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to update submission'),
  });

  const columns: Column<Assignment>[] = [
    {
      key: 'title',
      header: 'Assignment Title',
      sortable: true,
      cell: (a) => (
        <div>
          <h4 className="font-semibold text-foreground">{a.title}</h4>
          <p className="text-xs text-muted-foreground line-clamp-1">{a.description}</p>
        </div>
      ),
    },
    {
      key: 'subjectName',
      header: 'Subject & Teacher',
      cell: (a) => (
        <div className="text-xs space-y-0.5">
          <div className="font-semibold text-foreground">{a.subjectName} ({a.subjectCode})</div>
          <div className="text-muted-foreground">Instructor: {a.teacherName}</div>
        </div>
      ),
    },
    {
      key: 'dueDate',
      header: 'Deadline',
      sortable: true,
      cell: (a) => {
        const isPast = new Date(a.dueDate) < new Date();
        return (
          <div className="flex items-center gap-1.5 text-xs">
            <Clock className={`h-3.5 w-3.5 ${isPast ? 'text-rose-500' : 'text-emerald-500'}`} />
            <span className={isPast ? 'text-rose-500 font-medium' : 'text-foreground'}>
              {new Date(a.dueDate).toLocaleString([], { dateStyle: 'medium', timeStyle: 'short' })}
            </span>
          </div>
        );
      },
    },
    {
      key: 'totalMarks',
      header: 'Max Points',
      cell: (a) => <span className="font-bold text-foreground">{a.totalMarks} pts</span>,
    },
    {
      key: 'submissionState',
      header: 'Your Submission',
      cell: (a) => {
        const existing = getExistingSubmission(a.id);
        if (!existing) {
          const isOverdue = new Date(a.dueDate) < new Date();
          return isOverdue ? (
            <Badge variant="destructive">Overdue</Badge>
          ) : (
            <Badge variant="warning">Pending</Badge>
          );
        }
        return (
          <Badge variant={existing.status === 'Graded' ? 'success' : 'info'}>
            {existing.status}
          </Badge>
        );
      },
    },
    {
      key: 'actions',
      header: 'Actions',
      className: 'text-right',
      cell: (a) => {
        const existing = getExistingSubmission(a.id);
        const isPastDeadline = new Date(a.dueDate) < new Date();

        return (
          <div className="flex items-center justify-end gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setViewingAssignmentId(a.id)}
              className="gap-1 text-xs"
            >
              <Eye className="h-3.5 w-3.5 text-blue-500" />
              Details
            </Button>

            {!existing ? (
              <Button
                variant="default"
                size="sm"
                disabled={isPastDeadline}
                onClick={() => {
                  setSubmittingAssignment(a);
                  setSubmitForm({ content: '', filePath: '' });
                }}
                className="gap-1 text-xs font-semibold shadow-sm"
              >
                <Send className="h-3.5 w-3.5" />
                Submit Work
              </Button>
            ) : (
              <Button
                variant="secondary"
                size="sm"
                disabled={isPastDeadline}
                onClick={() => {
                  setUpdatingSubmissionId(existing.id);
                  setUpdateForm({ content: existing.content || '', filePath: existing.filePath || '' });
                }}
                className="gap-1 text-xs font-semibold"
              >
                <Edit3 className="h-3.5 w-3.5 text-amber-500" />
                Edit Work
              </Button>
            )}
          </div>
        );
      },
    },
  ];

  return (
    <ProtectedRoute allowedRoles={['Student']}>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground">Class Assignments</h1>
          <p className="text-sm text-muted-foreground mt-1">View active assignments for your class section and submit your completed work.</p>
        </div>

        <DataTable
          columns={columns}
          data={assignmentsData?.items || []}
          isLoading={isLoading}
          keyExtractor={(a) => a.id}
          onSearch={(term) => {
            setSearchTerm(term);
            setPage(1);
          }}
          pagination={{
            pageNumber: assignmentsData?.pageNumber || 1,
            pageSize: assignmentsData?.pageSize || 10,
            totalCount: assignmentsData?.totalCount || 0,
            totalPages: assignmentsData?.totalPages || 1,
            onPageChange: (p) => setPage(p),
            onPageSizeChange: (s) => setPageSize(s),
          }}
          emptyTitle="No Class Assignments"
          emptyDescription="There are currently no published assignments for your enrolled class."
        />

        {/* View Assignment Detail Modal */}
        <Modal isOpen={!!viewingAssignmentId} onClose={() => setViewingAssignmentId(null)} title="Assignment Details" size="lg">
          {detailData && (
            <div className="space-y-4">
              <div className="border-b border-border pb-3">
                <h3 className="text-xl font-bold text-foreground">{detailData.assignment.title}</h3>
                <div className="flex flex-wrap items-center gap-3 text-xs text-muted-foreground mt-1">
                  <span>Subject: <strong className="text-foreground">{detailData.assignment.subjectName} ({detailData.assignment.subjectCode})</strong></span>
                  <span>Teacher: <strong className="text-foreground">{detailData.assignment.teacherName}</strong></span>
                  <span>Max Points: <strong className="text-foreground">{detailData.assignment.totalMarks} pts</strong></span>
                </div>
              </div>

              <div>
                <h5 className="text-xs font-bold uppercase tracking-wider text-muted-foreground mb-1">Instructions</h5>
                <p className="text-sm text-foreground bg-muted/30 p-4 rounded-xl leading-relaxed whitespace-pre-wrap">
                  {detailData.assignment.description}
                </p>
              </div>

              <div className="flex items-center gap-2 text-xs font-medium bg-secondary/50 p-3 rounded-lg">
                <Clock className="h-4 w-4 text-primary" />
                <span>Deadline: {new Date(detailData.assignment.dueDate).toLocaleString()}</span>
              </div>

              {detailData.submission && (
                <div className="rounded-xl border border-emerald-500/30 bg-emerald-500/10 p-4 space-y-2">
                  <div className="flex items-center justify-between">
                    <span className="flex items-center gap-1.5 font-bold text-emerald-600 dark:text-emerald-400 text-sm">
                      <CheckCircle2 className="h-4 w-4" /> Your Submission Status
                    </span>
                    <Badge variant="success">{detailData.submission.status}</Badge>
                  </div>

                  {detailData.submission.marksObtained !== undefined && detailData.submission.marksObtained !== null && (
                    <div className="text-sm font-bold text-foreground">
                      Grade: <span className="text-emerald-500">{detailData.submission.marksObtained} / {detailData.assignment.totalMarks} pts</span>
                    </div>
                  )}

                  {detailData.submission.feedback && (
                    <div className="text-xs text-foreground mt-2 border-t border-emerald-500/20 pt-2">
                      <strong>Teacher Feedback:</strong> {detailData.submission.feedback}
                    </div>
                  )}
                </div>
              )}

              <div className="flex justify-end pt-2">
                <Button variant="outline" onClick={() => setViewingAssignmentId(null)}>
                  Close
                </Button>
              </div>
            </div>
          )}
        </Modal>

        {/* Submit Assignment Modal */}
        <Modal isOpen={!!submittingAssignment} onClose={() => setSubmittingAssignment(null)} title={`Submit Work: ${submittingAssignment?.title}`}>
          <form
            onSubmit={(e) => {
              e.preventDefault();
              if (submittingAssignment) {
                submitMutation.mutate({
                  assignmentId: submittingAssignment.id,
                  content: submitForm.content,
                  filePath: submitForm.filePath,
                });
              }
            }}
            className="space-y-4"
          >
            <div className="flex items-center justify-between text-xs bg-muted/40 p-3 rounded-lg">
              <span>Deadline: <strong>{submittingAssignment ? new Date(submittingAssignment.dueDate).toLocaleString() : ''}</strong></span>
              <span>Max Points: <strong>{submittingAssignment?.totalMarks} pts</strong></span>
            </div>

            <Textarea
              label="Submission Content / Answer Text"
              placeholder="Write your response or detailed notes here..."
              value={submitForm.content}
              onChange={(e) => setSubmitForm({ ...submitForm, content: e.target.value })}
            />

            <Input
              label="Attachment Link / Document URL (Optional)"
              placeholder="https://docs.google.com/... or https://github.com/..."
              value={submitForm.filePath}
              onChange={(e) => setSubmitForm({ ...submitForm, filePath: e.target.value })}
            />

            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setSubmittingAssignment(null)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={submitMutation.isPending} disabled={!submitForm.content && !submitForm.filePath}>
                Submit Assignment
              </Button>
            </div>
          </form>
        </Modal>

        {/* Edit Submission Modal */}
        <Modal isOpen={!!updatingSubmissionId} onClose={() => setUpdatingSubmissionId(null)} title="Update Your Submission">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              if (updatingSubmissionId) {
                updateMutation.mutate({
                  id: updatingSubmissionId,
                  payload: updateForm,
                });
              }
            }}
            className="space-y-4"
          >
            <Textarea
              label="Update Submission Content"
              value={updateForm.content}
              onChange={(e) => setUpdateForm({ ...updateForm, content: e.target.value })}
            />

            <Input
              label="Attachment Link / Document URL"
              value={updateForm.filePath}
              onChange={(e) => setUpdateForm({ ...updateForm, filePath: e.target.value })}
            />

            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setUpdatingSubmissionId(null)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={updateMutation.isPending}>
                Save Updated Work
              </Button>
            </div>
          </form>
        </Modal>
      </div>
    </ProtectedRoute>
  );
}
