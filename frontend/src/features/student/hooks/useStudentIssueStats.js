import { useAsync } from "../../../shared/hooks/useAsync";
import { getStudentIssueStats } from "../api/studentIssueApi";

export function useStudentIssueStats() {
  return useAsync(getStudentIssueStats, [], { initialData: {} });
}
