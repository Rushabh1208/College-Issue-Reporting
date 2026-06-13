import { useMemo, useState } from "react";
import { Filter } from "lucide-react";
import { assignIssue, deleteIssue } from "../api/adminIssueApi";
import { useAdminIssues } from "../hooks/useAdminIssues";
import { useAdminIssueStats } from "../hooks/useAdminIssueStats";
import { useConfirm } from "../../../shared/hooks/useConfirm";
import { useUsers } from "../../users/hooks/useUsers";
import { AdminIssueActions } from "../components/AdminIssueActions";
import { IssueCard } from "../../issues/components/IssueCard";
import { IssueStats } from "../../issues/components/IssueStats";
import { ISSUE_STATUSES } from "../../../shared/constants/api";
import { useUiStore } from "../../../app/store/uiStore";
import { Card } from "../../../shared/ui/Card";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorState } from "../../../shared/ui/ErrorState";
import { Pagination } from "../../../shared/ui/Pagination";
import { SkeletonList } from "../../../shared/ui/Skeleton";

const PAGE_SIZE = 10;

export default function AdminIssuesPage() {
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [busyId, setBusyId] = useState(null);
  const pushToast = useUiStore((state) => state.pushToast);
  const filters = useMemo(() => ({ status: status || undefined, page, pageSize: PAGE_SIZE }), [status, page]);
  const { data: issues = [], error, isLoading, refetch } = useAdminIssues(filters);
  const { data: stats = {} } = useAdminIssueStats();
  const { data: users = [] } = useUsers({ page: 1, pageSize: 100 });
  const staffUsers = users.filter((user) => user.role === "Staff");

  function changeStatus(nextStatus) {
    setStatus(nextStatus);
    setPage(1);
  }

  async function handleAssign(issue, staffId) {
    setBusyId(issue.id);
    try {
      await assignIssue(issue.id, staffId);
      pushToast({ type: "success", title: "Issue assigned", message: "The issue moved to In progress." });
      await refetch();
    } catch (error) {
      pushToast({ type: "error", title: "Assignment failed", message: error.message });
    } finally {
      setBusyId(null);
    }
  }

  const confirm = useConfirm();

  async function handleDelete(issue) {
    const ok = await confirm({
      title: "Delete issue?",
      description: `Delete issue #${issue.id}? This hides it from all lists.`,
      confirmLabel: "Delete",
      variant: "danger"
    });
    if (!ok) return;
    setBusyId(issue.id);
    try {
      await deleteIssue(issue.id);
      pushToast({ type: "success", title: "Issue deleted", message: "The issue was removed from active lists." });
      await refetch();
    } catch (error) {
      pushToast({ type: "error", title: "Delete failed", message: error.message });
    } finally {
      setBusyId(null);
    }
  }

  if (isLoading) return <SkeletonList rows={5} />;
  if (error) return <ErrorState message={error.message} onRetry={refetch} />;

  return (
    <div className="grid gap-4">
      <div>
        <h2 className="text-xl font-black text-slate-950">Issue workflow</h2>
        <p className="mt-1 text-sm text-slate-600">Filter, assign, and remove campus reports.</p>
      </div>
      <IssueStats counts={stats} activeStatus={status} onStatusClick={changeStatus} />

      {issues.length === 0 ? (
        <EmptyState title="No issues found" description="Try a different status filter or previous page." />
      ) : (
        <div className="grid gap-3 xl:grid-cols-2">
          {issues.map((issue) => (
            <IssueCard
              key={issue.id}
              issue={issue}
              actions={
                <AdminIssueActions
                  issue={issue}
                  staffUsers={staffUsers}
                  busy={busyId === issue.id}
                  onAssign={handleAssign}
                  onDelete={handleDelete}
                />
              }
            />
          ))}
        </div>
      )}
      <Pagination
        page={page}
        pageSize={PAGE_SIZE}
        itemCount={issues.length}
        isLoading={isLoading}
        onPrev={() => setPage((value) => Math.max(1, value - 1))}
        onNext={() => setPage((value) => value + 1)}
      />
    </div>
  );
}
