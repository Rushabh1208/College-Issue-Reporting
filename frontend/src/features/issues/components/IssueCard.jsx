import { useState } from "react";
import { MapPin, CalendarClock, Image as ImageIcon, UserRound, ArrowUp, Tag, History } from "lucide-react";
import { Card } from "../../../shared/ui/Card";
import { StatusBadge } from "../../../shared/ui/StatusBadge";
import { PriorityBadge } from "../../../shared/ui/PriorityBadge";
import { compactFileSize, formatDateTime } from "../../../shared/utils/formatters";
import { upvoteIssue, removeUpvote } from "../../student/api/studentIssueApi";
import { useUiStore } from "../../../app/store/uiStore";
import { IssueTimeline } from "../../../shared/components/IssueTimeline";

export function IssueCard({ issue: initialIssue, actions }) {
  const [issue, setIssue] = useState(initialIssue);
  const [showTimeline, setShowTimeline] = useState(false);
  const pushToast = useUiStore((state) => state.pushToast);

  const handleUpvoteToggle = async () => {
    const isUpvoted = issue.hasUpvoted;
    
    // Optimistic update
    setIssue((prev) => ({
      ...prev,
      hasUpvoted: !isUpvoted,
      upvoteCount: isUpvoted ? Math.max(0, prev.upvoteCount - 1) : prev.upvoteCount + 1,
    }));

    try {
      if (isUpvoted) {
        await removeUpvote(issue.id);
      } else {
        await upvoteIssue(issue.id);
      }
    } catch (error) {
      // Revert on failure
      setIssue((prev) => ({
        ...prev,
        hasUpvoted: isUpvoted,
        upvoteCount: isUpvoted ? prev.upvoteCount + 1 : Math.max(0, prev.upvoteCount - 1),
      }));
      pushToast({ type: "error", title: "Error", message: "Failed to update upvote status." });
    }
  };

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
            <span>Assigned: {issue.assignedStaffName || "Unassigned"}</span>
          </div>
          {issue.reporterName && (
            <div className="flex items-center gap-2">
              <UserRound className="h-4 w-4 text-slate-400" aria-hidden="true" />
              <span>
                Reporter: {issue.reporterName}
                {issue.reporterStudentId && ` (Student ID: ${issue.reporterStudentId})`}
              </span>
            </div>
          )}
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
        
        <div className="mt-4 flex flex-wrap items-center justify-between gap-2 border-t border-slate-100 pt-4">
          <div className="flex items-center gap-2">
            {issue.hasUpvoted !== undefined ? (
              <button
                onClick={handleUpvoteToggle}
                className={`inline-flex items-center gap-1.5 rounded-md px-3 py-1.5 text-sm font-medium transition-colors ${
                  issue.hasUpvoted
                    ? "bg-brand-50 text-brand-700 hover:bg-brand-100"
                    : "bg-white text-slate-600 ring-1 ring-inset ring-slate-300 hover:bg-slate-50"
                }`}
              >
                <ArrowUp className="h-4 w-4" />
                <span className="font-semibold">{issue.upvoteCount}</span>
                <span>{issue.hasUpvoted ? "Upvoted" : "Upvote"}</span>
              </button>
            ) : (
              issue.upvoteCount !== undefined && (
                <span className="inline-flex items-center gap-1 rounded-md bg-slate-50 px-2 py-1 text-xs font-medium text-slate-600 ring-1 ring-inset ring-slate-500/10">
                  <ArrowUp className="h-3 w-3" />
                  {issue.upvoteCount}
                </span>
              )
            )}
            <button
              onClick={() => setShowTimeline(!showTimeline)}
              className="inline-flex items-center gap-1.5 rounded-md px-3 py-1.5 text-sm font-medium transition-colors bg-white text-slate-600 ring-1 ring-inset ring-slate-300 hover:bg-slate-50"
            >
              <History className="h-4 w-4" />
              <span>{showTimeline ? "Hide Timeline" : "View Timeline"}</span>
            </button>
          </div>
          {actions && <div className="flex flex-wrap gap-2">{actions}</div>}
        </div>

        {showTimeline && (
          <div className="mt-4 border-t border-slate-100 pt-4">
            <IssueTimeline issueId={issue.id} />
          </div>
        )}
      </div>
    </Card>
  );
}
