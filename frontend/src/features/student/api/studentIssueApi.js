import { apiClient } from "../../../shared/lib/apiClient";
import { normalizeIssue, normalizeIssues } from "../../issues/api/issueMappers";

export async function getStudentIssues() {
  const { data } = await apiClient.get("/student/issues");
  return normalizeIssues(data);
}

export async function getCategories() {
  const { data } = await apiClient.get("/categories");
  return data;
}

export async function reportIssue(payload, onUploadProgress) {
  const formData = new FormData();
  formData.append("Title", payload.title);
  formData.append("Description", payload.description);
  formData.append("Block", payload.block);
  formData.append("RoomNumber", payload.roomNumber);
  formData.append("CategoryId", payload.categoryId);
  formData.append("Priority", payload.priority);
  formData.append("IsAnonymous", payload.isAnonymous);
  if (payload.image?.[0]) formData.append("Image", payload.image[0]);

  const { data } = await apiClient.post("/issues/report", formData, {
    headers: { "Content-Type": "multipart/form-data" },
    onUploadProgress
  });
  return {
    ...data,
    issue: normalizeIssue(data.issue ?? data.Issue),
    imageUrl: data.imageUrl ?? data.ImageUrl ?? null,
    image: data.image ?? data.Image ?? null
  };
}

export async function upvoteIssue(id) {
  const { data } = await apiClient.post(`/issues/${id}/upvote`);
  return data;
}

export async function removeUpvote(id) {
  const { data } = await apiClient.delete(`/issues/${id}/upvote`);
  return data;
}
