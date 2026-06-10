import { apiClient } from "../../../shared/lib/apiClient";

export async function login(payload) {
  const { data } = await apiClient.post("/login", payload);
  return data;
}
