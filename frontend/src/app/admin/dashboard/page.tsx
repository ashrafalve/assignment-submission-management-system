'use client';

import React from 'react';
import Link from 'next/link';
import { useQuery } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { useAuth } from '@/contexts/AuthContext';
import { adminService } from '@/services/admin-service';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import {
  HiUsers,
  HiAcademicCap,
  HiBookOpen,
  HiUserPlus,
  HiArrowRight,
  HiSparkles,
} from 'react-icons/hi2';

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
      <div className="space-y-8 animate-in fade-in duration-300">
        <div>
          <div className="flex items-center gap-2 text-indigo-500 font-semibold text-xs uppercase tracking-widest mb-1">
            <HiSparkles className="h-4 w-4" />
            <span>Administrator Control Center</span>
          </div>
          <h1 className="text-3xl font-extrabold tracking-tight text-foreground">Admin Portal Overview</h1>
          <p className="text-sm text-muted-foreground mt-1 font-medium">
            Welcome back, <strong className="text-foreground">{user?.fullName}</strong>. Real-time overview of users, classes, subjects, and teacher assignments.
          </p>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
          <Card className="relative overflow-hidden border-t-4 border-t-indigo-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Total Users</CardTitle>
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-indigo-500/15 text-indigo-500 font-bold">
                <HiUsers className="h-5 w-5" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-black tracking-tight">{usersData?.totalCount ?? '...'}</div>
              <p className="text-xs text-muted-foreground mt-1 font-medium">Active System Accounts</p>
            </CardContent>
          </Card>

          <Card className="relative overflow-hidden border-t-4 border-t-emerald-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Active Classes</CardTitle>
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-emerald-500/15 text-emerald-500 font-bold">
                <HiAcademicCap className="h-5 w-5" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-black tracking-tight">{classesData?.totalCount ?? '...'}</div>
              <p className="text-xs text-muted-foreground mt-1 font-medium">School Sections</p>
            </CardContent>
          </Card>

          <Card className="relative overflow-hidden border-t-4 border-t-amber-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Subjects</CardTitle>
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-amber-500/15 text-amber-500 font-bold">
                <HiBookOpen className="h-5 w-5" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-black tracking-tight">{subjectsData?.totalCount ?? '...'}</div>
              <p className="text-xs text-muted-foreground mt-1 font-medium">Curriculum Courses</p>
            </CardContent>
          </Card>

          <Card className="relative overflow-hidden border-t-4 border-t-purple-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Teacher Mappings</CardTitle>
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-purple-500/15 text-purple-500 font-bold">
                <HiUserPlus className="h-5 w-5" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-black tracking-tight">{teacherAssignments?.length ?? '...'}</div>
              <p className="text-xs text-muted-foreground mt-1 font-medium font-medium">Course Assignments</p>
            </CardContent>
          </Card>
        </div>

        {/* Action Navigation Cards */}
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 pt-2">
          <Link href="/admin/users" className="group">
            <Card className="h-full transition-all duration-200 group-hover:-translate-y-1 group-hover:border-indigo-500/50 group-hover:shadow-xl group-hover:shadow-indigo-500/10">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3.5">
                  <div className="p-3.5 rounded-2xl bg-indigo-500/15 text-indigo-500 transition-colors group-hover:bg-indigo-500 group-hover:text-white">
                    <HiUsers className="h-5 w-5" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-indigo-500 transition-colors text-base">Users</h3>
                    <p className="text-xs text-muted-foreground font-medium">Manage system accounts</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-bold text-indigo-500 gap-1 pt-2">
                  Manage Users <HiArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/admin/classes" className="group">
            <Card className="h-full transition-all duration-200 group-hover:-translate-y-1 group-hover:border-emerald-500/50 group-hover:shadow-xl group-hover:shadow-emerald-500/10">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3.5">
                  <div className="p-3.5 rounded-2xl bg-emerald-500/15 text-emerald-500 transition-colors group-hover:bg-emerald-500 group-hover:text-white">
                    <HiAcademicCap className="h-5 w-5" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-emerald-500 transition-colors text-base">Classes</h3>
                    <p className="text-xs text-muted-foreground font-medium">Manage school sections</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-bold text-emerald-500 gap-1 pt-2">
                  Manage Classes <HiArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/admin/subjects" className="group">
            <Card className="h-full transition-all duration-200 group-hover:-translate-y-1 group-hover:border-amber-500/50 group-hover:shadow-xl group-hover:shadow-amber-500/10">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3.5">
                  <div className="p-3.5 rounded-2xl bg-amber-500/15 text-amber-500 transition-colors group-hover:bg-amber-500 group-hover:text-white">
                    <HiBookOpen className="h-5 w-5" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-amber-500 transition-colors text-base">Subjects</h3>
                    <p className="text-xs text-muted-foreground font-medium">Manage course codes</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-bold text-amber-500 gap-1 pt-2">
                  Manage Subjects <HiArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/admin/teacher-assignments" className="group">
            <Card className="h-full transition-all duration-200 group-hover:-translate-y-1 group-hover:border-purple-500/50 group-hover:shadow-xl group-hover:shadow-purple-500/10">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3.5">
                  <div className="p-3.5 rounded-2xl bg-purple-500/15 text-purple-500 transition-colors group-hover:bg-purple-500 group-hover:text-white">
                    <HiUserPlus className="h-5 w-5" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-purple-500 transition-colors text-base">Assignments</h3>
                    <p className="text-xs text-muted-foreground font-medium">Assign teachers to classes</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-bold text-purple-500 gap-1 pt-2">
                  Map Teachers <HiArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>
        </div>
      </div>
    </ProtectedRoute>
  );
}
