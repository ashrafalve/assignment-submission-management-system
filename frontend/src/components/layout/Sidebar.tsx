'use client';

import React from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import {
  HiUsers,
  HiAcademicCap,
  HiBookOpen,
  HiUserPlus,
  HiDocumentText,
  HiClipboardDocumentCheck,
  HiCheckBadge,
  HiSquares2X2,
  HiShieldCheck,
  HiXMark,
} from 'react-icons/hi2';
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
    { label: 'Overview', href: '/admin/dashboard', icon: HiSquares2X2 },
    { label: 'User Management', href: '/admin/users', icon: HiUsers },
    { label: 'Class Management', href: '/admin/classes', icon: HiAcademicCap },
    { label: 'Subject Management', href: '/admin/subjects', icon: HiBookOpen },
    { label: 'Teacher Assignments', href: '/admin/teacher-assignments', icon: HiUserPlus },
  ];

  const teacherNav = [
    { label: 'Dashboard', href: '/teacher/dashboard', icon: HiSquares2X2 },
    { label: 'My Assignments', href: '/teacher/assignments', icon: HiDocumentText },
    { label: 'Review Submissions', href: '/teacher/submissions', icon: HiClipboardDocumentCheck },
  ];

  const studentNav = [
    { label: 'Dashboard', href: '/student/dashboard', icon: HiSquares2X2 },
    { label: 'My Class Assignments', href: '/student/assignments', icon: HiDocumentText },
    { label: 'My Submissions', href: '/student/submissions', icon: HiCheckBadge },
  ];

  const navItems =
    role === 'Admin' ? adminNav : role === 'Teacher' ? teacherNav : role === 'Student' ? studentNav : [];

  return (
    <>
      {/* Mobile Backdrop */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-background/80 backdrop-blur-md lg:hidden"
          onClick={onClose}
        />
      )}

      {/* Sidebar Container */}
      <aside
        className={cn(
          'fixed top-16 bottom-0 left-0 z-40 flex w-64 flex-col border-r border-border/80 bg-card/60 backdrop-blur-xl p-4 transition-transform duration-200 ease-in-out lg:static lg:translate-x-0',
          isOpen ? 'translate-x-0' : '-translate-x-full'
        )}
      >
        <div className="mb-3 flex items-center justify-between lg:hidden">
          <span className="text-[11px] font-extrabold uppercase tracking-widest text-muted-foreground">Navigation</span>
          <Button variant="ghost" size="icon" onClick={onClose}>
            <HiXMark className="h-5 w-5" />
          </Button>
        </div>

        {user && (
          <div className="mb-5 rounded-xl border border-border/60 bg-accent/40 p-3.5 shadow-xs">
            <div className="flex items-center gap-2">
              <div className="flex h-6 w-6 items-center justify-center rounded-lg bg-indigo-500/15 text-indigo-500 font-bold">
                <HiShieldCheck className="h-4 w-4" />
              </div>
              <span className="text-xs font-bold text-foreground uppercase tracking-wider">{role} Portal</span>
            </div>
            {user.className && (
              <p className="mt-1.5 text-xs text-muted-foreground">Class: <span className="font-semibold text-foreground">{user.className}</span></p>
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
                  'group relative flex items-center gap-3 rounded-xl px-3.5 py-2.5 text-sm font-semibold transition-all',
                  isActive
                    ? 'bg-indigo-500/10 text-indigo-600 dark:text-indigo-400 font-bold shadow-xs'
                    : 'text-muted-foreground hover:bg-accent/60 hover:text-foreground'
                )}
              >
                {isActive && (
                  <span className="absolute left-0 top-2 bottom-2 w-1 rounded-r-full bg-indigo-600 dark:bg-indigo-400" />
                )}
                <Icon className={cn('h-4.5 w-4.5 transition-colors', isActive ? 'text-indigo-600 dark:text-indigo-400' : 'text-muted-foreground group-hover:text-foreground')} />
                {item.label}
              </Link>
            );
          })}
        </nav>

        <div className="border-t border-border/60 pt-3.5 text-center">
          <p className="text-[11px] font-medium text-muted-foreground/80">AcademiaFlow Enterprise v1.0</p>
        </div>
      </aside>
    </>
  );
}
