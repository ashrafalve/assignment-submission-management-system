'use client';

import React, { createContext, useContext, useState, useCallback } from 'react';
import { CheckCircle2, AlertCircle, Info, AlertTriangle, X } from 'lucide-react';
import { cn } from '@/lib/utils';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastItem {
  id: string;
  title?: string;
  message: string;
  type: ToastType;
}

interface ToastContextType {
  toast: (message: string, type?: ToastType, title?: string) => void;
  success: (message: string, title?: string) => void;
  error: (message: string, title?: string) => void;
  warning: (message: string, title?: string) => void;
  info: (message: string, title?: string) => void;
}

const ToastContext = createContext<ToastContextType | undefined>(undefined);

export function ToastProvider({ children }: { children: React.ReactNode }) {
  const [toasts, setToasts] = useState<ToastItem[]>([]);

  const removeToast = useCallback((id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  const addToast = useCallback((message: string, type: ToastType = 'info', title?: string) => {
    const id = Math.random().toString(36).substring(2, 9);
    setToasts((prev) => [...prev, { id, message, type, title }]);

    setTimeout(() => {
      removeToast(id);
    }, 4000);
  }, [removeToast]);

  const success = useCallback((message: string, title?: string) => addToast(message, 'success', title), [addToast]);
  const error = useCallback((message: string, title?: string) => addToast(message, 'error', title), [addToast]);
  const warning = useCallback((message: string, title?: string) => addToast(message, 'warning', title), [addToast]);
  const info = useCallback((message: string, title?: string) => addToast(message, 'info', title), [addToast]);

  return (
    <ToastContext.Provider value={{ toast: addToast, success, error, warning, info }}>
      {children}
      <div className="fixed bottom-4 right-4 z-50 flex flex-col gap-2 max-w-sm w-full pointer-events-none px-4">
        {toasts.map((t) => (
          <div
            key={t.id}
            className={cn(
              'pointer-events-auto flex items-start gap-3 rounded-xl border p-4 shadow-xl backdrop-blur-md transition-all animate-in slide-in-from-right-5 fade-in duration-200',
              t.type === 'success' && 'bg-emerald-950/90 text-emerald-100 border-emerald-500/40',
              t.type === 'error' && 'bg-rose-950/90 text-rose-100 border-rose-500/40',
              t.type === 'warning' && 'bg-amber-950/90 text-amber-100 border-amber-500/40',
              t.type === 'info' && 'bg-slate-900/90 text-slate-100 border-slate-700/50'
            )}
          >
            {t.type === 'success' && <CheckCircle2 className="h-5 w-5 shrink-0 text-emerald-400 mt-0.5" />}
            {t.type === 'error' && <AlertCircle className="h-5 w-5 shrink-0 text-rose-400 mt-0.5" />}
            {t.type === 'warning' && <AlertTriangle className="h-5 w-5 shrink-0 text-amber-400 mt-0.5" />}
            {t.type === 'info' && <Info className="h-5 w-5 shrink-0 text-blue-400 mt-0.5" />}

            <div className="flex-1 text-sm">
              {t.title && <h4 className="font-semibold leading-tight">{t.title}</h4>}
              <p className={cn('text-xs opacity-90', t.title && 'mt-1')}>{t.message}</p>
            </div>

            <button
              onClick={() => removeToast(t.id)}
              className="opacity-70 hover:opacity-100 transition-opacity p-0.5"
            >
              <X className="h-4 w-4" />
            </button>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast() {
  const context = useContext(ToastContext);
  if (!context) {
    throw new Error('useToast must be used within a ToastProvider');
  }
  return context;
}
