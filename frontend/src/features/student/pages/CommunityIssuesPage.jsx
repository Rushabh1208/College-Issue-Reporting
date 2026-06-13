import { useMemo, useState } from "react";
import { Search } from "lucide-react";
import { useCommunityIssues } from "../hooks/useCommunityIssues";
import { useCommunityIssueStats } from "../hooks/useCommunityIssueStats";
import { IssueCard } from "../../issues/components/IssueCard";
import { IssueStats } from "../../issues/components/IssueStats";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorState } from "../../../shared/ui/ErrorState";
import { SkeletonList } from "../../../shared/ui/Skeleton";

export default function CommunityIssuesPage() {
  const [search, setSearch] = useState("");
  const [activeStatus, setActiveStatus] = useState("");

  const filters = useMemo(() => ({
    search: search.trim() || undefined,
    status: activeStatus || undefined
  }), [search, activeStatus]);

  const { data: issues = [], error, isLoading, refetch } = useCommunityIssues(filters);
  const { data: counts = {} } = useCommunityIssueStats();



  if (error) return <ErrorState message={error.message} onRetry={refetch} />;

  return (
    <div className="grid gap-4">
      <div>
        <h2 className="text-xl font-black text-slate-950">Community reports</h2>
        <p className="mt-1 text-sm text-slate-600">Browse public issues from other students. Upvote ones affecting you too.</p>
      </div>
      <IssueStats counts={counts} activeStatus={activeStatus} onStatusClick={setActiveStatus} statuses={["Open", "InProgress"]} />

      <div className="relative">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" aria-hidden="true" />
        <input
          className="min-h-11 w-full rounded-lg border border-slate-200 bg-white pl-9 pr-3.5 py-2.5 text-sm text-slate-900 shadow-sm focus:border-brand-500"
          placeholder="Search by title, description, block, or room..."
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
      </div>

      {isLoading ? (
        <SkeletonList />
      ) : issues.length === 0 ? (
        <EmptyState title="No community issues" description="No issues match your search or filter." />
      ) : (
        <div className="grid gap-3 md:grid-cols-2">
          {issues.map((issue) => <IssueCard key={issue.id} issue={issue} />)}
        </div>
      )}
    </div>
  );
}
