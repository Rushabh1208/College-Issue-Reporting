import { apiClient } from "../../../shared/lib/apiClient";
import { normalizeIssue, normalizeIssues } from "../../issues/api/issueMappers";

export async function getStudentIssues() {
  const { data } = await apiClient.get("/student/issues");
  return normalizeIssues(data);
}

export async function reportIssue(payload, onUploadProgress) {
  const formData = new FormData();
  formData.append("Title", payload.title);
  formData.append("Description", payload.description);
  formData.append("Block", payload.block);
  formData.append("RoomNumber", payload.roomNumber);
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
