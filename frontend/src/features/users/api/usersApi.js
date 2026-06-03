import { apiClient } from "../../../shared/lib/apiClient";
import { normalizeUsers } from "./userMappers";

export async function getUsers({ page = 1, pageSize = 10 }) {
  const { data } = await apiClient.get("/admin/users", {
    params: { page, pageSize }
  });
  return normalizeUsers(data);
}
