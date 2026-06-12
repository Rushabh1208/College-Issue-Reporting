import { MapPin, CalendarClock, Image as ImageIcon, UserRound, ArrowUp, Tag } from "lucide-react";
import { Card } from "../../../shared/ui/Card";
import { StatusBadge } from "../../../shared/ui/StatusBadge";
import { PriorityBadge } from "../../../shared/ui/PriorityBadge";
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
            <div className="flex flex-wrap items-center gap-2 mb-2">
              <StatusBadge status={issue.status} />
              <PriorityBadge priority={issue.priority} />
              {issue.upvoteCount !== undefined && (
                <span className="inline-flex items-center gap-1 rounded-md bg-slate-50 px-2 py-1 text-xs font-medium text-slate-600 ring-1 ring-inset ring-slate-500/10">
                  <ArrowUp className="h-3 w-3" />
                  {issue.upvoteCount}
                </span>
              )}
            </div>
            <h3 className="break-words text-base font-bold text-slate-950">{issue.title}</h3>
            <p className="mt-1 line-clamp-3 text-sm leading-6 text-slate-600">{issue.description}</p>
          </div>
        </div>
        <dl className="mt-4 grid gap-2 text-sm text-slate-600">
          {issue.categoryName && (
            <div className="flex items-center gap-2">
              <Tag className="h-4 w-4 text-slate-400" aria-hidden="true" />
              <span>Category: {issue.categoryName}</span>
            </div>
          )}
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
