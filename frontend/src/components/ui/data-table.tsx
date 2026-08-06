'use client';

import React, { useState, useEffect } from 'react';
import { HiMagnifyingGlass, HiChevronUpDown, HiChevronUp, HiChevronDown } from 'react-icons/hi2';
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
            <HiMagnifyingGlass className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
            <Input
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder={searchPlaceholder}
              className="pl-10 h-10 rounded-xl bg-card/60 border-border/80 text-sm shadow-xs focus:bg-card transition-all"
            />
          </div>
        </div>
      )}

      {/* Table Container */}
      <div className="rounded-2xl border border-border/80 bg-card/80 backdrop-blur-md overflow-hidden shadow-sm transition-all">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-accent/50 text-[11px] font-bold uppercase tracking-wider text-muted-foreground border-b border-border/70">
              <tr>
                {columns.map((col) => (
                  <th
                    key={col.key}
                    className={cn('px-4 py-3.5 font-bold select-none', col.className)}
                  >
                    {col.sortable ? (
                      <button
                        onClick={() => handleSortClick(col.key)}
                        className="flex items-center gap-1.5 hover:text-foreground transition-colors focus:outline-none"
                      >
                        {col.header}
                        {sortKey === col.key ? (
                          sortDesc ? (
                            <HiChevronDown className="h-4 w-4 text-indigo-500 font-bold" />
                          ) : (
                            <HiChevronUp className="h-4 w-4 text-indigo-500 font-bold" />
                          )
                        ) : (
                          <HiChevronUpDown className="h-4 w-4 opacity-40" />
                        )}
                      </button>
                    ) : (
                      col.header
                    )}
                  </th>
                ))}
              </tr>
            </thead>

            <tbody className="divide-y divide-border/50">
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
                    className="hover:bg-accent/50 transition-colors group"
                  >
                    {columns.map((col) => (
                      <td key={col.key} className={cn('px-4 py-3.5 text-foreground font-medium', col.className)}>
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
