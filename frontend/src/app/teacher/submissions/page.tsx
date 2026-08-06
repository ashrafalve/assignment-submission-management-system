'use client';

import React, { useState, useEffect, Suspense } from 'react';
import { useSearchParams } from 'next/navigation';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { teacherService, GradeSubmissionPayload } from '@/services/teacher-service';
import { DataTable, Column } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea, Select } from '@/components/ui/form';
import { Modal } from '@/components/ui/modal';
import { Badge } from '@/components/ui/badge';
import { LoadingScreen } from '@/components/ui/loading';
import { useToast } from '@/contexts/ToastContext';
import { Submission, SubmissionStatus } from '@/types/domain';
import { CheckSquare, Eye, Award, Clock } from 'lucide-react';

function TeacherSubmissionsContent() {
  const searchParams = useSearchParams();
  const queryClient = useQueryClient();
  const { success, error: toastError } = useToast();

  const [selectedAssignmentId, setSelectedAssignmentId] = useState<string>(
    searchParams.get('assignmentId') || ''
  );
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const [gradingSubmission, setGradingSubmission] = useState<Submission | null>(null);
  const [viewingSubmission, setViewingSubmission] = useState<Submission | null>(null);

  const [gradeForm, setGradeForm] = useState<GradeSubmissionPayload>({
    marksObtained: 0,
    feedback: '',
    status: 'Graded',
  });

  const { data: assignmentsData } = useQuery({
    queryKey: ['teacher-assignments-list'],
    queryFn: () => teacherService.getAssignments({ pageSize: 100 }),
  });

  const assignmentOptions = [
    { label: 'Select Assignment to Review', value: '' },
    ...(assignmentsData?.items.map((a) => ({
      label: `${a.title} (${a.className} • ${a.subjectCode})`,
      value: a.id,
    })) || []),
  ];

  useEffect(() => {
    if (!selectedAssignmentId && assignmentsData?.items.length) {
      setSelectedAssignmentId(assignmentsData.items[0].id);
    }
  }, [assignmentsData, selectedAssignmentId]);

  const currentAssignment = assignmentsData?.items.find((a) => a.id === selectedAssignmentId);

  const { data: submissionsData, isLoading } = useQuery({
    queryKey: ['teacher-submissions', selectedAssignmentId, page, pageSize],
    queryFn: () =>
      selectedAssignmentId
        ? teacherService.getSubmissionsForAssignment(selectedAssignmentId, { pageNumber: page, pageSize })
        : Promise.resolve({ items: [], totalCount: 0, pageNumber: 1, pageSize: 10, totalPages: 1, hasPreviousPage: false, hasNextPage: false }),
    enabled: !!selectedAssignmentId,
  });

  const gradeMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: GradeSubmissionPayload }) =>
      teacherService.gradeSubmission(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['teacher-submissions'] });
      success('Submission graded and feedback saved');
      setGradingSubmission(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to grade submission'),
  });

  const columns: Column<Submission>[] = [
    {
      key: 'studentName',
      header: 'Student Name',
      sortable: true,
      cell: (s) => <span className="font-semibold text-foreground">{s.studentName}</span>,
    },
    {
      key: 'submittedAt',
      header: 'Submission Time',
      sortable: true,
      cell: (s) => (
        <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
          <Clock className="h-3.5 w-3.5 text-muted-foreground" />
          {s.submittedAt ? new Date(s.submittedAt).toLocaleString() : 'Not submitted'}
        </div>
      ),
    },
    {
      key: 'status',
      header: 'Status',
      sortable: true,
      cell: (s) => (
        <Badge
          variant={
            s.status === 'Graded'
              ? 'success'
              : s.status === 'Submitted'
              ? 'info'
              : s.status === 'Late'
              ? 'warning'
              : 'destructive'
          }
        >
          {s.status}
        </Badge>
      ),
    },
    {
      key: 'marksObtained',
      header: 'Score / Marks',
      cell: (s) => (
        <div className="font-bold text-sm">
          {s.marksObtained !== undefined && s.marksObtained !== null ? (
            <span className="text-emerald-500">{s.marksObtained} / {s.totalMarks} pts</span>
          ) : (
            <span className="text-muted-foreground text-xs font-normal">Ungraded</span>
          )}
        </div>
      ),
    },
    {
      key: 'actions',
      header: 'Actions',
      className: 'text-right',
      cell: (s) => (
        <div className="flex items-center justify-end gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => setViewingSubmission(s)}
            className="gap-1 text-xs"
          >
            <Eye className="h-3.5 w-3.5 text-blue-500" />
            View
          </Button>

          <Button
            variant="default"
            size="sm"
            onClick={() => {
              setGradingSubmission(s);
              setGradeForm({
                marksObtained: s.marksObtained ?? 0,
                feedback: s.feedback || '',
                status: 'Graded',
              });
            }}
            className="gap-1 text-xs font-semibold shadow-sm"
          >
            <Award className="h-3.5 w-3.5" />
            Grade
          </Button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground">Submission Review & Grading</h1>
          <p className="text-sm text-muted-foreground mt-1">Review student submissions, assign marks, and provide constructive feedback.</p>
        </div>
      </div>

      <div className="w-full max-w-lg">
        <Select
          label="Select Assignment"
          value={selectedAssignmentId}
          onChange={(e) => {
            setSelectedAssignmentId(e.target.value);
            setPage(1);
          }}
          options={assignmentOptions}
        />
      </div>

      {currentAssignment && (
        <div className="rounded-xl border border-primary/20 bg-primary/5 p-4 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2">
          <div>
            <h3 className="font-bold text-foreground">{currentAssignment.title}</h3>
            <p className="text-xs text-muted-foreground">
              Subject: {currentAssignment.subjectName} ({currentAssignment.subjectCode}) • Class: {currentAssignment.className}
            </p>
          </div>
          <div className="flex items-center gap-3 text-xs font-semibold">
            <span className="text-muted-foreground">Max Marks: <strong className="text-foreground">{currentAssignment.totalMarks} pts</strong></span>
            <span className="text-muted-foreground">Due: <strong className="text-foreground">{new Date(currentAssignment.dueDate).toLocaleDateString()}</strong></span>
          </div>
        </div>
      )}

      <DataTable
        columns={columns}
        data={submissionsData?.items || []}
        isLoading={isLoading}
        keyExtractor={(s) => s.id}
        pagination={{
          pageNumber: submissionsData?.pageNumber || 1,
          pageSize: submissionsData?.pageSize || 10,
          totalCount: submissionsData?.totalCount || 0,
          totalPages: submissionsData?.totalPages || 1,
          onPageChange: (p) => setPage(p),
          onPageSizeChange: (s) => setPageSize(s),
        }}
        emptyTitle="No Submissions Yet"
        emptyDescription="No students have submitted work for this assignment yet."
      />

      {/* View Submission Modal */}
      <Modal isOpen={!!viewingSubmission} onClose={() => setViewingSubmission(null)} title="Student Submission Details" size="lg">
        {viewingSubmission && (
          <div className="space-y-4">
            <div className="flex items-center justify-between border-b border-border pb-3">
              <div>
                <h4 className="font-bold text-foreground text-base">{viewingSubmission.studentName}</h4>
                <p className="text-xs text-muted-foreground">Submitted at: {viewingSubmission.submittedAt ? new Date(viewingSubmission.submittedAt).toLocaleString() : 'N/A'}</p>
              </div>
              <Badge variant={viewingSubmission.status === 'Graded' ? 'success' : 'info'}>
                {viewingSubmission.status}
              </Badge>
            </div>

            <div>
              <h5 className="text-xs font-bold uppercase tracking-wider text-muted-foreground mb-1">Text Content</h5>
              <div className="rounded-lg border border-border bg-muted/30 p-3 text-sm font-mono whitespace-pre-wrap">
                {viewingSubmission.content || <span className="text-muted-foreground italic">No text content provided</span>}
              </div>
            </div>

            {viewingSubmission.filePath && (
              <div>
                <h5 className="text-xs font-bold uppercase tracking-wider text-muted-foreground mb-1">File Attachment</h5>
                <p className="text-sm font-medium text-primary underline">{viewingSubmission.filePath}</p>
              </div>
            )}

            {viewingSubmission.feedback && (
              <div className="rounded-lg bg-emerald-500/10 border border-emerald-500/20 p-3 text-sm">
                <h5 className="font-bold text-emerald-600 dark:text-emerald-400 text-xs uppercase">Teacher Feedback</h5>
                <p className="mt-1 text-foreground">{viewingSubmission.feedback}</p>
              </div>
            )}

            <div className="flex justify-end pt-2">
              <Button variant="outline" onClick={() => setViewingSubmission(null)}>
                Close
              </Button>
            </div>
          </div>
        )}
      </Modal>

      {/* Grade Submission Modal */}
      <Modal isOpen={!!gradingSubmission} onClose={() => setGradingSubmission(null)} title={`Grade Submission - ${gradingSubmission?.studentName}`}>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            if (gradingSubmission) {
              if (gradeForm.marksObtained > gradingSubmission.totalMarks) {
                toastError(`Marks obtained cannot exceed maximum marks (${gradingSubmission.totalMarks})`);
                return;
              }
              gradeMutation.mutate({ id: gradingSubmission.id, payload: gradeForm });
            }
          }}
          className="space-y-4"
        >
          <div className="flex items-center justify-between bg-muted/40 p-3 rounded-lg text-sm">
            <span className="text-muted-foreground">Assignment Max Marks:</span>
            <strong className="font-bold text-foreground">{gradingSubmission?.totalMarks} pts</strong>
          </div>

          <Input
            label="Marks Obtained"
            type="number"
            min={0}
            max={gradingSubmission?.totalMarks || 100}
            step="0.5"
            value={gradeForm.marksObtained}
            onChange={(e) => setGradeForm({ ...gradeForm, marksObtained: Number(e.target.value) })}
            required
          />

          <Select
            label="Submission Status"
            value={gradeForm.status}
            onChange={(e) => setGradeForm({ ...gradeForm, status: e.target.value as SubmissionStatus })}
            options={[
              { label: 'Graded', value: 'Graded' },
              { label: 'Submitted (Pending Review)', value: 'Submitted' },
              { label: 'Rejected (Resubmission Needed)', value: 'Rejected' },
              { label: 'Late', value: 'Late' },
            ]}
          />

          <Textarea
            label="Teacher Feedback"
            placeholder="Provide constructive feedback for the student..."
            value={gradeForm.feedback}
            onChange={(e) => setGradeForm({ ...gradeForm, feedback: e.target.value })}
          />

          <div className="flex justify-end gap-2 pt-4">
            <Button type="button" variant="outline" onClick={() => setGradingSubmission(null)}>
              Cancel
            </Button>
            <Button type="submit" isLoading={gradeMutation.isPending}>
              Save Grade & Feedback
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}

export default function TeacherSubmissionsPage() {
  return (
    <ProtectedRoute allowedRoles={['Teacher']}>
      <Suspense fallback={<LoadingScreen message="Loading submissions..." />}>
        <TeacherSubmissionsContent />
      </Suspense>
    </ProtectedRoute>
  );
}
