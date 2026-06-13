import { useAsync } from "../../../shared/hooks/useAsync";
import { getStaffIssueStats } from "../api/staffIssueApi";

export function useStaffIssueStats() {
  return useAsync(getStaffIssueStats, [], { initialData: {} });
}
