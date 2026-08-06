'use client';

import React from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import {
  Users,
  GraduationCap,
  BookOpen,
  UserCheck,
  FileSpreadsheet,
  CheckSquare,
  FileCheck,
  LayoutDashboard,
  Shield,
  X,
} from 'lucide-react';
import { Button } from '@/components/ui/button';

interface SidebarProps {
  isOpen?: boolean;
  onClose?: () => void;
}

export function Sidebar({ isOpen = false, onClose }: SidebarProps) {
  const pathname = usePathname();
  const { user } = useAuth();
  const role = user?.role;

  const adminNav = [
    { label: 'Overview', href: '/admin/dashboard', icon: LayoutDashboard },
    { label: 'User Management', href: '/admin/users', icon: Users },
    { label: 'Class Management', href: '/admin/classes', icon: GraduationCap },
    { label: 'Subject Management', href: '/admin/subjects', icon: BookOpen },
    { label: 'Teacher Assignments', href: '/admin/teacher-assignments', icon: UserCheck },
  ];

  const teacherNav = [
    { label: 'Dashboard', href: '/teacher/dashboard', icon: LayoutDashboard },
    { label: 'My Assignments', href: '/teacher/assignments', icon: FileSpreadsheet },
    { label: 'Review Submissions', href: '/teacher/submissions', icon: CheckSquare },
  ];

  const studentNav = [
    { label: 'Dashboard', href: '/student/dashboard', icon: LayoutDashboard },
    { label: 'My Class Assignments', href: '/student/assignments', icon: FileSpreadsheet },
    { label: 'My Submissions', href: '/student/submissions', icon: FileCheck },
  ];

  const navItems =
    role === 'Admin' ? adminNav : role === 'Teacher' ? teacherNav : role === 'Student' ? studentNav : [];

  return (
    <>
      {/* Mobile Backdrop */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-background/80 backdrop-blur-sm lg:hidden"
          onClick={onClose}
        />
      )}

      {/* Sidebar Container */}
      <aside
        className={cn(
          'fixed top-16 bottom-0 left-0 z-40 flex w-64 flex-col border-r border-border bg-card p-4 transition-transform duration-200 ease-in-out lg:static lg:translate-x-0',
          isOpen ? 'translate-x-0' : '-translate-x-full'
        )}
      >
        <div className="mb-4 flex items-center justify-between lg:hidden">
          <span className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Navigation</span>
          <Button variant="ghost" size="icon" onClick={onClose}>
            <X className="h-4 w-4" />
          </Button>
        </div>

        {user && (
          <div className="mb-6 rounded-lg bg-muted/60 p-3">
            <div className="flex items-center gap-2">
              <Shield className="h-4 w-4 text-primary" />
              <span className="text-xs font-semibold text-foreground uppercase tracking-wider">{role} Portal</span>
            </div>
            {user.className && (
              <p className="mt-1 text-xs text-muted-foreground">Class: <span className="font-medium text-foreground">{user.className}</span></p>
            )}
          </div>
        )}

        <nav className="flex-1 space-y-1">
          {navItems.map((item) => {
            const Icon = item.icon;
            const isActive = pathname === item.href || pathname?.startsWith(item.href + '/');

            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={onClose}
                className={cn(
                  'flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors',
                  isActive
                    ? 'bg-primary text-primary-foreground font-semibold shadow-sm shadow-primary/20'
                    : 'text-muted-foreground hover:bg-accent hover:text-accent-foreground'
                )}
              >
                <Icon className={cn('h-4 w-4', isActive ? 'text-primary-foreground' : 'text-muted-foreground')} />
                {item.label}
              </Link>
            );
          })}
        </nav>

        <div className="border-t border-border pt-4 text-center">
          <p className="text-xs text-muted-foreground">AssignmentHub v1.0</p>
        </div>
      </aside>
    </>
  );
}
