import { useAsync } from "../../../shared/hooks/useAsync";
import { getWomenCellIssues } from "../api/womencellApi";

export function useWomenCellIssues(filters = {}) {
  return useAsync(() => getWomenCellIssues(filters), [filters.status, filters.categoryId, filters.priority, filters.fromDate, filters.toDate, filters.page, filters.pageSize], { initialData: [] });
}
