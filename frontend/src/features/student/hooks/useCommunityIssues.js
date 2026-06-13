import { useAsync } from "../../../shared/hooks/useAsync";
import { getCommunityIssues } from "../api/studentIssueApi";

export function useCommunityIssues(filters = {}) {
  return useAsync(() => getCommunityIssues(filters), [filters.search, filters.status], { initialData: [] });
}
