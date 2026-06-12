export function PriorityBadge({ priority }) {
  if (!priority) return null;

  const styles = {
    Low: "bg-slate-100 text-slate-700 ring-slate-600/20",
    Medium: "bg-blue-50 text-blue-700 ring-blue-600/20",
    High: "bg-amber-50 text-amber-700 ring-amber-600/20",
    Critical: "bg-red-50 text-red-700 ring-red-600/20"
  };

  const className = styles[priority] || styles.Medium;

  return (
    <span className={`inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset ${className}`}>
      {priority.toUpperCase()}
    </span>
  );
}
