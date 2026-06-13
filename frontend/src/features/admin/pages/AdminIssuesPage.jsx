import { useMemo, useState } from "react";
import { Filter } from "lucide-react";
import { assignIssue, deleteIssue } from "../api/adminIssueApi";
import { useAdminIssues } from "../hooks/useAdminIssues";
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
  const filters = useMemo(() => ({ status, page, pageSize: PAGE_SIZE }), [status, page]);
  const { data: issues = [], error, isLoading, refetch } = useAdminIssues(filters);
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

  async function handleDelete(issue) {
    if (!window.confirm(`Delete issue #${issue.id}? This hides it from all lists.`)) return;
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
      <IssueStats issues={issues} />
      <Card className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-2 text-sm font-bold text-slate-700">
          <Filter className="h-4 w-4 text-slate-400" aria-hidden="true" />
          Status filter
        </div>
        <select
          className="min-h-11 rounded-lg border border-slate-200 bg-white px-5 text-sm font-semibold text-slate-700"
          value={status}
          onChange={(event) => changeStatus(event.target.value)}
        >
          <option value="">All statuses</option>
          {ISSUE_STATUSES.map((item) => (
            <option key={item} value={item}>{item === "InProgress" ? "In progress" : item}</option>
          ))}
        </select>
      </Card>

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
