import { apiClient } from "../../../shared/lib/apiClient";

export async function importStudents(file) {
  const formData = new FormData();
  formData.append("file", file);

  const { data } = await apiClient.post("/admin/students/import", formData, {
    headers: {
      "Content-Type": "multipart/form-data"
    }
  });
  return data;
}

export async function getStudents(page = 1, pageSize = 20) {
  const { data } = await apiClient.get("/admin/students", {
    params: { page, pageSize }
  });
  return data;
}

export async function deactivateStudent(id) {
  const { data } = await apiClient.put(`/admin/students/${id}/deactivate`);
  return data;
}

export async function resetPassword(id) {
  const { data } = await apiClient.put(`/admin/students/${id}/reset-password`);
  return data;
}
