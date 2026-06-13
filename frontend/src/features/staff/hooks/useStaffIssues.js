import { useAsync } from "../../../shared/hooks/useAsync";
import { getStaffIssues } from "../api/staffIssueApi";

export function useStaffIssues(filters = {}) {
  return useAsync(() => getStaffIssues(filters), [filters.status], { initialData: [] });
}
