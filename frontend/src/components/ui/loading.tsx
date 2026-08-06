import React from 'react';
import { cn } from '@/lib/utils';

export function Spinner({ className }: { className?: string }) {
  return (
    <div
      className={cn(
        'inline-block h-6 w-6 animate-spin rounded-full border-2 border-primary border-t-transparent',
        className
      )}
    />
  );
}

export function LoadingScreen({ message = 'Loading data...' }: { message?: string }) {
  return (
    <div className="flex flex-col items-center justify-center min-h-[50vh] w-full gap-3 p-6">
      <Spinner className="h-10 w-10 border-3" />
      <p className="text-sm font-medium text-muted-foreground animate-pulse">{message}</p>
    </div>
  );
}
