const STATUS_MAP = {
  0: "Open",
  1: "InProgress",
  2: "Resolved",
  3: "Closed",
  "Open": "Open",
  "InProgress": "InProgress",
  "Resolved": "Resolved",
  "Closed": "Closed"
};

const PRIORITY_MAP = {
  0: "Low",
  1: "Medium",
  2: "High",
  3: "Critical",
  "Low": "Low",
  "Medium": "Medium",
  "High": "High",
  "Critical": "Critical"
};

export function normalizeIssue(issue) {
  if (!issue) return issue;

  return {
    id: issue.id ?? issue.Id,
    title: issue.title ?? issue.Title ?? "",
    description: issue.description ?? issue.Description ?? "",
    status: STATUS_MAP[issue.status ?? issue.Status] ?? "Open",
    block: issue.block ?? issue.Block ?? "",
    roomNumber: issue.roomNumber ?? issue.RoomNumber ?? "",
    assignedStaffName: issue.assignedStaffName ?? issue.AssignedStaffName ?? "Unassigned",
    imageUrl: issue.imageUrl ?? issue.ImageUrl ?? null,
    imageObjectKey: issue.imageObjectKey ?? issue.ImageObjectKey ?? null,
    imageStorageProvider: issue.imageStorageProvider ?? issue.ImageStorageProvider ?? null,
    imageMimeType: issue.imageMimeType ?? issue.ImageMimeType ?? null,
    imageSizeBytes: issue.imageSizeBytes ?? issue.ImageSizeBytes ?? null,
    createdAt: issue.createdAt ?? issue.CreatedAt ?? null,
    categoryName: issue.categoryName ?? issue.CategoryName ?? "",
    priority: PRIORITY_MAP[issue.priority ?? issue.Priority] ?? "Low",
    isAnonymous: issue.isAnonymous ?? issue.IsAnonymous ?? false,
    upvoteCount: issue.upvoteCount ?? issue.UpvoteCount ?? 0,
    hasUpvoted: issue.hasUpvoted ?? issue.HasUpvoted ?? null,
    reporterName: issue.reporterName ?? issue.ReporterName ?? "",
    reporterStudentId: issue.reporterStudentId ?? issue.ReporterStudentId ?? "",
    isOwnIssue: issue.isOwnIssue ?? issue.IsOwnIssue ?? false
  };
}

export function normalizeIssues(issues) {
  return Array.isArray(issues) ? issues.map(normalizeIssue) : [];
}
