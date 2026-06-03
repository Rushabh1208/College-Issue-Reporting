import { useAsync } from "../../../shared/hooks/useAsync";
import { getUsers } from "../api/usersApi";

export function useUsers(filters) {
  return useAsync(() => getUsers(filters), [filters.page, filters.pageSize], { initialData: [] });
}
