'use client';

import React from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useTheme } from '@/contexts/ThemeContext';
import { Sun, Moon, LogOut, Menu, User, Bell } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Avatar } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';

interface NavbarProps {
  onToggleSidebar?: () => void;
}

export function Navbar({ onToggleSidebar }: NavbarProps) {
  const { user, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();

  return (
    <header className="sticky top-0 z-30 flex h-16 w-full items-center justify-between border-b border-border bg-background/80 px-4 backdrop-blur-md transition-colors sm:px-6">
      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          className="lg:hidden"
          onClick={onToggleSidebar}
          aria-label="Toggle menu"
        >
          <Menu className="h-5 w-5" />
        </Button>

        <div className="flex items-center gap-2">
          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary text-primary-foreground font-black tracking-wider shadow-md shadow-primary/20">
            AM
          </div>
          <span className="hidden font-bold tracking-tight text-foreground sm:inline-block sm:text-lg">
            Assignment<span className="text-primary">Hub</span>
          </span>
        </div>
      </div>

      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          onClick={toggleTheme}
          aria-label="Toggle Theme"
          className="rounded-full"
        >
          {theme === 'dark' ? <Sun className="h-5 w-5 text-amber-400" /> : <Moon className="h-5 w-5 text-slate-700" />}
        </Button>

        {user && (
          <div className="flex items-center gap-3 border-l border-border pl-3">
            <Avatar name={user.fullName} size="sm" />
            <div className="hidden flex-col text-left md:flex">
              <span className="text-sm font-semibold leading-none text-foreground">
                {user.fullName}
              </span>
              <span className="mt-1 text-xs text-muted-foreground">{user.email}</span>
            </div>
            <Badge
              variant={
                user.role === 'Admin' ? 'destructive' : user.role === 'Teacher' ? 'info' : 'success'
              }
              className="ml-1 uppercase tracking-wider text-[10px]"
            >
              {user.role}
            </Badge>

            <Button
              variant="ghost"
              size="icon"
              onClick={logout}
              title="Logout"
              className="text-muted-foreground hover:text-destructive transition-colors"
            >
              <LogOut className="h-4 w-4" />
            </Button>
          </div>
        )}
      </div>
    </header>
  );
}
