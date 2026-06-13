import { useState } from "react";
import { CheckCircle2 } from "lucide-react";
import { useWomenCellIssues } from "../hooks/useWomenCellIssues";
import { useWomenCellIssueStats } from "../hooks/useWomenCellIssueStats";
import { useConfirm } from "../../../shared/hooks/useConfirm";
import { updateWomenCellIssueStatus } from "../api/womencellApi";
import { ISSUE_STATUSES } from "../../../shared/constants/api";
import { useUiStore } from "../../../app/store/uiStore";
import { IssueCard } from "../../issues/components/IssueCard";
import { IssueStats } from "../../issues/components/IssueStats";
import { Button } from "../../../shared/ui/Button";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorState } from "../../../shared/ui/ErrorState";
import { SkeletonList } from "../../../shared/ui/Skeleton";

export default function WomenCellIssuesPage() {
  const [filterStatus, setFilterStatus] = useState("");
  const filters = { status: filterStatus || undefined };
  const { data: issues = [], error, isLoading, refetch } = useWomenCellIssues(filters);
  const { data: stats = {} } = useWomenCellIssueStats();
  const [busyId, setBusyId] = useState(null);
  const pushToast = useUiStore((state) => state.pushToast);

  const confirm = useConfirm();

  async function setStatus(issue, status) {
    if (status === "Resolved") {
      const ok = await confirm({
        title: "Mark as resolved?",
        description: "Are you sure you want to mark this sensitive issue as resolved?",
        confirmLabel: "Resolve",
        variant: "primary"
      });
      if (!ok) return;
    }
    setBusyId(issue.id);
    try {
      await updateWomenCellIssueStatus(issue.id, status);
      pushToast({ type: "success", title: "Status updated", message: `${issue.title} is now ${status}.` });
      await refetch();
    } catch (err) {
      pushToast({ type: "error", title: "Update failed", message: err.message });
    } finally {
      setBusyId(null);
    }
  }

  if (isLoading) return <SkeletonList />;
  if (error) return <ErrorState message={error.message} onRetry={refetch} />;

  return (
    <div className="grid gap-4">
      <div>
        <h2 className="text-xl font-black text-slate-950">Women Welfare Queue</h2>
        <p className="mt-1 text-sm text-slate-600">Secure queue for issues reported under the Women Welfare category.</p>
      </div>
      <IssueStats counts={stats} activeStatus={filterStatus} onStatusClick={setFilterStatus} />
      {issues.length === 0 ? (
        <EmptyState title="No active issues" description="There are currently no Women Welfare issues reported." />
      ) : (
        <div className="grid gap-3 md:grid-cols-2">
          {issues.map((issue) => (
            <IssueCard
              key={issue.id}
              issue={issue}
              actions={ISSUE_STATUSES.map((status) => (
                <Button
                  key={status}
                  variant={issue.status === status ? "primary" : "secondary"}
                  disabled={issue.status === status || busyId === issue.id || (issue.status === "Resolved" && status === "Open")}
                  isLoading={busyId === issue.id && issue.status !== status}
                  onClick={() => setStatus(issue, status)}
                >
                  {status === "Resolved" && <CheckCircle2 className="h-4 w-4" aria-hidden="true" />}
                  {status === "InProgress" ? "In progress" : status}
                </Button>
              ))}
            />
          ))}
        </div>
      )}
    </div>
  );
}
