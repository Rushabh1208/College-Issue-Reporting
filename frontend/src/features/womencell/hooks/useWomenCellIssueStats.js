import { useAsync } from "../../../shared/hooks/useAsync";
import { getWomenCellIssueStats } from "../api/womencellApi";

export function useWomenCellIssueStats() {
  return useAsync(getWomenCellIssueStats, [], { initialData: {} });
}
