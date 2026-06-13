import { useAsync } from "../../../shared/hooks/useAsync";
import { getAdminIssueStats } from "../api/adminIssueApi";

export function useAdminIssueStats() {
  return useAsync(getAdminIssueStats, [], { initialData: {} });
}
