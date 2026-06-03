import { CheckCircle2, Clock3, CircleDot } from "lucide-react";
import { cn } from "../utils/cn";

const styles = {
  Open: "bg-amber-50 text-amber-700 ring-amber-200",
  InProgress: "bg-blue-50 text-blue-700 ring-blue-200",
  Resolved: "bg-green-50 text-green-700 ring-green-200"
};

const icons = {
  Open: CircleDot,
  InProgress: Clock3,
  Resolved: CheckCircle2
};

export function StatusBadge({ status }) {
  const Icon = icons[status] || CircleDot;
  return (
    <span className={cn("inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-semibold ring-1", styles[status] || styles.Open)}>
      <Icon className="h-3.5 w-3.5" aria-hidden="true" />
      {status === "InProgress" ? "In progress" : status}
    </span>
  );
}
