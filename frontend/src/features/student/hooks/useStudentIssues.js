import { useAsync } from "../../../shared/hooks/useAsync";
import { getStudentIssues } from "../api/studentIssueApi";

export function useStudentIssues() {
  return useAsync(getStudentIssues, [], { initialData: [] });
}
