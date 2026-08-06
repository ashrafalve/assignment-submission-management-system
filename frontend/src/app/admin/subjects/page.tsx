'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { adminService, CreateSubjectPayload, UpdateSubjectPayload } from '@/services/admin-service';
import { DataTable, Column } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/form';
import { Modal } from '@/components/ui/modal';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/contexts/ToastContext';
import { Subject } from '@/types/domain';
import { Plus, Edit, Trash2, BookOpen } from 'lucide-react';

export default function AdminSubjectsPage() {
  const queryClient = useQueryClient();
  const { success, error: toastError } = useToast();

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');

  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingSubject, setEditingSubject] = useState<Subject | null>(null);
  const [deletingSubject, setDeletingSubject] = useState<Subject | null>(null);

  const [createForm, setCreateForm] = useState<CreateSubjectPayload>({
    name: '',
    code: '',
    description: '',
  });

  const [updateForm, setUpdateForm] = useState<UpdateSubjectPayload>({
    name: '',
    code: '',
    description: '',
    isActive: true,
  });

  const { data, isLoading } = useQuery({
    queryKey: ['admin-subjects', page, pageSize, searchTerm],
    queryFn: () => adminService.getSubjects({ pageNumber: page, pageSize, searchTerm }),
  });

  const createMutation = useMutation({
    mutationFn: adminService.createSubject,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-subjects'] });
      success('Subject created successfully');
      setIsCreateOpen(false);
      setCreateForm({ name: '', code: '', description: '' });
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to create subject'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateSubjectPayload }) =>
      adminService.updateSubject(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-subjects'] });
      success('Subject updated successfully');
      setEditingSubject(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to update subject'),
  });

  const deleteMutation = useMutation({
    mutationFn: adminService.deleteSubject,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-subjects'] });
      success('Subject soft-deleted successfully');
      setDeletingSubject(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to delete subject'),
  });

  const columns: Column<Subject>[] = [
    {
      key: 'code',
      header: 'Subject Code',
      sortable: true,
      cell: (sub) => <span className="font-bold text-primary font-mono">{sub.code}</span>,
    },
    { key: 'name', header: 'Subject Name', sortable: true },
    {
      key: 'description',
      header: 'Description',
      cell: (sub) => sub.description || <span className="text-muted-foreground text-xs">—</span>,
    },
    {
      key: 'isActive',
      header: 'Status',
      cell: (sub) => (
        <Badge variant={sub.isActive ? 'success' : 'outline'}>
          {sub.isActive ? 'Active' : 'Inactive'}
        </Badge>
      ),
    },
    {
      key: 'actions',
      header: 'Actions',
      className: 'text-right',
      cell: (sub) => (
        <div className="flex items-center justify-end gap-1">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => {
              setEditingSubject(sub);
              setUpdateForm({
                name: sub.name,
                code: sub.code,
                description: sub.description || '',
                isActive: sub.isActive,
              });
            }}
          >
            <Edit className="h-4 w-4 text-blue-500" />
          </Button>
          <Button variant="ghost" size="icon" onClick={() => setDeletingSubject(sub)}>
            <Trash2 className="h-4 w-4 text-rose-500" />
          </Button>
        </div>
      ),
    },
  ];

  return (
    <ProtectedRoute allowedRoles={['Admin']}>
      <div className="space-y-6">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-foreground">Subject Management</h1>
            <p className="text-sm text-muted-foreground mt-1">Manage academic subjects and course codes.</p>
          </div>
          <Button onClick={() => setIsCreateOpen(true)} className="gap-2 font-semibold shadow-md">
            <Plus className="h-4 w-4" />
            Add Subject
          </Button>
        </div>

        <DataTable
          columns={columns}
          data={data?.items || []}
          isLoading={isLoading}
          keyExtractor={(s) => s.id}
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

        {/* Create Modal */}
        <Modal isOpen={isCreateOpen} onClose={() => setIsCreateOpen(false)} title="Create New Subject">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              createMutation.mutate(createForm);
            }}
            className="space-y-4"
          >
            <Input
              label="Subject Name"
              placeholder="e.g. Mathematics"
              value={createForm.name}
              onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })}
              required
            />
            <Input
              label="Subject Code"
              placeholder="e.g. MATH-101"
              value={createForm.code}
              onChange={(e) => setCreateForm({ ...createForm, code: e.target.value.toUpperCase() })}
              required
            />
            <Textarea
              label="Description (Optional)"
              value={createForm.description}
              onChange={(e) => setCreateForm({ ...createForm, description: e.target.value })}
            />
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setIsCreateOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={createMutation.isPending}>
                Create Subject
              </Button>
            </div>
          </form>
        </Modal>

        {/* Edit Modal */}
        <Modal isOpen={!!editingSubject} onClose={() => setEditingSubject(null)} title="Edit Subject">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              if (editingSubject) updateMutation.mutate({ id: editingSubject.id, payload: updateForm });
            }}
            className="space-y-4"
          >
            <Input
              label="Subject Name"
              value={updateForm.name}
              onChange={(e) => setUpdateForm({ ...updateForm, name: e.target.value })}
              required
            />
            <Input
              label="Subject Code"
              value={updateForm.code}
              onChange={(e) => setUpdateForm({ ...updateForm, code: e.target.value.toUpperCase() })}
              required
            />
            <Textarea
              label="Description"
              value={updateForm.description}
              onChange={(e) => setUpdateForm({ ...updateForm, description: e.target.value })}
            />
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setEditingSubject(null)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={updateMutation.isPending}>
                Save Changes
              </Button>
            </div>
          </form>
        </Modal>

        {/* Delete Modal */}
        <Modal isOpen={!!deletingSubject} onClose={() => setDeletingSubject(null)} title="Delete Subject">
          <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Are you sure you want to delete subject <strong className="text-foreground">{deletingSubject?.name}</strong> ({deletingSubject?.code})?
            </p>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="outline" onClick={() => setDeletingSubject(null)}>
                Cancel
              </Button>
              <Button
                variant="destructive"
                isLoading={deleteMutation.isPending}
                onClick={() => deletingSubject && deleteMutation.mutate(deletingSubject.id)}
              >
                Delete Subject
              </Button>
            </div>
          </div>
        </Modal>
      </div>
    </ProtectedRoute>
  );
}
