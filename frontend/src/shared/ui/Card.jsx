import { cn } from "../utils/cn";

export function Card({ children, className }) {
  return <section className={cn("rounded-lg border border-slate-200 bg-white p-4 shadow-soft", className)}>{children}</section>;
}
