import { Card } from "../../../shared/ui/Card";

import { cn } from "../../../shared/utils/cn";

const STATUS_ORDER = ["Open", "InProgress", "Resolved"];
const LABELS = { Open: "Open", InProgress: "In progress", Resolved: "Resolved" };
const GRID_COLS = { 2: "grid-cols-2", 3: "grid-cols-3" };

export function IssueStats({ counts = {}, activeStatus, onStatusClick, statuses = STATUS_ORDER }) {
  return (
    <div className={`grid ${GRID_COLS[statuses.length] || "grid-cols-3"} gap-2`}>
      {statuses.map((status) => {
        const isActive = activeStatus === status;
        return (
          <button
            key={status}
            type="button"
            onClick={() => onStatusClick?.(isActive ? "" : status)}
            className="text-left"
          >
            <Card className={cn("p-3 transition-colors hover:bg-slate-50", isActive && "bg-brand-50 ring-brand-600 hover:bg-brand-100")}>
              <p className={cn("text-xl font-black", isActive ? "text-brand-900" : "text-slate-950")}>
                {counts[status] ?? 0}
              </p>
              <p className={cn("mt-0.5 text-xs font-semibold", isActive ? "text-brand-700" : "text-slate-500")}>
                {LABELS[status]}
              </p>
            </Card>
          </button>
        );
      })}
    </div>
  );
}
