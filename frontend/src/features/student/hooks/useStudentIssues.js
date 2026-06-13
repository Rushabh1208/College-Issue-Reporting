import { useAsync } from "../../../shared/hooks/useAsync";
import { getStudentIssues } from "../api/studentIssueApi";

export function useStudentIssues(filters = {}) {
  return useAsync(() => getStudentIssues(filters), [filters.status, filters.all], { initialData: [] });
}
