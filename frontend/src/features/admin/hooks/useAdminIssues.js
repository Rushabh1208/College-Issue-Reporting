import { useAsync } from "../../../shared/hooks/useAsync";
import { getAdminIssues } from "../api/adminIssueApi";

export function useAdminIssues(filters) {
  return useAsync(() => getAdminIssues(filters), [filters.status, filters.page, filters.pageSize], { initialData: [] });
}
