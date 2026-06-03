import { AlertCircle, CheckCircle2, Info, X } from "lucide-react";
import { useUiStore } from "../../app/store/uiStore";
import { cn } from "../utils/cn";

const icons = {
  success: CheckCircle2,
  error: AlertCircle,
  info: Info
};

export function ToastHost() {
  const { toasts, removeToast } = useUiStore();
  return (
    <div className="fixed inset-x-3 top-3 z-50 grid gap-2 sm:left-auto sm:right-4 sm:w-96">
      {toasts.map((toast) => {
        const Icon = icons[toast.type] || Info;
        return (
          <div
            key={toast.id}
            className={cn(
              "flex items-start gap-3 rounded-lg border bg-white p-3 shadow-lift",
              toast.type === "error" && "border-red-100",
              toast.type === "success" && "border-green-100"
            )}
          >
            <Icon className={cn("mt-0.5 h-5 w-5 shrink-0", toast.type === "error" ? "text-red-600" : toast.type === "success" ? "text-green-600" : "text-brand-600")} />
            <div className="min-w-0 flex-1">
              <p className="text-sm font-bold text-slate-950">{toast.title}</p>
              {toast.message && <p className="mt-0.5 text-sm text-slate-600">{toast.message}</p>}
            </div>
            <button className="rounded-md p-1 text-slate-400 hover:bg-slate-100 hover:text-slate-700" onClick={() => removeToast(toast.id)} aria-label="Dismiss notification">
              <X className="h-4 w-4" />
            </button>
          </div>
        );
      })}
    </div>
  );
}
