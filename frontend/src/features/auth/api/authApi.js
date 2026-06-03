import { apiClient } from "../../../shared/lib/apiClient";

export async function login(payload) {
  const { data } = await apiClient.post("/login", payload);
  return data;
}

export async function registerStudent(payload) {
  const { data } = await apiClient.post("/register", payload);
  return data;
}
