'use client';

import React from 'react';
import Link from 'next/link';
import { useQuery } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { useAuth } from '@/contexts/AuthContext';
import { teacherService } from '@/services/teacher-service';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { School, FileSpreadsheet, CheckSquare, Plus, ArrowRight } from 'lucide-react';

export default function TeacherDashboardPage() {
  const { user } = useAuth();

  const { data: assignmentsData } = useQuery({
    queryKey: ['teacher-assignments-dashboard'],
    queryFn: () => teacherService.getAssignments({ pageSize: 100 }),
  });

  const totalAssignments = assignmentsData?.totalCount || 0;
  const publishedCount = assignmentsData?.items.filter((a) => a.status === 'Published').length || 0;
  const draftCount = assignmentsData?.items.filter((a) => a.status === 'Draft').length || 0;

  return (
    <ProtectedRoute allowedRoles={['Teacher']}>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground">Teacher Dashboard</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Welcome back, <strong className="text-foreground">{user?.fullName}</strong>. Manage your coursework, publish assignments, and grade student submissions.
          </p>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <Card className="border-l-4 border-l-primary">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Total Assignments</CardTitle>
              <FileSpreadsheet className="h-5 w-5 text-primary" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{totalAssignments}</div>
              <p className="text-xs text-muted-foreground mt-1">Created Coursework</p>
            </CardContent>
          </Card>

          <Card className="border-l-4 border-l-emerald-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Published</CardTitle>
              <School className="h-5 w-5 text-emerald-500" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{publishedCount}</div>
              <p className="text-xs text-muted-foreground mt-1">Visible to Students</p>
            </CardContent>
          </Card>

          <Card className="border-l-4 border-l-amber-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Drafts</CardTitle>
              <CheckSquare className="h-5 w-5 text-amber-500" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{draftCount}</div>
              <p className="text-xs text-muted-foreground mt-1">Unpublished Work</p>
            </CardContent>
          </Card>
        </div>

        {/* Action Navigation Cards */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 pt-4">
          <Link href="/teacher/assignments" className="group">
            <Card className="h-full transition-all group-hover:border-primary/50 group-hover:shadow-lg">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3">
                  <div className="p-3 rounded-xl bg-primary/10 text-primary">
                    <FileSpreadsheet className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-primary transition-colors">Assignments Management</h3>
                    <p className="text-xs text-muted-foreground">Create, edit, publish, or save draft assignments</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-semibold text-primary gap-1">
                  Manage Coursework <ArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/teacher/submissions" className="group">
            <Card className="h-full transition-all group-hover:border-emerald-500/50 group-hover:shadow-lg">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3">
                  <div className="p-3 rounded-xl bg-emerald-500/10 text-emerald-500">
                    <CheckSquare className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-emerald-500 transition-colors">Review & Grade Submissions</h3>
                    <p className="text-xs text-muted-foreground">Review student work, assign marks, and give feedback</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-semibold text-emerald-500 gap-1">
                  Review Submissions <ArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>
        </div>
      </div>
    </ProtectedRoute>
  );
}
