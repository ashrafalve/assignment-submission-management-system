'use client';

import React from 'react';
import Link from 'next/link';
import { useQuery } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { useAuth } from '@/contexts/AuthContext';
import { studentService } from '@/services/student-service';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { GraduationCap, FileSpreadsheet, FileCheck, Award, ArrowRight } from 'lucide-react';

export default function StudentDashboardPage() {
  const { user } = useAuth();

  const { data: assignmentsData } = useQuery({
    queryKey: ['student-assignments-dashboard'],
    queryFn: () => studentService.getPublishedAssignments({ pageSize: 100 }),
  });

  const { data: submissionsData } = useQuery({
    queryKey: ['student-submissions-dashboard'],
    queryFn: studentService.getMySubmissions,
  });

  const totalAssignments = assignmentsData?.totalCount || 0;
  const totalSubmissions = submissionsData?.length || 0;
  const gradedCount = submissionsData?.filter((s) => s.status === 'Graded').length || 0;

  return (
    <ProtectedRoute allowedRoles={['Student']}>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground">Student Portal</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Welcome back, <strong className="text-foreground">{user?.fullName}</strong>. View published class assignments, submit work before deadlines, and check teacher feedback.
          </p>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <Card className="border-l-4 border-l-primary">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">My Enrolled Class</CardTitle>
              <GraduationCap className="h-5 w-5 text-primary" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{user?.className || 'Class Enrolled'}</div>
              <p className="text-xs text-muted-foreground mt-1">{user?.email}</p>
            </CardContent>
          </Card>

          <Card className="border-l-4 border-l-blue-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Class Assignments</CardTitle>
              <FileSpreadsheet className="h-5 w-5 text-blue-500" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{totalAssignments}</div>
              <p className="text-xs text-muted-foreground mt-1">Published Work</p>
            </CardContent>
          </Card>

          <Card className="border-l-4 border-l-amber-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">My Submissions</CardTitle>
              <FileCheck className="h-5 w-5 text-amber-500" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{totalSubmissions}</div>
              <p className="text-xs text-muted-foreground mt-1">Submitted Tasks</p>
            </CardContent>
          </Card>

          <Card className="border-l-4 border-l-emerald-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Graded Tasks</CardTitle>
              <Award className="h-5 w-5 text-emerald-500" />
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-extrabold">{gradedCount}</div>
              <p className="text-xs text-muted-foreground mt-1">With Instructor Marks</p>
            </CardContent>
          </Card>
        </div>

        {/* Action Navigation Cards */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 pt-4">
          <Link href="/student/assignments" className="group">
            <Card className="h-full transition-all group-hover:border-primary/50 group-hover:shadow-lg">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3">
                  <div className="p-3 rounded-xl bg-primary/10 text-primary">
                    <FileSpreadsheet className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-primary transition-colors">Class Assignments</h3>
                    <p className="text-xs text-muted-foreground">View instructions and submit your work before deadlines</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-semibold text-primary gap-1">
                  View Class Work <ArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/student/submissions" className="group">
            <Card className="h-full transition-all group-hover:border-emerald-500/50 group-hover:shadow-lg">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3">
                  <div className="p-3 rounded-xl bg-emerald-500/10 text-emerald-500">
                    <FileCheck className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-emerald-500 transition-colors">Submissions & Feedback</h3>
                    <p className="text-xs text-muted-foreground">Review your submitted work, assigned scores, and teacher comments</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-semibold text-emerald-500 gap-1">
                  Check Grades <ArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>
        </div>
      </div>
    </ProtectedRoute>
  );
}
