'use client';

import React from 'react';
import Link from 'next/link';
import { useQuery } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { useAuth } from '@/contexts/AuthContext';
import { adminService } from '@/services/admin-service';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Shield, Users, GraduationCap, BookOpen, UserCheck, ArrowRight } from 'lucide-react';

export default function AdminDashboardPage() {
  const { user } = useAuth();

  const { data: usersData } = useQuery({
    queryKey: ['admin-users-count'],
    queryFn: () => adminService.getUsers({ pageSize: 1 }),
  });

  const { data: classesData } = useQuery({
    queryKey: ['admin-classes-count'],
    queryFn: () => adminService.getClasses({ pageSize: 1 }),
  });

  const { data: subjectsData } = useQuery({
    queryKey: ['admin-subjects-count'],
    queryFn: () => adminService.getSubjects({ pageSize: 1 }),
  });

  const { data: teacherAssignments } = useQuery({
    queryKey: ['admin-teacher-assignments'],
    queryFn: adminService.getTeacherAssignments,
  });

  return (
    <ProtectedRoute allowedRoles={['Admin']}>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground">Admin Portal Overview</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Welcome back, <strong className="text-foreground">{user?.fullName}</strong>. Real-time overview of users, classes, subjects, and teacher assignments.
          </p>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <Card className="border-l-4 border-l-primary">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Total Users</CardTitle>
              <Users className="h-5 w-5 text-primary" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{usersData?.totalCount ?? '...'}</div>
              <p className="text-xs text-muted-foreground mt-1">System Accounts</p>
            </CardContent>
          </Card>

          <Card className="border-l-4 border-l-emerald-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Active Classes</CardTitle>
              <GraduationCap className="h-5 w-5 text-emerald-500" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{classesData?.totalCount ?? '...'}</div>
              <p className="text-xs text-muted-foreground mt-1">School Sections</p>
            </CardContent>
          </Card>

          <Card className="border-l-4 border-l-amber-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Subjects</CardTitle>
              <BookOpen className="h-5 w-5 text-amber-500" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{subjectsData?.totalCount ?? '...'}</div>
              <p className="text-xs text-muted-foreground mt-1">Curriculum Courses</p>
            </CardContent>
          </Card>

          <Card className="border-l-4 border-l-blue-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Teacher Assignments</CardTitle>
              <UserCheck className="h-5 w-5 text-blue-500" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{teacherAssignments?.length ?? '...'}</div>
              <p className="text-xs text-muted-foreground mt-1">Course Mappings</p>
            </CardContent>
          </Card>
        </div>

        {/* Action Navigation Cards */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4 pt-4">
          <Link href="/admin/users" className="group">
            <Card className="h-full transition-all group-hover:border-primary/50 group-hover:shadow-lg">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3">
                  <div className="p-3 rounded-xl bg-primary/10 text-primary">
                    <Users className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-primary transition-colors">Users</h3>
                    <p className="text-xs text-muted-foreground">Manage user accounts</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-semibold text-primary gap-1">
                  Manage Users <ArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/admin/classes" className="group">
            <Card className="h-full transition-all group-hover:border-emerald-500/50 group-hover:shadow-lg">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3">
                  <div className="p-3 rounded-xl bg-emerald-500/10 text-emerald-500">
                    <GraduationCap className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-emerald-500 transition-colors">Classes</h3>
                    <p className="text-xs text-muted-foreground">Manage school sections</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-semibold text-emerald-500 gap-1">
                  Manage Classes <ArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/admin/subjects" className="group">
            <Card className="h-full transition-all group-hover:border-amber-500/50 group-hover:shadow-lg">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3">
                  <div className="p-3 rounded-xl bg-amber-500/10 text-amber-500">
                    <BookOpen className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-amber-500 transition-colors">Subjects</h3>
                    <p className="text-xs text-muted-foreground">Manage course codes</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-semibold text-amber-500 gap-1">
                  Manage Subjects <ArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/admin/teacher-assignments" className="group">
            <Card className="h-full transition-all group-hover:border-blue-500/50 group-hover:shadow-lg">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3">
                  <div className="p-3 rounded-xl bg-blue-500/10 text-blue-500">
                    <UserCheck className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-blue-500 transition-colors">Assignments</h3>
                    <p className="text-xs text-muted-foreground">Assign teachers</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-semibold text-blue-500 gap-1">
                  Map Teachers <ArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>
        </div>
      </div>
    </ProtectedRoute>
  );
}
