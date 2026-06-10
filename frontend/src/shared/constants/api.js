export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "https://college-issue-reporting.onrender.com";

export const ROLES = {
  STUDENT: "Student",
  STAFF: "Staff",
  ADMIN: "Admin",
  WOMENCELL: "WomenCell"
};

export const ISSUE_STATUSES = ["Open", "InProgress", "Resolved"];
