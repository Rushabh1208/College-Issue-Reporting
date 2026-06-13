import { apiClient } from "../../../shared/lib/apiClient";
import { normalizeIssues } from "../../issues/api/issueMappers";

export async function getAdminIssues({ status, page = 1, pageSize = 10 }) {
  const { data } = await apiClient.get("/admin/issues", {
    params: {
      status: status || undefined,
      page,
      pageSize
    }
  });
  return normalizeIssues(data);
}

export async function getAdminIssueStats() {
  const { data } = await apiClient.get("/admin/issues/stats");
  return data;
}

export async function assignIssue(issueId, staffId) {
  const { data } = await apiClient.put(`/admin/issues/${issueId}/assign/${staffId}`);
  return data;
}

export async function deleteIssue(issueId) {
  const { data } = await apiClient.delete(`/admin/issues/${issueId}`);
  return data;
}
