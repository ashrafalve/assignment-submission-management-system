'use client';

import React, { useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { AlertOctagon, RefreshCw } from 'lucide-react';

export default function ErrorPage({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    console.error('Unhandled UI Exception:', error);
  }, [error]);

  return (
    <div className="flex min-h-[75vh] w-full flex-col items-center justify-center p-4 text-center">
      <div className="flex h-20 w-20 items-center justify-center rounded-3xl bg-destructive/10 text-destructive mb-6 shadow-inner">
        <AlertOctagon className="h-10 w-10 animate-pulse" />
      </div>

      <h1 className="text-3xl font-bold tracking-tight text-foreground sm:text-4xl">
        Something went wrong!
      </h1>
      <p className="mt-2 text-sm text-muted-foreground max-w-md">
        An unexpected error occurred while processing your request. Please try again or refresh the page.
      </p>

      {error.message && (
        <div className="mt-4 rounded-lg bg-destructive/10 border border-destructive/20 p-3 text-xs font-mono text-destructive max-w-lg">
          {error.message}
        </div>
      )}

      <div className="mt-6 flex items-center gap-3">
        <Button onClick={() => reset()} className="gap-2 font-semibold shadow-md">
          <RefreshCw className="h-4 w-4" />
          Try Again
        </Button>
      </div>
    </div>
  );
}
