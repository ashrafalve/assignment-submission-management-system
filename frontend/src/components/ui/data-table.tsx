'use client';

import React, { useState, useEffect } from 'react';
import { Search, ArrowUpDown, ArrowUp, ArrowDown } from 'lucide-react';
import { Input } from './input';
import { TableSkeleton } from './skeleton';
import { EmptyState } from './empty-state';
import { Pagination, PaginationProps } from './pagination';
import { useDebounce } from '@/hooks/useDebounce';
import { cn } from '@/lib/utils';

export interface Column<T> {
  key: string;
  header: string;
  cell?: (row: T) => React.ReactNode;
  sortable?: boolean;
  className?: string;
}

export interface DataTableProps<T> {
  columns: Column<T>[];
  data: T[];
  isLoading?: boolean;
  keyExtractor: (row: T) => string;
  pagination?: PaginationProps;
  onSearch?: (term: string) => void;
  onSort?: (columnKey: string, descending: boolean) => void;
  searchPlaceholder?: string;
  emptyTitle?: string;
  emptyDescription?: string;
  emptyActionLabel?: string;
  onEmptyAction?: () => void;
}

export function DataTable<T>({
  columns,
  data,
  isLoading = false,
  keyExtractor,
  pagination,
  onSearch,
  onSort,
  searchPlaceholder = 'Search records...',
  emptyTitle = 'No records found',
  emptyDescription = 'There are no items to display matching your criteria.',
  emptyActionLabel,
  onEmptyAction,
}: DataTableProps<T>) {
  const [searchTerm, setSearchTerm] = useState('');
  const debouncedSearchTerm = useDebounce(searchTerm, 350);

  const [sortKey, setSortKey] = useState<string | null>(null);
  const [sortDesc, setSortDesc] = useState(false);

  useEffect(() => {
    if (onSearch) {
      onSearch(debouncedSearchTerm);
    }
  }, [debouncedSearchTerm, onSearch]);

  const handleSortClick = (key: string) => {
    const isDesc = sortKey === key ? !sortDesc : false;
    setSortKey(key);
    setSortDesc(isDesc);
    if (onSort) onSort(key, isDesc);
  };

  return (
    <div className="w-full space-y-4">
      {/* Top Bar: Search Input */}
      {onSearch && (
        <div className="flex items-center justify-between gap-4">
          <div className="relative w-full max-w-sm">
            <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <Input
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder={searchPlaceholder}
              className="pl-9 h-10 shadow-sm"
            />
          </div>
        </div>
      )}

      {/* Table Container */}
      <div className="rounded-xl border border-border bg-card overflow-hidden shadow-sm transition-all">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-muted/50 text-xs uppercase tracking-wider text-muted-foreground border-b border-border">
              <tr>
                {columns.map((col) => (
                  <th
                    key={col.key}
                    className={cn('px-4 py-3.5 font-semibold select-none', col.className)}
                  >
                    {col.sortable ? (
                      <button
                        onClick={() => handleSortClick(col.key)}
                        className="flex items-center gap-1.5 hover:text-foreground transition-colors focus:outline-none"
                      >
                        {col.header}
                        {sortKey === col.key ? (
                          sortDesc ? (
                            <ArrowDown className="h-3.5 w-3.5 text-primary" />
                          ) : (
                            <ArrowUp className="h-3.5 w-3.5 text-primary" />
                          )
                        ) : (
                          <ArrowUpDown className="h-3.5 w-3.5 opacity-40" />
                        )}
                      </button>
                    ) : (
                      col.header
                    )}
                  </th>
                ))}
              </tr>
            </thead>

            <tbody className="divide-y divide-border/60">
              {isLoading ? (
                <tr>
                  <td colSpan={columns.length} className="p-4">
                    <TableSkeleton rows={5} />
                  </td>
                </tr>
              ) : data.length === 0 ? (
                <tr>
                  <td colSpan={columns.length} className="p-4">
                    <EmptyState
                      title={emptyTitle}
                      description={emptyDescription}
                      actionLabel={emptyActionLabel}
                      onAction={onEmptyAction}
                    />
                  </td>
                </tr>
              ) : (
                data.map((row) => (
                  <tr
                    key={keyExtractor(row)}
                    className="hover:bg-accent/40 transition-colors group"
                  >
                    {columns.map((col) => (
                      <td key={col.key} className={cn('px-4 py-3 text-foreground', col.className)}>
                        {col.cell ? col.cell(row) : (row as any)[col.key]}
                      </td>
                    ))}
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination Footer */}
        {pagination && <Pagination {...pagination} />}
      </div>
    </div>
  );
}
