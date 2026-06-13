import { apiClient } from "../../../shared/lib/apiClient";

export async function getIssueTimeline(id) {
  const { data } = await apiClient.get(`/issues/${id}/timeline`);
  return data;
}
