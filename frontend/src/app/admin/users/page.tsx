'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { adminService, CreateUserPayload, UpdateUserPayload } from '@/services/admin-service';
import { DataTable, Column } from '@/components/ui/data-table';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Select } from '@/components/ui/form';
import { Modal } from '@/components/ui/modal';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/contexts/ToastContext';
import { User, UserRole } from '@/types/auth';
import { UserPlus, Edit, Trash2, KeyRound } from 'lucide-react';

export default function AdminUsersPage() {
  const queryClient = useQueryClient();
  const { success, error: toastError } = useToast();

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [roleFilter, setRoleFilter] = useState<string>('');

  // Modals state
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [deletingUser, setDeletingUser] = useState<User | null>(null);
  const [changingPassUser, setChangingPassUser] = useState<User | null>(null);

  // Form states
  const [createForm, setCreateForm] = useState<CreateUserPayload>({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    role: 'Student',
    classId: '',
  });

  const [updateForm, setUpdateForm] = useState<UpdateUserPayload>({
    firstName: '',
    lastName: '',
    role: 'Student',
    isActive: true,
    classId: '',
  });

  const [passForm, setPassForm] = useState({ newPassword: '', confirmPassword: '' });

  // 1. Fetch Users Query
  const { data, isLoading } = useQuery({
    queryKey: ['admin-users', page, pageSize, searchTerm, roleFilter],
    queryFn: () => adminService.getUsers({ pageNumber: page, pageSize, searchTerm, role: roleFilter || undefined }),
  });

  // 2. Fetch Classes for Student enrollment dropdown
  const { data: classesData } = useQuery({
    queryKey: ['admin-classes-list'],
    queryFn: () => adminService.getClasses({ pageNumber: 1, pageSize: 100 }),
  });

  // Mutations
  const createMutation = useMutation({
    mutationFn: adminService.createUser,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] });
      success('User created successfully');
      setIsCreateOpen(false);
      setCreateForm({ firstName: '', lastName: '', email: '', password: '', role: 'Student', classId: '' });
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to create user'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateUserPayload }) =>
      adminService.updateUser(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] });
      success('User updated successfully');
      setEditingUser(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to update user'),
  });

  const passMutation = useMutation({
    mutationFn: ({ id, newPassword, confirmPassword }: { id: string; newPassword: string; confirmPassword: string }) =>
      adminService.changePassword(id, { newPassword, confirmPassword }),
    onSuccess: () => {
      success('Password changed successfully');
      setChangingPassUser(null);
      setPassForm({ newPassword: '', confirmPassword: '' });
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to change password'),
  });

  const deleteMutation = useMutation({
    mutationFn: adminService.deleteUser,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] });
      success('User soft-deleted successfully');
      setDeletingUser(null);
    },
    onError: (err: any) => toastError(err.response?.data?.message || 'Failed to delete user'),
  });

  const columns: Column<User>[] = [
    { key: 'fullName', header: 'Name', sortable: true },
    { key: 'email', header: 'Email', sortable: true },
    {
      key: 'role',
      header: 'Role',
      sortable: true,
      cell: (user) => (
        <Badge
          variant={user.role === 'Admin' ? 'destructive' : user.role === 'Teacher' ? 'info' : 'success'}
        >
          {user.role}
        </Badge>
      ),
    },
    {
      key: 'className',
      header: 'Enrolled Class',
      cell: (user) => user.className ? <span className="font-medium text-foreground">{user.className}</span> : <span className="text-muted-foreground text-xs">—</span>,
    },
    {
      key: 'isActive',
      header: 'Status',
      cell: (user) => (
        <Badge variant={user.isActive ? 'success' : 'outline'}>
          {user.isActive ? 'Active' : 'Inactive'}
        </Badge>
      ),
    },
    {
      key: 'actions',
      header: 'Actions',
      className: 'text-right',
      cell: (user) => (
        <div className="flex items-center justify-end gap-1">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => {
              setEditingUser(user);
              setUpdateForm({
                firstName: user.firstName,
                lastName: user.lastName,
                role: user.role,
                isActive: user.isActive,
                classId: user.classId || '',
              });
            }}
            title="Edit User"
          >
            <Edit className="h-4 w-4 text-blue-500" />
          </Button>

          <Button
            variant="ghost"
            size="icon"
            onClick={() => setChangingPassUser(user)}
            title="Change Password"
          >
            <KeyRound className="h-4 w-4 text-amber-500" />
          </Button>

          <Button
            variant="ghost"
            size="icon"
            onClick={() => setDeletingUser(user)}
            title="Delete User"
          >
            <Trash2 className="h-4 w-4 text-rose-500" />
          </Button>
        </div>
      ),
    },
  ];

  const classOptions = [
    { label: 'None / Select Class', value: '' },
    ...(classesData?.items.map((c) => ({ label: `${c.name} (${c.academicYear})`, value: c.id })) || []),
  ];

  return (
    <ProtectedRoute allowedRoles={['Admin']}>
      <div className="space-y-6">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-foreground">User Management</h1>
            <p className="text-sm text-muted-foreground mt-1">Manage system administrators, teachers, and students.</p>
          </div>
          <Button onClick={() => setIsCreateOpen(true)} className="gap-2 font-semibold shadow-md">
            <UserPlus className="h-4 w-4" />
            Add User
          </Button>
        </div>

        {/* Filter Controls */}
        <div className="flex flex-wrap items-center gap-3">
          <Select
            value={roleFilter}
            onChange={(e) => {
              setRoleFilter(e.target.value);
              setPage(1);
            }}
            options={[
              { label: 'All Roles', value: '' },
              { label: 'Admin', value: 'Admin' },
              { label: 'Teacher', value: 'Teacher' },
              { label: 'Student', value: 'Student' },
            ]}
            className="w-40"
          />
        </div>

        {/* User Data Table */}
        <DataTable
          columns={columns}
          data={data?.items || []}
          isLoading={isLoading}
          keyExtractor={(u) => u.id}
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

        {/* Create User Modal */}
        <Modal isOpen={isCreateOpen} onClose={() => setIsCreateOpen(false)} title="Create New User">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              createMutation.mutate({
                ...createForm,
                classId: createForm.role === 'Student' && createForm.classId ? createForm.classId : undefined,
              });
            }}
            className="space-y-4"
          >
            <div className="grid grid-cols-2 gap-3">
              <Input
                label="First Name"
                value={createForm.firstName}
                onChange={(e) => setCreateForm({ ...createForm, firstName: e.target.value })}
                required
              />
              <Input
                label="Last Name"
                value={createForm.lastName}
                onChange={(e) => setCreateForm({ ...createForm, lastName: e.target.value })}
                required
              />
            </div>
            <Input
              label="Email Address"
              type="email"
              value={createForm.email}
              onChange={(e) => setCreateForm({ ...createForm, email: e.target.value })}
              required
            />
            <Input
              label="Password"
              type="password"
              value={createForm.password}
              onChange={(e) => setCreateForm({ ...createForm, password: e.target.value })}
              required
            />
            <Select
              label="User Role"
              value={createForm.role}
              onChange={(e) => setCreateForm({ ...createForm, role: e.target.value as UserRole })}
              options={[
                { label: 'Student', value: 'Student' },
                { label: 'Teacher', value: 'Teacher' },
                { label: 'Admin', value: 'Admin' },
              ]}
            />
            {createForm.role === 'Student' && (
              <Select
                label="Enroll in Class"
                value={createForm.classId || ''}
                onChange={(e) => setCreateForm({ ...createForm, classId: e.target.value })}
                options={classOptions}
              />
            )}
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setIsCreateOpen(false)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={createMutation.isPending}>
                Create User
              </Button>
            </div>
          </form>
        </Modal>

        {/* Edit User Modal */}
        <Modal isOpen={!!editingUser} onClose={() => setEditingUser(null)} title="Edit User Details">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              if (editingUser) {
                updateMutation.mutate({
                  id: editingUser.id,
                  payload: {
                    ...updateForm,
                    classId: updateForm.role === 'Student' ? updateForm.classId || undefined : undefined,
                  },
                });
              }
            }}
            className="space-y-4"
          >
            <div className="grid grid-cols-2 gap-3">
              <Input
                label="First Name"
                value={updateForm.firstName}
                onChange={(e) => setUpdateForm({ ...updateForm, firstName: e.target.value })}
                required
              />
              <Input
                label="Last Name"
                value={updateForm.lastName}
                onChange={(e) => setUpdateForm({ ...updateForm, lastName: e.target.value })}
                required
              />
            </div>
            <Select
              label="User Role"
              value={updateForm.role}
              onChange={(e) => setUpdateForm({ ...updateForm, role: e.target.value as UserRole })}
              options={[
                { label: 'Student', value: 'Student' },
                { label: 'Teacher', value: 'Teacher' },
                { label: 'Admin', value: 'Admin' },
              ]}
            />
            {updateForm.role === 'Student' && (
              <Select
                label="Enrolled Class"
                value={updateForm.classId || ''}
                onChange={(e) => setUpdateForm({ ...updateForm, classId: e.target.value })}
                options={classOptions}
              />
            )}
            <Select
              label="Account Status"
              value={updateForm.isActive ? 'true' : 'false'}
              onChange={(e) => setUpdateForm({ ...updateForm, isActive: e.target.value === 'true' })}
              options={[
                { label: 'Active', value: 'true' },
                { label: 'Inactive / Suspended', value: 'false' },
              ]}
            />
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setEditingUser(null)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={updateMutation.isPending}>
                Save Changes
              </Button>
            </div>
          </form>
        </Modal>

        {/* Change Password Modal */}
        <Modal isOpen={!!changingPassUser} onClose={() => setChangingPassUser(null)} title={`Change Password for ${changingPassUser?.fullName}`}>
          <form
            onSubmit={(e) => {
              e.preventDefault();
              if (changingPassUser) {
                passMutation.mutate({
                  id: changingPassUser.id,
                  newPassword: passForm.newPassword,
                  confirmPassword: passForm.confirmPassword,
                });
              }
            }}
            className="space-y-4"
          >
            <Input
              label="New Password"
              type="password"
              value={passForm.newPassword}
              onChange={(e) => setPassForm({ ...passForm, newPassword: e.target.value })}
              required
            />
            <Input
              label="Confirm Password"
              type="password"
              value={passForm.confirmPassword}
              onChange={(e) => setPassForm({ ...passForm, confirmPassword: e.target.value })}
              required
            />
            <div className="flex justify-end gap-2 pt-4">
              <Button type="button" variant="outline" onClick={() => setChangingPassUser(null)}>
                Cancel
              </Button>
              <Button type="submit" isLoading={passMutation.isPending}>
                Update Password
              </Button>
            </div>
          </form>
        </Modal>

        {/* Soft Delete Modal */}
        <Modal isOpen={!!deletingUser} onClose={() => setDeletingUser(null)} title="Delete User Account">
          <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Are you sure you want to soft-delete <strong className="text-foreground">{deletingUser?.fullName}</strong> ({deletingUser?.email})?
            </p>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="outline" onClick={() => setDeletingUser(null)}>
                Cancel
              </Button>
              <Button
                variant="destructive"
                isLoading={deleteMutation.isPending}
                onClick={() => deletingUser && deleteMutation.mutate(deletingUser.id)}
              >
                Delete Account
              </Button>
            </div>
          </div>
        </Modal>
      </div>
    </ProtectedRoute>
  );
}
