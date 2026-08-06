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
import {
  HiEnvelope,
  HiLockClosed,
  HiExclamationCircle,
  HiShieldCheck,
  HiAcademicCap,
  HiUserGroup,
  HiArrowRight,
  HiBolt,
} from 'react-icons/hi2';
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
    <Card className="w-full max-w-md rounded-2xl border border-border/80 bg-card/85 shadow-2xl shadow-indigo-500/10 backdrop-blur-2xl transition-all">
      <CardHeader className="space-y-2 text-center pb-3 pt-6">
        <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-2xl bg-gradient-to-tr from-indigo-600 via-indigo-500 to-purple-500 text-white font-black text-xl shadow-lg shadow-indigo-500/30">
          AF
        </div>
        <CardTitle className="text-2xl font-extrabold tracking-tight text-foreground">Welcome Back</CardTitle>
        <CardDescription className="text-xs text-muted-foreground font-medium">
          Sign in to access your AcademiaFlow portal
        </CardDescription>
      </CardHeader>

      <CardContent className="space-y-4 px-6">
        {errorMessage && (
          <div className="flex items-center gap-3 rounded-xl border border-destructive/40 bg-destructive/10 p-3 text-xs text-destructive font-medium animate-in fade-in slide-in-from-top-1">
            <HiExclamationCircle className="h-4.5 w-4.5 shrink-0 text-destructive" />
            <span>{errorMessage}</span>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-3.5">
          <div className="space-y-1">
            <div className="relative">
              <HiEnvelope className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
              <Input
                {...register('email')}
                type="email"
                placeholder="email@assignmentmanagement.com"
                className="pl-10 h-10 rounded-xl bg-background/50 text-sm focus:bg-background transition-all"
                error={errors.email?.message}
                disabled={isLoading}
              />
            </div>
          </div>

          <div className="space-y-1">
            <div className="relative">
              <HiLockClosed className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
              <Input
                {...register('password')}
                type="password"
                placeholder="••••••••"
                className="pl-10 h-10 rounded-xl bg-background/50 text-sm focus:bg-background transition-all"
                error={errors.password?.message}
                disabled={isLoading}
              />
            </div>
          </div>

          <Button type="submit" className="w-full h-10 rounded-xl font-bold bg-gradient-to-r from-indigo-600 to-violet-600 hover:from-indigo-500 hover:to-violet-500 text-white shadow-md shadow-indigo-500/25 transition-all gap-2" isLoading={isLoading}>
            <span>Sign In</span>
            <HiArrowRight className="h-4 w-4" />
          </Button>
        </form>

        {/* Demo Account Quick Switcher */}
        <div className="pt-3.5 border-t border-border/60">
          <div className="flex items-center justify-center gap-1.5 mb-2.5 text-[11px] font-bold text-muted-foreground uppercase tracking-widest">
            <HiBolt className="h-3.5 w-3.5 text-amber-500" />
            <span>Quick Demo Login Presets</span>
          </div>
          <div className="grid grid-cols-3 gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="text-xs h-9 rounded-xl font-semibold gap-1.5 border-rose-500/30 hover:bg-rose-500/10 hover:text-rose-500 transition-all"
              onClick={() => setDemoCredentials('admin@assignmentmanagement.com', 'Admin')}
            >
              <HiShieldCheck className="h-4 w-4 text-rose-500" />
              Admin
            </Button>
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="text-xs h-9 rounded-xl font-semibold gap-1.5 border-blue-500/30 hover:bg-blue-500/10 hover:text-blue-500 transition-all"
              onClick={() => setDemoCredentials('john.teacher@assignmentmanagement.com', 'Teacher')}
            >
              <HiAcademicCap className="h-4 w-4 text-blue-500" />
              Teacher
            </Button>
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="text-xs h-9 rounded-xl font-semibold gap-1.5 border-emerald-500/30 hover:bg-emerald-500/10 hover:text-emerald-500 transition-all"
              onClick={() => setDemoCredentials('alex.student@assignmentmanagement.com', 'Student')}
            >
              <HiUserGroup className="h-4 w-4 text-emerald-500" />
              Student
            </Button>
          </div>
        </div>
      </CardContent>

      <CardFooter className="justify-center border-t border-border/40 py-3">
        <p className="text-[11px] font-medium text-muted-foreground">
          AcademiaFlow System • ASP.NET Core & Next.js
        </p>
      </CardFooter>
    </Card>
  );
}
