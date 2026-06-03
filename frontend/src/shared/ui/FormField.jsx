import { cn } from "../utils/cn";

export function FormField({ label, error, hint, children }) {
  return (
    <label className="grid gap-2">
      <span className="text-sm font-semibold text-slate-800">{label}</span>
      {children}
      {hint && !error && <span className="text-xs text-slate-500">{hint}</span>}
      {error && <span className="text-xs font-medium text-red-600">{error}</span>}
    </label>
  );
}

export function inputClass(error) {
  return cn(
    "min-h-11 w-full rounded-lg border bg-white px-3.5 py-2.5 text-sm text-slate-900 shadow-sm transition placeholder:text-slate-400",
    error ? "border-red-300 focus:border-red-400" : "border-slate-200 focus:border-brand-500"
  );
}
