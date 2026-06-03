import { useAsync } from "../../../shared/hooks/useAsync";
import { getStaffIssues } from "../api/staffIssueApi";

export function useStaffIssues() {
  return useAsync(getStaffIssues, [], { initialData: [] });
}
