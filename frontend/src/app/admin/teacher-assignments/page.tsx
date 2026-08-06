'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { adminService, AssignTeacherPayload } from '@/services/admin-service';
import { DataTable, Column } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { Select } from '@/components/ui/form';
import { Modal } from '@/components/ui/modal';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/contexts/ToastContext';
import { TeacherSubject } from '@/types/domain';
import { UserCheck, Trash2, UserPlus } from 'lucide-react';

export default function AdminTeacherAssignmentsPage() {
  const queryClient = useQueryClient();
  const { success, error: toastError } = useToast();

  const [isAssignOpen, setIsAssignOpen] = useState(false);
  const [deletingAssignment, setDeletingAssignment] = useState<TeacherSubject | null>(null);

  const [assignForm, setAssignForm] = useState<AssignTeacherPayload>({
    teacherId: '',
    subjectId: '',
    classId: '',
  });

  // Queries
  const { data: assignments, isLoading } = useQuery({
    queryKey: ['admin-teacher-assignments'],
    queryFn: adminService.getTeacherAssignments,
  });

  const { data: teachersData } = useQuery({
    queryKey: ['admin-teachers-list'],
    queryFn: () => adminService.getUsers({ role: 'Teacher', pageSize: 100 }),
  });

  const { data: subjectsData } = useQuery({
    queryKey: ['admin-subjects-list'],
    queryFn: () => adminService.getSubjects({ pageSize: 100 }),
  });

  const { data: classesData } = useQuery({
    queryKey: ['admin-classes-list'],
    queryFn: () => adminService.getClasses({ pageSize: 100 }),
  });

  // Mutations
  const assignMutation = useMutation({
    mutationFn: adminService.assignTeacher,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-teacher-assignments'] });
      success('Teacher assigned successfully');
      setIsAssignOpen(false);
      setAssignForm({ teacherId: '', subjectId: '', classId: '' });
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to assign teacher'),
  });

  const removeMutation = useMutation({
    mutationFn: adminService.removeTeacherAssignment,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-teacher-assignments'] });
      success('Teacher assignment removed');
      setDeletingAssignment(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to remove assignment'),
  });

  const columns: Column<TeacherSubject>[] = [
    { key: 'teacherName', header: 'Teacher Name', sortable: true },
    {
      key: 'subjectName',
      header: 'Subject',
      sortable: true,
      cell: (ts) => (
        <div className="flex items-center gap-2">
          <Badge variant="outline" className="font-mono text-xs text-primary">{ts.subjectCode}</Badge>
          <span>{ts.subjectName}</span>
        </div>
      ),
    },
    { key: 'className', header: 'Class Section', sortable: true },
    {
      key: 'assignedAt',
      header: 'Assigned Date',
      cell: (ts) => new Date(ts.assignedAt).toLocaleDateString(),
    },
    {
      key: 'actions',
      header: 'Actions',
      className: 'text-right',
      cell: (ts) => (
        <Button
          variant="ghost"
          size="icon"
          onClick={() => setDeletingAssignment(ts)}
          title="Remove Assignment"
        >
          <Trash2 className="h-4 w-4 text-rose-500" />
        </Button>
      ),
    },
  ];

  const teacherOptions = [
    { label: 'Select Teacher', value: '' },
    ...(teachersData?.items.map((t) => ({ label: `${t.fullName} (${t.email})`, value: t.id })) || []),
  ];

  const subjectOptions = [
    { label: 'Select Subject', value: '' },
    ...(subjectsData?.items.map((s) => ({ label: `${s.name} (${s.code})`, value: s.id })) || []),
  ];

  const classOptions = [
    { label: 'Select Class', value: '' },
    ...(classesData?.items.map((c) => ({ label: `${c.name} (${c.academicYear})`, value: c.id })) || []),
  ];

  return (
    <ProtectedRoute allowedRoles={['Admin']}>
      <div className="space-y-6">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-foreground">Teacher Assignments</h1>
            <p className="text-sm text-muted-foreground mt-1">Assign teachers to specific subjects and class sections.</p>
          </div>
          <Button onClick={() => setIsAssignOpen(true)} className="gap-2 font-semibold shadow-md">
            <UserPlus className="h-4 w-4" />
            Assign Teacher
          </Button>
        </div>

        <DataTable
          columns={columns}
          data={assignments || []}
          isLoading={isLoading}
          keyExtractor={(ts) => ts.id}
          emptyTitle="No Teacher Assignments"
          emptyDescription="Assign a teacher to a subject and class to enable them to create assignments."
        />

        {/* Assign Teacher Modal */}
        <Modal isOpen={isAssignOpen} onClose={() => setIsAssignOpen(false)} title="Assign Teacher to Subject & Class">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              assignMutation.mutate(assignForm);
            }}
            className="space-y-4"
          >
            <Select
              label="Teacher"
              value={assignForm.teacherId}
              onChange={(e) => setAssignForm({ ...assignForm, teacherId: e.target.value })}
              options={teacherOptions}
              required
            />
            <Select
              label="Subject"
              value={assignForm.subjectId}
              onChange={(e) => setAssignForm({ ...assignForm, subjectId: e.target.value })}
              options={subjectOptions}
              required
            />
            <Select
              label="Class Section"
              value={assignForm.classId}
              onChange={(e) => setAssignForm({ ...assignForm, classId: e.target.value })}
              options={classOptions}
              required
            />
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setIsAssignOpen(false)}>
                Cancel
              </Button>
              <Button
                type="submit"
                isLoading={assignMutation.isPending}
                disabled={!assignForm.teacherId || !assignForm.subjectId || !assignForm.classId}
              >
                Assign Teacher
              </Button>
            </div>
          </form>
        </Modal>

        {/* Remove Assignment Modal */}
        <Modal isOpen={!!deletingAssignment} onClose={() => setDeletingAssignment(null)} title="Remove Teacher Assignment">
          <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Are you sure you want to remove assignment of teacher <strong className="text-foreground">{deletingAssignment?.teacherName}</strong> from subject <strong className="text-foreground">{deletingAssignment?.subjectName}</strong> ({deletingAssignment?.className})?
            </p>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="outline" onClick={() => setDeletingAssignment(null)}>
                Cancel
              </Button>
              <Button
                variant="destructive"
                isLoading={removeMutation.isPending}
                onClick={() => deletingAssignment && removeMutation.mutate(deletingAssignment.id)}
              >
                Remove Assignment
              </Button>
            </div>
          </div>
        </Modal>
      </div>
    </ProtectedRoute>
  );
}
