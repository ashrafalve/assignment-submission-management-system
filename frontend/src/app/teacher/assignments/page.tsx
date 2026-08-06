'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { teacherService, CreateAssignmentPayload, UpdateAssignmentPayload } from '@/services/teacher-service';
import { adminService } from '@/services/admin-service';
import { DataTable, Column } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea, Select, Checkbox } from '@/components/ui/form';
import { Modal } from '@/components/ui/modal';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/contexts/ToastContext';
import { Assignment, AssignmentStatus } from '@/types/domain';
import { Plus, Edit, Trash2, Send, FileEdit, CheckSquare, Clock } from 'lucide-react';
import Link from 'next/link';

export default function TeacherAssignmentsPage() {
  const queryClient = useQueryClient();
  const { success, error: toastError } = useToast();

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('');

  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingAssignment, setEditingAssignment] = useState<Assignment | null>(null);
  const [deletingAssignment, setDeletingAssignment] = useState<Assignment | null>(null);

  const [createForm, setCreateForm] = useState<CreateAssignmentPayload>({
    title: '',
    description: '',
    dueDate: '',
    totalMarks: 100,
    classId: '',
    subjectId: '',
    publishNow: false,
  });

  const [updateForm, setUpdateForm] = useState<UpdateAssignmentPayload>({
    title: '',
    description: '',
    dueDate: '',
    totalMarks: 100,
    classId: '',
    subjectId: '',
  });

  // Queries
  const { data, isLoading } = useQuery({
    queryKey: ['teacher-assignments', page, pageSize, searchTerm, statusFilter],
    queryFn: () =>
      teacherService.getAssignments({
        pageNumber: page,
        pageSize,
        searchTerm,
        status: (statusFilter as AssignmentStatus) || undefined,
      }),
  });

  // Fetch teacher's assigned subjects & classes from TeacherSubject mappings
  const { data: myTeacherSubjects } = useQuery({
    queryKey: ['my-teacher-subjects'],
    queryFn: adminService.getTeacherAssignments, // In real app, filter for logged-in teacher
  });

  // Dynamic Class and Subject Options from assigned courses
  const uniqueClasses = Array.from(
    new Map((myTeacherSubjects || []).map((ts) => [ts.classId, { label: ts.className, value: ts.classId }])).values()
  );

  const uniqueSubjects = Array.from(
    new Map((myTeacherSubjects || []).map((ts) => [ts.subjectId, { label: `${ts.subjectName} (${ts.subjectCode})`, value: ts.subjectId }])).values()
  );

  const classOptions = [{ label: 'Select Assigned Class', value: '' }, ...uniqueClasses];
  const subjectOptions = [{ label: 'Select Assigned Subject', value: '' }, ...uniqueSubjects];

  // Mutations
  const createMutation = useMutation({
    mutationFn: teacherService.createAssignment,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['teacher-assignments'] });
      success('Assignment created successfully');
      setIsCreateOpen(false);
      setCreateForm({
        title: '',
        description: '',
        dueDate: '',
        totalMarks: 100,
        classId: '',
        subjectId: '',
        publishNow: false,
      });
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to create assignment'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateAssignmentPayload }) =>
      teacherService.updateAssignment(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['teacher-assignments'] });
      success('Assignment updated successfully');
      setEditingAssignment(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to update assignment'),
  });

  const publishMutation = useMutation({
    mutationFn: teacherService.publishAssignment,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['teacher-assignments'] });
      success('Assignment published to students');
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to publish assignment'),
  });

  const draftMutation = useMutation({
    mutationFn: teacherService.saveDraft,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['teacher-assignments'] });
      success('Assignment saved as draft');
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to revert to draft'),
  });

  const deleteMutation = useMutation({
    mutationFn: teacherService.deleteAssignment,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['teacher-assignments'] });
      success('Assignment deleted');
      setDeletingAssignment(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to delete assignment'),
  });

  const columns: Column<Assignment>[] = [
    {
      key: 'title',
      header: 'Title & Details',
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
      header: 'Subject & Class',
      cell: (a) => (
        <div className="text-xs space-y-0.5">
          <div className="font-semibold text-foreground">{a.subjectName} ({a.subjectCode})</div>
          <div className="text-muted-foreground">{a.className}</div>
        </div>
      ),
    },
    {
      key: 'dueDate',
      header: 'Due Date',
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
      header: 'Max Marks',
      cell: (a) => <span className="font-bold text-foreground">{a.totalMarks} pts</span>,
    },
    {
      key: 'status',
      header: 'Status',
      sortable: true,
      cell: (a) => (
        <Badge
          variant={a.status === 'Published' ? 'success' : a.status === 'Draft' ? 'warning' : 'outline'}
        >
          {a.status}
        </Badge>
      ),
    },
    {
      key: 'actions',
      header: 'Actions',
      className: 'text-right',
      cell: (a) => (
        <div className="flex items-center justify-end gap-1">
          <Link href={`/teacher/submissions?assignmentId=${a.id}`}>
            <Button variant="ghost" size="icon" title="Review Submissions">
              <CheckSquare className="h-4 w-4 text-emerald-500" />
            </Button>
          </Link>

          {a.status === 'Draft' ? (
            <Button
              variant="ghost"
              size="icon"
              onClick={() => publishMutation.mutate(a.id)}
              title="Publish Assignment"
              isLoading={publishMutation.isPending}
            >
              <Send className="h-4 w-4 text-primary" />
            </Button>
          ) : (
            <Button
              variant="ghost"
              size="icon"
              onClick={() => draftMutation.mutate(a.id)}
              title="Revert to Draft"
              isLoading={draftMutation.isPending}
            >
              <FileEdit className="h-4 w-4 text-amber-500" />
            </Button>
          )}

          <Button
            variant="ghost"
            size="icon"
            onClick={() => {
              setEditingAssignment(a);
              setUpdateForm({
                title: a.title,
                description: a.description,
                dueDate: a.dueDate.substring(0, 16),
                totalMarks: a.totalMarks,
                classId: a.classId,
                subjectId: a.subjectId,
              });
            }}
            title="Edit Assignment"
          >
            <Edit className="h-4 w-4 text-blue-500" />
          </Button>

          <Button variant="ghost" size="icon" onClick={() => setDeletingAssignment(a)} title="Delete Assignment">
            <Trash2 className="h-4 w-4 text-rose-500" />
          </Button>
        </div>
      ),
    },
  ];

  return (
    <ProtectedRoute allowedRoles={['Teacher']}>
      <div className="space-y-6">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-foreground">Assignment Management</h1>
            <p className="text-sm text-muted-foreground mt-1">Create, edit, publish, and manage coursework for your assigned classes.</p>
          </div>
          <Button onClick={() => setIsCreateOpen(true)} className="gap-2 font-semibold shadow-md">
            <Plus className="h-4 w-4" />
            Create Assignment
          </Button>
        </div>

        {/* Filter Bar */}
        <div className="flex flex-wrap items-center gap-3">
          <Select
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setPage(1);
            }}
            options={[
              { label: 'All Statuses', value: '' },
              { label: 'Published Only', value: 'Published' },
              { label: 'Drafts Only', value: 'Draft' },
              { label: 'Closed', value: 'Closed' },
            ]}
            className="w-44"
          />
        </div>

        {/* Assignments Table */}
        <DataTable
          columns={columns}
          data={data?.items || []}
          isLoading={isLoading}
          keyExtractor={(a) => a.id}
          onSearch={(term) => {
            setSearchTerm(term);
            setPage(1);
          }}
          pagination={{
            pageNumber: data?.pageNumber || 1,
            pageSize: data?.pageSize || 10,
            totalCount: data?.totalCount || 0,
            totalPages: data?.totalPages || 1,
            onPageChange: (p) => setPage(p),
            onPageSizeChange: (s) => setPageSize(s),
          }}
        />

        {/* Create Assignment Modal */}
        <Modal isOpen={isCreateOpen} onClose={() => setIsCreateOpen(false)} title="Create New Assignment" size="lg">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              createMutation.mutate({
                ...createForm,
                dueDate: new Date(createForm.dueDate).toISOString(),
              });
            }}
            className="space-y-4"
          >
            <Input
              label="Assignment Title"
              placeholder="e.g. Mathematics Midterm Problem Set 1"
              value={createForm.title}
              onChange={(e) => setCreateForm({ ...createForm, title: e.target.value })}
              required
            />
            <Textarea
              label="Description & Instructions"
              placeholder="Provide detailed instructions for students..."
              value={createForm.description}
              onChange={(e) => setCreateForm({ ...createForm, description: e.target.value })}
              required
            />

            <div className="grid grid-cols-2 gap-3">
              <Select
                label="Class Section"
                value={createForm.classId}
                onChange={(e) => setCreateForm({ ...createForm, classId: e.target.value })}
                options={classOptions}
                required
              />
              <Select
                label="Subject"
                value={createForm.subjectId}
                onChange={(e) => setCreateForm({ ...createForm, subjectId: e.target.value })}
                options={subjectOptions}
                required
              />
            </div>

            <div className="grid grid-cols-2 gap-3">
              <Input
                label="Due Date & Time"
                type="datetime-local"
                value={createForm.dueDate}
                onChange={(e) => setCreateForm({ ...createForm, dueDate: e.target.value })}
                required
              />
              <Input
                label="Max Marks (Total Points)"
                type="number"
                min={1}
                value={createForm.totalMarks}
                onChange={(e) => setCreateForm({ ...createForm, totalMarks: Number(e.target.value) })}
                required
              />
            </div>

            <div className="pt-2">
              <Checkbox
                label="Publish immediately (visible to students)"
                checked={createForm.publishNow}
                onChange={(e) => setCreateForm({ ...createForm, publishNow: e.target.checked })}
              />
            </div>

            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setIsCreateOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={createMutation.isPending} disabled={!createForm.classId || !createForm.subjectId}>
                Create Assignment
              </Button>
            </div>
          </form>
        </Modal>

        {/* Edit Assignment Modal */}
        <Modal isOpen={!!editingAssignment} onClose={() => setEditingAssignment(null)} title="Edit Assignment" size="lg">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              if (editingAssignment) {
                updateMutation.mutate({
                  id: editingAssignment.id,
                  payload: {
                    ...updateForm,
                    dueDate: updateForm.dueDate ? new Date(updateForm.dueDate).toISOString() : undefined,
                  },
                });
              }
            }}
            className="space-y-4"
          >
            <Input
              label="Title"
              value={updateForm.title}
              onChange={(e) => setUpdateForm({ ...updateForm, title: e.target.value })}
              required
            />
            <Textarea
              label="Description & Instructions"
              value={updateForm.description}
              onChange={(e) => setUpdateForm({ ...updateForm, description: e.target.value })}
              required
            />
            <div className="grid grid-cols-2 gap-3">
              <Input
                label="Due Date & Time"
                type="datetime-local"
                value={updateForm.dueDate}
                onChange={(e) => setUpdateForm({ ...updateForm, dueDate: e.target.value })}
                required
              />
              <Input
                label="Max Marks"
                type="number"
                min={1}
                value={updateForm.totalMarks}
                onChange={(e) => setUpdateForm({ ...updateForm, totalMarks: Number(e.target.value) })}
                required
              />
            </div>
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setEditingAssignment(null)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={updateMutation.isPending}>
                Save Changes
              </Button>
            </div>
          </form>
        </Modal>

        {/* Delete Confirmation Modal */}
        <Modal isOpen={!!deletingAssignment} onClose={() => setDeletingAssignment(null)} title="Delete Assignment">
          <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Are you sure you want to delete assignment <strong className="text-foreground">{deletingAssignment?.title}</strong>?
            </p>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="outline" onClick={() => setDeletingAssignment(null)}>
                Cancel
              </Button>
              <Button
                variant="destructive"
                isLoading={deleteMutation.isPending}
                onClick={() => deletingAssignment && deleteMutation.mutate(deletingAssignment.id)}
              >
                Delete Assignment
              </Button>
            </div>
          </div>
        </Modal>
      </div>
    </ProtectedRoute>
  );
}
