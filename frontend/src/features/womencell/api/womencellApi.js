import { apiClient } from "../../../shared/lib/apiClient";

export const getWomenCellIssues = async () => {
  const { data } = await apiClient.get("/womencell/issues");
  return data;
};

export const updateWomenCellIssueStatus = async (id, status) => {
  const { data } = await apiClient.put(`/womencell/issues/${id}/status`, { status });
  return data;
};
