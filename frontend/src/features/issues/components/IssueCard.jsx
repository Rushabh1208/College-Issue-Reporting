import { MapPin, CalendarClock, Image as ImageIcon, UserRound } from "lucide-react";
import { Card } from "../../../shared/ui/Card";
import { StatusBadge } from "../../../shared/ui/StatusBadge";
import { compactFileSize, formatDateTime } from "../../../shared/utils/formatters";

export function IssueCard({ issue, actions }) {
  return (
    <Card className="overflow-hidden p-0">
      {issue.imageUrl && (
        <img src={issue.imageUrl} alt="" className="h-44 w-full object-cover" loading="lazy" />
      )}
      <div className="p-4">
        <div className="flex items-start justify-between gap-3">
          <div className="min-w-0">
            <h3 className="break-words text-base font-bold text-slate-950">{issue.title}</h3>
            <p className="mt-1 line-clamp-3 text-sm leading-6 text-slate-600">{issue.description}</p>
          </div>
          <StatusBadge status={issue.status} />
        </div>
        <dl className="mt-4 grid gap-2 text-sm text-slate-600">
          <div className="flex items-center gap-2">
            <MapPin className="h-4 w-4 text-slate-400" aria-hidden="true" />
            <span>Block {issue.block}, Room {issue.roomNumber}</span>
          </div>
          <div className="flex items-center gap-2">
            <UserRound className="h-4 w-4 text-slate-400" aria-hidden="true" />
            <span>{issue.assignedStaffName || "Unassigned"}</span>
          </div>
          <div className="flex items-center gap-2">
            <CalendarClock className="h-4 w-4 text-slate-400" aria-hidden="true" />
            <span>{formatDateTime(issue.createdAt)}</span>
          </div>
          {issue.imageSizeBytes && (
            <div className="flex items-center gap-2">
              <ImageIcon className="h-4 w-4 text-slate-400" aria-hidden="true" />
              <span>{issue.imageMimeType || "Image"} {compactFileSize(issue.imageSizeBytes)}</span>
            </div>
          )}
        </dl>
        {actions && <div className="mt-4 flex flex-wrap gap-2">{actions}</div>}
      </div>
    </Card>
  );
}
