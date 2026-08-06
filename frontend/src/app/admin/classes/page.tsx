'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { adminService, CreateClassPayload, UpdateClassPayload } from '@/services/admin-service';
import { DataTable, Column } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/form';
import { Modal } from '@/components/ui/modal';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/contexts/ToastContext';
import { SchoolClass } from '@/types/domain';
import { Plus, Edit, Trash2, GraduationCap } from 'lucide-react';

export default function AdminClassesPage() {
  const queryClient = useQueryClient();
  const { success, error: toastError } = useToast();

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');

  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingClass, setEditingClass] = useState<SchoolClass | null>(null);
  const [deletingClass, setDeletingClass] = useState<SchoolClass | null>(null);

  const [createForm, setCreateForm] = useState<CreateClassPayload>({
    name: '',
    academicYear: '2026-2027',
    description: '',
  });

  const [updateForm, setUpdateForm] = useState<UpdateClassPayload>({
    name: '',
    academicYear: '',
    description: '',
    isActive: true,
  });

  const { data, isLoading } = useQuery({
    queryKey: ['admin-classes', page, pageSize, searchTerm],
    queryFn: () => adminService.getClasses({ pageNumber: page, pageSize, searchTerm }),
  });

  const createMutation = useMutation({
    mutationFn: adminService.createClass,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-classes'] });
      success('Class created successfully');
      setIsCreateOpen(false);
      setCreateForm({ name: '', academicYear: '2026-2027', description: '' });
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to create class'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateClassPayload }) =>
      adminService.updateClass(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-classes'] });
      success('Class updated successfully');
      setEditingClass(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to update class'),
  });

  const deleteMutation = useMutation({
    mutationFn: adminService.deleteClass,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-classes'] });
      success('Class soft-deleted successfully');
      setDeletingClass(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to delete class'),
  });

  const columns: Column<SchoolClass>[] = [
    { key: 'name', header: 'Class Name', sortable: true },
    { key: 'academicYear', header: 'Academic Year', sortable: true },
    {
      key: 'description',
      header: 'Description',
      cell: (cls) => cls.description || <span className="text-muted-foreground text-xs">—</span>,
    },
    {
      key: 'isActive',
      header: 'Status',
      cell: (cls) => (
        <Badge variant={cls.isActive ? 'success' : 'outline'}>
          {cls.isActive ? 'Active' : 'Inactive'}
        </Badge>
      ),
    },
    {
      key: 'actions',
      header: 'Actions',
      className: 'text-right',
      cell: (cls) => (
        <div className="flex items-center justify-end gap-1">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => {
              setEditingClass(cls);
              setUpdateForm({
                name: cls.name,
                academicYear: cls.academicYear,
                description: cls.description || '',
                isActive: cls.isActive,
              });
            }}
          >
            <Edit className="h-4 w-4 text-blue-500" />
          </Button>
          <Button variant="ghost" size="icon" onClick={() => setDeletingClass(cls)}>
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
            <h1 className="text-3xl font-bold tracking-tight text-foreground">Class Management</h1>
            <p className="text-sm text-muted-foreground mt-1">Configure academic classes and sections.</p>
          </div>
          <Button onClick={() => setIsCreateOpen(true)} className="gap-2 font-semibold shadow-md">
            <Plus className="h-4 w-4" />
            Add Class
          </Button>
        </div>

        <DataTable
          columns={columns}
          data={data?.items || []}
          isLoading={isLoading}
          keyExtractor={(c) => c.id}
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
        <Modal isOpen={isCreateOpen} onClose={() => setIsCreateOpen(false)} title="Create New Class">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              createMutation.mutate(createForm);
            }}
            className="space-y-4"
          >
            <Input
              label="Class Name"
              placeholder="e.g. Grade 10 - Section A"
              value={createForm.name}
              onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })}
              required
            />
            <Input
              label="Academic Year"
              placeholder="e.g. 2026-2027"
              value={createForm.academicYear}
              onChange={(e) => setCreateForm({ ...createForm, academicYear: e.target.value })}
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
                Create Class
              </Button>
            </div>
          </form>
        </Modal>

        {/* Edit Modal */}
        <Modal isOpen={!!editingClass} onClose={() => setEditingClass(null)} title="Edit Class">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              if (editingClass) updateMutation.mutate({ id: editingClass.id, payload: updateForm });
            }}
            className="space-y-4"
          >
            <Input
              label="Class Name"
              value={updateForm.name}
              onChange={(e) => setUpdateForm({ ...updateForm, name: e.target.value })}
              required
            />
            <Input
              label="Academic Year"
              value={updateForm.academicYear}
              onChange={(e) => setUpdateForm({ ...updateForm, academicYear: e.target.value })}
              required
            />
            <Textarea
              label="Description"
              value={updateForm.description}
              onChange={(e) => setUpdateForm({ ...updateForm, description: e.target.value })}
            />
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setEditingClass(null)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={updateMutation.isPending}>
                Save Changes
              </Button>
            </div>
          </form>
        </Modal>

        {/* Delete Modal */}
        <Modal isOpen={!!deletingClass} onClose={() => setDeletingClass(null)} title="Delete Class">
          <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Are you sure you want to delete class <strong className="text-foreground">{deletingClass?.name}</strong>?
            </p>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="outline" onClick={() => setDeletingClass(null)}>
                Cancel
              </Button>
              <Button
                variant="destructive"
                isLoading={deleteMutation.isPending}
                onClick={() => deletingClass && deleteMutation.mutate(deletingClass.id)}
              >
                Delete Class
              </Button>
            </div>
          </div>
        </Modal>
      </div>
    </ProtectedRoute>
  );
}
