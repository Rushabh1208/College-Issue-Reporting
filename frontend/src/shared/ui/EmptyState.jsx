import { Inbox } from "lucide-react";
import { Card } from "./Card";

export function EmptyState({ title, description, action }) {
  return (
    <Card className="grid place-items-center px-5 py-10 text-center">
      <div className="grid h-12 w-12 place-items-center rounded-full bg-slate-100 text-slate-500">
        <Inbox className="h-6 w-6" aria-hidden="true" />
      </div>
      <h2 className="mt-4 text-base font-bold text-slate-950">{title}</h2>
      {description && <p className="mt-1 max-w-sm text-sm leading-6 text-slate-600">{description}</p>}
      {action && <div className="mt-5">{action}</div>}
    </Card>
  );
}
