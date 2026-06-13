import { useState } from "react";
import { CheckCircle2 } from "lucide-react";
import { useStaffIssues } from "../hooks/useStaffIssues";
import { useStaffIssueStats } from "../hooks/useStaffIssueStats";
import { useConfirm } from "../../../shared/hooks/useConfirm";
import { updateIssueStatus } from "../api/staffIssueApi";
import { ISSUE_STATUSES } from "../../../shared/constants/api";
import { useUiStore } from "../../../app/store/uiStore";
import { IssueCard } from "../../issues/components/IssueCard";
import { IssueStats } from "../../issues/components/IssueStats";
import { Button } from "../../../shared/ui/Button";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorState } from "../../../shared/ui/ErrorState";
import { SkeletonList } from "../../../shared/ui/Skeleton";

export default function StaffIssuesPage() {
  const [activeStatus, setActiveStatus] = useState("");
  const { data: issues = [], error, isLoading, refetch } = useStaffIssues({ status: activeStatus || undefined });
  const { data: counts = {} } = useStaffIssueStats();
  const [busyId, setBusyId] = useState(null);
  const pushToast = useUiStore((state) => state.pushToast);

  const confirm = useConfirm();

  async function setStatus(issue, status) {
    if (status === "Resolved") {
      const ok = await confirm({
        title: "Mark as resolved?",
        description: "Are you sure you want to mark this issue as resolved?",
        confirmLabel: "Resolve",
        variant: "primary"
      });
      if (!ok) return;
    }
    setBusyId(issue.id);
    try {
      await updateIssueStatus(issue.id, status);
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
        <h2 className="text-xl font-black text-slate-950">Assigned queue</h2>
        <p className="mt-1 text-sm text-slate-600">Only issues assigned to your account can be updated.</p>
      </div>
      <IssueStats counts={counts} activeStatus={activeStatus} onStatusClick={setActiveStatus} />
      {issues.length === 0 ? (
        <EmptyState title="No assigned issues" description="Assigned campus work will appear here." />
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
