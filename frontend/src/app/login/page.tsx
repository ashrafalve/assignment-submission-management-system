'use client';

import React from 'react';
import { LoginForm } from '@/features/auth/LoginForm';

export default function LoginPage() {
  return (
    <div className="relative flex min-h-screen w-full items-center justify-center p-4 overflow-hidden bg-background">
      {/* Background Gradient Blurs */}
      <div className="absolute -top-40 -left-40 h-96 w-96 rounded-full bg-primary/20 blur-3xl pointer-events-none" />
      <div className="absolute -bottom-40 -right-40 h-96 w-96 rounded-full bg-blue-500/20 blur-3xl pointer-events-none" />

      <div className="z-10 w-full max-w-md">
        <LoginForm />
      </div>
    </div>
  );
}
