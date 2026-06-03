import { useState } from "react";
import { useUsers } from "../hooks/useUsers";
import { UserCard } from "../components/UserCard";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { ErrorState } from "../../../shared/ui/ErrorState";
import { Pagination } from "../../../shared/ui/Pagination";
import { SkeletonList } from "../../../shared/ui/Skeleton";

const PAGE_SIZE = 10;

export default function AdminUsersPage() {
  const [page, setPage] = useState(1);
  const { data: users = [], error, isLoading, refetch } = useUsers({ page, pageSize: PAGE_SIZE });

  if (isLoading) return <SkeletonList rows={5} />;
  if (error) return <ErrorState message={error.message} onRetry={refetch} />;

  return (
    <div className="grid gap-4">
      <div>
        <h2 className="text-xl font-black text-slate-950">Users</h2>
        <p className="mt-1 text-sm text-slate-600">Admin-visible user records from the backend.</p>
      </div>
      {users.length === 0 ? (
        <EmptyState title="No users found" description="There are no users on this page." />
      ) : (
        <div className="grid gap-3 md:grid-cols-2">
          {users.map((user) => <UserCard key={user.id} user={user} />)}
        </div>
      )}
      <Pagination
        page={page}
        pageSize={PAGE_SIZE}
        itemCount={users.length}
        isLoading={isLoading}
        onPrev={() => setPage((value) => Math.max(1, value - 1))}
        onNext={() => setPage((value) => value + 1)}
      />
    </div>
  );
}
