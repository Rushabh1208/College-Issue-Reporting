import { apiClient } from "../../../shared/lib/apiClient";
import { normalizeIssues } from "../../issues/api/issueMappers";

export async function getStaffIssues() {
  const { data } = await apiClient.get("/staff/issues");
  return normalizeIssues(data);
}

export async function updateIssueStatus(issueId, status) {
  const { data } = await apiClient.put(`/staff/issues/${issueId}/status`, { status });
  return data;
}
