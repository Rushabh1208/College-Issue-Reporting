import { Trash2, UserCheck } from "lucide-react";
import { useState } from "react";
import { Button } from "../../../shared/ui/Button";

export function AdminIssueActions({ issue, staffUsers, onAssign, onDelete, busy }) {
  const [staffId, setStaffId] = useState("");

  return (
    <div className="grid w-full gap-2">
      <div className="flex gap-2">
        <select
          className="min-h-11 min-w-0 flex-1 rounded-lg border border-slate-200 bg-white px-3 text-sm font-medium text-slate-700"
          value={staffId}
          onChange={(event) => setStaffId(event.target.value)}
          aria-label={`Assign staff for ${issue.title}`}
        >
          <option value="">Select staff</option>
          {staffUsers.map((user) => (
            <option key={user.id} value={user.id}>{user.name} - {user.email}</option>
          ))}
        </select>
        <Button disabled={!staffId || busy} isLoading={busy} onClick={() => onAssign(issue, staffId)}>
          <UserCheck className="h-4 w-4" aria-hidden="true" />
          Assign
        </Button>
      </div>
      <Button variant="danger" disabled={busy} onClick={() => onDelete(issue)}>
        <Trash2 className="h-4 w-4" aria-hidden="true" />
        Delete issue
      </Button>
    </div>
  );
}
