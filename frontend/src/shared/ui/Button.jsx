import { Loader2 } from "lucide-react";
import { cn } from "../utils/cn";

const variants = {
  primary: "bg-brand-600 text-white shadow-soft hover:bg-brand-700",
  secondary: "bg-white text-slate-800 border border-slate-200 hover:bg-slate-50",
  ghost: "text-slate-700 hover:bg-slate-100",
  danger: "bg-red-600 text-white shadow-soft hover:bg-red-700"
};

export function Button({ children, className, variant = "primary", isLoading, disabled, type = "button", ...props }) {
  return (
    <button
      type={type}
      disabled={disabled || isLoading}
      className={cn(
        "inline-flex min-h-11 items-center justify-center gap-2 rounded-lg px-4 py-2.5 text-sm font-semibold transition disabled:cursor-not-allowed disabled:opacity-60",
        variants[variant],
        className
      )}
      {...props}
    >
      {isLoading && <Loader2 className="h-4 w-4 animate-spin" aria-hidden="true" />}
      {children}
    </button>
  );
}
