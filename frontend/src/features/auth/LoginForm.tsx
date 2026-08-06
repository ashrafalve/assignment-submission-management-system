'use client';

import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from '@/components/ui/card';
import { Lock, Mail, AlertCircle, Shield, GraduationCap, School } from 'lucide-react';
import { UserRole } from '@/types/auth';

const loginSchema = z.object({
  email: z.string().min(1, 'Email is required').email('Please enter a valid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

type LoginFormData = z.infer<typeof loginSchema>;

export function LoginForm() {
  const { login } = useAuth();
  const router = useRouter();
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: '',
    },
  });

  const onSubmit = async (data: LoginFormData) => {
    setErrorMessage(null);
    setIsLoading(true);

    try {
      const response = await login(data.email, data.password);
      
      // Role-based routing
      const redirectMap: Record<UserRole, string> = {
        Admin: '/admin/dashboard',
        Teacher: '/teacher/dashboard',
        Student: '/student/dashboard',
      };

      router.push(redirectMap[response.role] || '/login');
    } catch (err: any) {
      const msg = err.response?.data?.message || err.message || 'Invalid email or password. Please try again.';
      setErrorMessage(msg);
    } finally {
      setIsLoading(false);
    }
  };

  const setDemoCredentials = (email: string, role: string) => {
    const password = role === 'Admin' ? 'Admin@1234' : role === 'Teacher' ? 'Teacher@1234' : 'Student@1234';
    setValue('email', email, { shouldValidate: true });
    setValue('password', password, { shouldValidate: true });
    setErrorMessage(null);
  };

  return (
    <Card className="w-full max-w-md border-border/60 bg-card/90 shadow-2xl backdrop-blur-xl transition-all">
      <CardHeader className="space-y-2 text-center pb-4">
        <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-2xl bg-primary text-primary-foreground font-black text-xl shadow-lg shadow-primary/30">
          AH
        </div>
        <CardTitle className="text-2xl font-bold tracking-tight">Welcome Back</CardTitle>
        <CardDescription className="text-sm text-muted-foreground">
          Sign in to access your AssignmentHub dashboard
        </CardDescription>
      </CardHeader>

      <CardContent className="space-y-4">
        {errorMessage && (
          <div className="flex items-center gap-3 rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-sm text-destructive font-medium animate-in fade-in slide-in-from-top-1">
            <AlertCircle className="h-4 w-4 shrink-0" />
            <span>{errorMessage}</span>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-1">
            <div className="relative">
              <Mail className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
              <Input
                {...register('email')}
                type="email"
                placeholder="email@assignmentmanagement.com"
                className="pl-9"
                error={errors.email?.message}
                disabled={isLoading}
              />
            </div>
          </div>

          <div className="space-y-1">
            <div className="relative">
              <Lock className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
              <Input
                {...register('password')}
                type="password"
                placeholder="••••••••"
                className="pl-9"
                error={errors.password?.message}
                disabled={isLoading}
              />
            </div>
          </div>

          <Button type="submit" className="w-full font-semibold shadow-md" isLoading={isLoading}>
            Sign In
          </Button>
        </form>

        {/* Demo Account Quick Switcher */}
        <div className="pt-4 border-t border-border/60">
          <p className="mb-2 text-xs font-semibold text-center text-muted-foreground uppercase tracking-wider">
            ⚡ Quick Demo Login Presets
          </p>
          <div className="grid grid-cols-3 gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="text-xs gap-1 border-destructive/30 hover:bg-destructive/10 hover:text-destructive"
              onClick={() => setDemoCredentials('admin@assignmentmanagement.com', 'Admin')}
            >
              <Shield className="h-3 w-3" />
              Admin
            </Button>
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="text-xs gap-1 border-blue-500/30 hover:bg-blue-500/10 hover:text-blue-500"
              onClick={() => setDemoCredentials('john.teacher@assignmentmanagement.com', 'Teacher')}
            >
              <School className="h-3 w-3" />
              Teacher
            </Button>
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="text-xs gap-1 border-emerald-500/30 hover:bg-emerald-500/10 hover:text-emerald-500"
              onClick={() => setDemoCredentials('alex.student@assignmentmanagement.com', 'Student')}
            >
              <GraduationCap className="h-3 w-3" />
              Student
            </Button>
          </div>
        </div>
      </CardContent>

      <CardFooter className="justify-center border-t border-border/40 py-3">
        <p className="text-xs text-muted-foreground">
          Assignment Management System • ASP.NET Core & Next.js
        </p>
      </CardFooter>
    </Card>
  );
}
