'use client';

import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { studentService } from '@/services/student-service';
import { DataTable, Column } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { Modal } from '@/components/ui/modal';
import { Badge } from '@/components/ui/badge';
import { Submission } from '@/types/domain';
import { FileCheck, Eye, Award, MessageSquare, Clock } from 'lucide-react';

export default function StudentSubmissionsPage() {
  const [viewingSubmission, setViewingSubmission] = useState<Submission | null>(null);

  const { data: submissions, isLoading } = useQuery({
    queryKey: ['student-my-submissions-page'],
    queryFn: studentService.getMySubmissions,
  });

  const columns: Column<Submission>[] = [
    {
      key: 'assignmentTitle',
      header: 'Assignment Title',
      sortable: true,
      cell: (s) => <span className="font-semibold text-foreground">{s.assignmentTitle}</span>,
    },
    {
      key: 'submittedAt',
      header: 'Submission Date',
      sortable: true,
      cell: (s) => (
        <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
          <Clock className="h-3.5 w-3.5 text-muted-foreground" />
          {s.submittedAt ? new Date(s.submittedAt).toLocaleString() : 'N/A'}
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
      header: 'Grade / Score',
      cell: (s) => (
        <div className="font-bold text-sm">
          {s.marksObtained !== undefined && s.marksObtained !== null ? (
            <span className="text-emerald-500">{s.marksObtained} / {s.totalMarks} pts</span>
          ) : (
            <span className="text-muted-foreground text-xs font-normal">Pending Grade</span>
          )}
        </div>
      ),
    },
    {
      key: 'feedback',
      header: 'Teacher Feedback',
      cell: (s) =>
        s.feedback ? (
          <span className="text-xs text-foreground line-clamp-1 italic">"{s.feedback}"</span>
        ) : (
          <span className="text-muted-foreground text-xs">—</span>
        ),
    },
    {
      key: 'actions',
      header: 'Actions',
      className: 'text-right',
      cell: (s) => (
        <Button
          variant="outline"
          size="sm"
          onClick={() => setViewingSubmission(s)}
          className="gap-1 text-xs"
        >
          <Eye className="h-3.5 w-3.5 text-blue-500" />
          View Details
        </Button>
      ),
    },
  ];

  return (
    <ProtectedRoute allowedRoles={['Student']}>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground">My Submissions & Grades</h1>
          <p className="text-sm text-muted-foreground mt-1">Track submitted assignments, view grades, and read instructor feedback.</p>
        </div>

        <DataTable
          columns={columns}
          data={submissions || []}
          isLoading={isLoading}
          keyExtractor={(s) => s.id}
          emptyTitle="No Submissions Found"
          emptyDescription="You have not submitted any assignments yet."
        />

        {/* View Submission Details Modal */}
        <Modal isOpen={!!viewingSubmission} onClose={() => setViewingSubmission(null)} title={`Submission: ${viewingSubmission?.assignmentTitle}`}>
          {viewingSubmission && (
            <div className="space-y-4">
              <div className="flex items-center justify-between border-b border-border pb-3">
                <span className="text-xs text-muted-foreground">
                  Submitted on: {viewingSubmission.submittedAt ? new Date(viewingSubmission.submittedAt).toLocaleString() : 'N/A'}
                </span>
                <Badge variant={viewingSubmission.status === 'Graded' ? 'success' : 'info'}>
                  {viewingSubmission.status}
                </Badge>
              </div>

              {viewingSubmission.marksObtained !== undefined && viewingSubmission.marksObtained !== null && (
                <div className="rounded-xl border border-emerald-500/30 bg-emerald-500/10 p-4 flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <Award className="h-5 w-5 text-emerald-500" />
                    <span className="font-bold text-foreground">Assigned Grade</span>
                  </div>
                  <div className="text-xl font-extrabold text-emerald-600 dark:text-emerald-400">
                    {viewingSubmission.marksObtained} / {viewingSubmission.totalMarks} pts
                  </div>
                </div>
              )}

              <div>
                <h5 className="text-xs font-bold uppercase tracking-wider text-muted-foreground mb-1">Your Submission Content</h5>
                <div className="rounded-lg border border-border bg-muted/30 p-3 text-sm font-mono whitespace-pre-wrap">
                  {viewingSubmission.content || <span className="text-muted-foreground italic">No text provided</span>}
                </div>
              </div>

              {viewingSubmission.filePath && (
                <div>
                  <h5 className="text-xs font-bold uppercase tracking-wider text-muted-foreground mb-1">Attachment Link</h5>
                  <a
                    href={viewingSubmission.filePath}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-sm font-medium text-primary underline"
                  >
                    {viewingSubmission.filePath}
                  </a>
                </div>
              )}

              {viewingSubmission.feedback && (
                <div className="rounded-lg bg-secondary p-3 text-sm space-y-1">
                  <div className="flex items-center gap-1.5 font-bold text-foreground text-xs">
                    <MessageSquare className="h-3.5 w-3.5 text-primary" /> Teacher Feedback
                  </div>
                  <p className="text-foreground">{viewingSubmission.feedback}</p>
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
      </div>
    </ProtectedRoute>
  );
}
