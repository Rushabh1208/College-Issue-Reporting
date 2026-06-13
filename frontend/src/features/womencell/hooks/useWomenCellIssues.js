import { useAsync } from "../../../shared/hooks/useAsync";
import { getWomenCellIssues } from "../api/womencellApi";

export function useWomenCellIssues() {
  return useAsync(getWomenCellIssues, [], { initialData: [] });
}
