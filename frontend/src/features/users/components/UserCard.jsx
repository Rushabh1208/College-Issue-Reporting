import { Shield, UserRound } from "lucide-react";
import { Card } from "../../../shared/ui/Card";
import { cn } from "../../../shared/utils/cn";

const roleStyles = {
  Admin: "bg-red-50 text-red-700 ring-red-200",
  Staff: "bg-blue-50 text-blue-700 ring-blue-200",
  Student: "bg-green-50 text-green-700 ring-green-200"
};

export function UserCard({ user }) {
  return (
    <Card>
      <div className="flex items-start gap-3">
        <div className="grid h-11 w-11 shrink-0 place-items-center rounded-lg bg-slate-100 text-slate-500">
          <UserRound className="h-5 w-5" aria-hidden="true" />
        </div>
        <div className="min-w-0 flex-1">
          <h3 className="break-words font-bold text-slate-950">{user.name}</h3>
          <p className="mt-1 break-words text-sm text-slate-600">{user.email}</p>
        </div>
        <span className={cn("inline-flex items-center gap-1 rounded-full px-2.5 py-1 text-xs font-bold ring-1", roleStyles[user.role] || roleStyles.Student)}>
          <Shield className="h-3.5 w-3.5" aria-hidden="true" />
          {user.role}
        </span>
      </div>
    </Card>
  );
}
