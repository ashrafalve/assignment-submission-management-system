'use client';

import React from 'react';
import Link from 'next/link';
import { useQuery } from '@tanstack/react-query';
import { ProtectedRoute } from '@/components/layout/ProtectedRoute';
import { useAuth } from '@/contexts/AuthContext';
import { studentService } from '@/services/student-service';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import {
  HiAcademicCap,
  HiDocumentText,
  HiCheckBadge,
  HiTrophy,
  HiSparkles,
  HiArrowRight,
} from 'react-icons/hi2';

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
      <div className="space-y-8 animate-in fade-in duration-300">
        <div>
          <div className="flex items-center gap-2 text-indigo-500 font-semibold text-xs uppercase tracking-widest mb-1">
            <HiSparkles className="h-4 w-4" />
            <span>Student Learning Portal</span>
          </div>
          <h1 className="text-3xl font-extrabold tracking-tight text-foreground">Student Dashboard</h1>
          <p className="text-sm text-muted-foreground mt-1 font-medium">
            Welcome back, <strong className="text-foreground">{user?.fullName}</strong>. View published class assignments, submit work before deadlines, and check teacher feedback.
          </p>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
          <Card className="relative overflow-hidden border-t-4 border-t-indigo-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground">My Enrolled Class</CardTitle>
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-indigo-500/15 text-indigo-500 font-bold">
                <HiAcademicCap className="h-5 w-5" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-xl font-bold tracking-tight text-foreground">{user?.className || 'Class Enrolled'}</div>
              <p className="text-xs text-muted-foreground mt-1 font-medium">{user?.email}</p>
            </CardContent>
          </Card>

          <Card className="relative overflow-hidden border-t-4 border-t-blue-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Class Assignments</CardTitle>
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-blue-500/15 text-blue-500 font-bold">
                <HiDocumentText className="h-5 w-5" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-black tracking-tight">{totalAssignments}</div>
              <p className="text-xs text-muted-foreground mt-1 font-medium">Published Tasks</p>
            </CardContent>
          </Card>

          <Card className="relative overflow-hidden border-t-4 border-t-amber-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground">My Submissions</CardTitle>
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-amber-500/15 text-amber-500 font-bold">
                <HiCheckBadge className="h-5 w-5" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-black tracking-tight">{totalSubmissions}</div>
              <p className="text-xs text-muted-foreground mt-1 font-medium">Submitted Tasks</p>
            </CardContent>
          </Card>

          <Card className="relative overflow-hidden border-t-4 border-t-emerald-500">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Graded Work</CardTitle>
              <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-emerald-500/15 text-emerald-500 font-bold">
                <HiTrophy className="h-5 w-5" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-black tracking-tight">{gradedCount}</div>
              <p className="text-xs text-muted-foreground mt-1 font-medium">With Scores & Feedback</p>
            </CardContent>
          </Card>
        </div>

        {/* Action Navigation Cards */}
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 pt-2">
          <Link href="/student/assignments" className="group">
            <Card className="h-full transition-all duration-200 group-hover:-translate-y-1 group-hover:border-indigo-500/50 group-hover:shadow-xl group-hover:shadow-indigo-500/10">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3.5">
                  <div className="p-3.5 rounded-2xl bg-indigo-500/15 text-indigo-500 transition-colors group-hover:bg-indigo-500 group-hover:text-white">
                    <HiDocumentText className="h-5 w-5" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-indigo-500 transition-colors text-base">Class Assignments</h3>
                    <p className="text-xs text-muted-foreground font-medium">View instructions and submit your work before deadlines</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-bold text-indigo-500 gap-1 pt-2">
                  View Class Work <HiArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>

          <Link href="/student/submissions" className="group">
            <Card className="h-full transition-all duration-200 group-hover:-translate-y-1 group-hover:border-emerald-500/50 group-hover:shadow-xl group-hover:shadow-emerald-500/10">
              <CardContent className="p-6 flex flex-col justify-between h-full space-y-4">
                <div className="flex items-center gap-3.5">
                  <div className="p-3.5 rounded-2xl bg-emerald-500/15 text-emerald-500 transition-colors group-hover:bg-emerald-500 group-hover:text-white">
                    <HiCheckBadge className="h-5 w-5" />
                  </div>
                  <div>
                    <h3 className="font-bold text-foreground group-hover:text-emerald-500 transition-colors text-base">Submissions & Feedback</h3>
                    <p className="text-xs text-muted-foreground font-medium">Review your submitted work, assigned scores, and teacher comments</p>
                  </div>
                </div>
                <div className="flex items-center text-xs font-bold text-emerald-500 gap-1 pt-2">
                  Check Grades <HiArrowRight className="h-3.5 w-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </CardContent>
            </Card>
          </Link>
        </div>
      </div>
    </ProtectedRoute>
  );
}
