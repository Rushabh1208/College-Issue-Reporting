export function normalizeIssue(issue) {
  if (!issue) return issue;

  return {
    id: issue.id ?? issue.Id,
    title: issue.title ?? issue.Title ?? "",
    description: issue.description ?? issue.Description ?? "",
    status: issue.status ?? issue.Status ?? "Open",
    block: issue.block ?? issue.Block ?? "",
    roomNumber: issue.roomNumber ?? issue.RoomNumber ?? "",
    assignedStaffName: issue.assignedStaffName ?? issue.AssignedStaffName ?? "Unassigned",
    imageUrl: issue.imageUrl ?? issue.ImageUrl ?? null,
    imageObjectKey: issue.imageObjectKey ?? issue.ImageObjectKey ?? null,
    imageStorageProvider: issue.imageStorageProvider ?? issue.ImageStorageProvider ?? null,
    imageMimeType: issue.imageMimeType ?? issue.ImageMimeType ?? null,
    imageSizeBytes: issue.imageSizeBytes ?? issue.ImageSizeBytes ?? null,
    createdAt: issue.createdAt ?? issue.CreatedAt ?? null
  };
}

export function normalizeIssues(issues) {
  return Array.isArray(issues) ? issues.map(normalizeIssue) : [];
}
