import { apiClient } from "../../../shared/lib/apiClient";

export async function getStaff({ page = 1, pageSize = 20 }) {
  const { data } = await apiClient.get("/admin/staff", {
    params: { page, pageSize }
  });
  return data; // returning the paginated response directly
}

export async function createStaff(payload) {
  const { data } = await apiClient.post("/admin/staff", payload);
  return data;
}

export async function updateStaff(id, payload) {
  const { data } = await apiClient.put(`/admin/staff/${id}`, payload);
  return data;
}

export async function deactivateStaff(id) {
  const { data } = await apiClient.put(`/admin/staff/${id}/deactivate`);
  return data;
}

export async function resetStaffPassword(id) {
  const { data } = await apiClient.put(`/admin/staff/${id}/reset-password`);
  return data;
}
