'use client';

import React from 'react';
import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { FileQuestion, ArrowLeft, Home } from 'lucide-react';

export default function NotFound() {
  return (
    <div className="flex min-h-[75vh] w-full flex-col items-center justify-center p-4 text-center">
      <div className="flex h-20 w-20 items-center justify-center rounded-3xl bg-primary/10 text-primary mb-6 shadow-inner">
        <FileQuestion className="h-10 w-10 animate-bounce" />
      </div>

      <h1 className="text-4xl font-extrabold tracking-tight text-foreground sm:text-5xl">
        404 - Page Not Found
      </h1>
      <p className="mt-3 text-base text-muted-foreground max-w-md">
        The page or resource you are looking for might have been moved, deleted, or does not exist.
      </p>

      <div className="mt-8 flex items-center gap-3">
        <Link href="/">
          <Button className="gap-2 font-semibold shadow-md">
            <Home className="h-4 w-4" />
            Go to Dashboard
          </Button>
        </Link>
      </div>
    </div>
  );
}
