import { useAsync } from "../../../shared/hooks/useAsync";
import { getCommunityIssueStats } from "../api/studentIssueApi";

export function useCommunityIssueStats() {
  return useAsync(getCommunityIssueStats, [], { initialData: {} });
}
