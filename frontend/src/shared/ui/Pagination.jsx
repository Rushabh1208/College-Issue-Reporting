import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "./Button";

export function Pagination({ page, pageSize, itemCount, onPrev, onNext, isLoading }) {
  const canPrev = page > 1;
  const canNext = itemCount === pageSize;
  return (
    <div className="flex items-center justify-between gap-3 rounded-lg border border-slate-200 bg-white p-2 shadow-sm">
      <Button variant="ghost" disabled={!canPrev || isLoading} onClick={onPrev}>
        <ChevronLeft className="h-4 w-4" aria-hidden="true" />
        Prev
      </Button>
      <span className="text-sm font-semibold text-slate-600">Page {page}</span>
      <Button variant="ghost" disabled={!canNext || isLoading} onClick={onNext}>
        Next
        <ChevronRight className="h-4 w-4" aria-hidden="true" />
      </Button>
    </div>
  );
}
