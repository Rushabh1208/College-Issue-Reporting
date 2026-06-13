import { apiClient } from "../../../shared/lib/apiClient";
import { normalizeIssues } from "../../issues/api/issueMappers";

export const getWomenCellIssues = async (filters = {}) => {
  const { data } = await apiClient.get("/womencell/issues", { params: filters });
  return normalizeIssues(data);
};

export const getWomenCellIssueStats = async () => {
  const { data } = await apiClient.get("/womencell/issues/stats");
  return data;
};

export const updateWomenCellIssueStatus = async (id, status) => {
  const { data } = await apiClient.put(`/womencell/issues/${id}/status`, { status });
  return data;
};
