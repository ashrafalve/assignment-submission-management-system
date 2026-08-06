'use client';

import React from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useTheme } from '@/contexts/ThemeContext';
import { HiSun, HiMoon, HiArrowRightOnRectangle, HiBars3 } from 'react-icons/hi2';
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
    <header className="sticky top-0 z-30 flex h-16 w-full items-center justify-between border-b border-border/80 bg-background/70 px-4 backdrop-blur-xl transition-all sm:px-6">
      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          className="lg:hidden"
          onClick={onToggleSidebar}
          aria-label="Toggle menu"
        >
          <HiBars3 className="h-5 w-5" />
        </Button>

        <div className="flex items-center gap-2.5">
          <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-gradient-to-tr from-indigo-600 via-indigo-500 to-purple-500 text-white font-black text-sm tracking-wider shadow-lg shadow-indigo-500/25">
            AF
          </div>
          <span className="hidden font-extrabold tracking-tight text-foreground sm:inline-block sm:text-lg">
            Academia<span className="text-indigo-500">Flow</span>
          </span>
        </div>
      </div>

      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          onClick={toggleTheme}
          aria-label="Toggle Theme"
          className="rounded-full hover:bg-accent/80 transition-colors"
        >
          {theme === 'dark' ? (
            <HiSun className="h-5 w-5 text-amber-400" />
          ) : (
            <HiMoon className="h-5 w-5 text-slate-700" />
          )}
        </Button>

        {user && (
          <div className="flex items-center gap-3 border-l border-border/70 pl-3">
            <Avatar name={user.fullName} size="sm" />
            <div className="hidden flex-col text-left md:flex">
              <span className="text-xs font-bold leading-none text-foreground">
                {user.fullName}
              </span>
              <span className="mt-1 text-[11px] text-muted-foreground">{user.email}</span>
            </div>
            <Badge
              variant={
                user.role === 'Admin' ? 'destructive' : user.role === 'Teacher' ? 'info' : 'success'
              }
              className="ml-1 uppercase tracking-wider text-[10px] px-2 py-0.5 font-bold"
            >
              {user.role}
            </Badge>

            <Button
              variant="ghost"
              size="icon"
              onClick={logout}
              title="Logout"
              className="text-muted-foreground hover:text-destructive hover:bg-destructive/10 transition-colors"
            >
              <HiArrowRightOnRectangle className="h-5 w-5" />
            </Button>
          </div>
        )}
      </div>
    </header>
  );
}
