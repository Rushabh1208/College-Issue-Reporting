import { apiClient } from "../../../shared/lib/apiClient";

export async function changePassword(payload) {
  const { data } = await apiClient.put("/account/change-password", payload);
  return data;
}
