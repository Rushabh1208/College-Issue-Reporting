import { useState } from "react";
import { Link } from "react-router-dom";
import { PlusCircle } from "lucide-react";
import { useStudentIssues } from "../hooks/useStudentIssues";
import { useStudentIssueStats } from "../hooks/useStudentIssueStats";
import { IssueCard } from "../../issues/components/IssueCard";
import { IssueStats } from "../../issues/components/IssueStats";
import { Button } from "../../../shared/ui/Button";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorState } from "../../../shared/ui/ErrorState";
import { SkeletonList } from "../../../shared/ui/Skeleton";

export default function StudentIssuesPage() {
  const [status, setStatus] = useState("");
  const filters = { status: status || undefined };
  const { data: issues = [], error, isLoading, refetch } = useStudentIssues(filters);
  const { data: stats = {} } = useStudentIssueStats();

  if (isLoading) return <SkeletonList />;
  if (error) return <ErrorState message={error.message} onRetry={refetch} />;

  return (
    <div className="grid gap-4">
      <div className="flex items-center justify-between gap-3">
        <div>
          <h2 className="text-xl font-black text-slate-950">Your reports</h2>
          <p className="mt-1 text-sm text-slate-600">Track each issue from submission to resolution.</p>
        </div>
        <Link className="hidden min-h-11 items-center justify-center gap-2 rounded-lg bg-brand-600 px-4 py-2.5 text-sm font-semibold text-white shadow-soft transition hover:bg-brand-700 sm:inline-flex" to="/student/report">
          <PlusCircle className="h-4 w-4" aria-hidden="true" />
          Report issue
        </Link>
      </div>
      <IssueStats counts={stats} activeStatus={status} onStatusClick={setStatus} />
      {issues.length === 0 ? (
        <EmptyState
          title="No issues reported"
          description="Create your first report with a location, description, and optional image."
          action={<Link className="inline-flex min-h-11 items-center justify-center gap-2 rounded-lg bg-brand-600 px-4 py-2.5 text-sm font-semibold text-white shadow-soft transition hover:bg-brand-700" to="/student/report"><PlusCircle className="h-4 w-4" />Report issue</Link>}
        />
      ) : (
        <div className="grid gap-3 md:grid-cols-2">
          {issues.map((issue) => <IssueCard key={issue.id} issue={issue} />)}
        </div>
      )}
      <Link
        to="/student/report"
        className="fixed bottom-24 right-4 z-30 inline-flex min-h-12 items-center justify-center gap-2 rounded-full bg-brand-600 px-5 text-sm font-bold text-white shadow-lift transition hover:bg-brand-700 lg:hidden"
      >
        <PlusCircle className="h-5 w-5" aria-hidden="true" />
        Report
      </Link>
    </div>
  );
}
