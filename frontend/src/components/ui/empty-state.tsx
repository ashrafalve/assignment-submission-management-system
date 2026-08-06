import React from 'react';
import { FolderOpen } from 'lucide-react';
import { Button } from './button';

export interface EmptyStateProps {
  icon?: React.ElementType;
  title: string;
  description: string;
  actionLabel?: string;
  onAction?: () => void;
}

export function EmptyState({
  icon: Icon = FolderOpen,
  title,
  description,
  actionLabel,
  onAction,
}: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center p-8 text-center rounded-2xl border border-dashed border-border/70 bg-card/40 my-4">
      <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-primary/10 text-primary mb-4 shadow-inner">
        <Icon className="h-7 w-7" />
      </div>
      <h3 className="text-lg font-bold tracking-tight text-foreground">{title}</h3>
      <p className="mt-1 text-sm text-muted-foreground max-w-sm">{description}</p>
      {actionLabel && onAction && (
        <Button onClick={onAction} className="mt-5 font-semibold shadow-md">
          {actionLabel}
        </Button>
      )}
    </div>
  );
}
