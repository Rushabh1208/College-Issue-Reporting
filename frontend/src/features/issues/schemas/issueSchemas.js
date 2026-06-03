import { ISSUE_STATUSES } from "../../../shared/constants/api";

export const issueFormRules = {
  title: {
    required: "Title is required",
    maxLength: { value: 100, message: "Title must be 100 characters or less" }
  },
  description: {
    required: "Description is required",
    maxLength: { value: 500, message: "Description must be 500 characters or less" }
  },
  block: {
    required: "Block is required",
    maxLength: { value: 10, message: "Block must be 10 characters or less" }
  },
  roomNumber: {
    required: "Room number is required",
    maxLength: { value: 10, message: "Room number must be 10 characters or less" }
  },
  image: {
    validate: (files) => {
      const file = files?.[0];
      if (!file) return true;
      if (!["image/jpeg", "image/jpg", "image/png"].includes(file.type)) return "Upload a JPEG or PNG image";
      if (file.size > 5 * 1024 * 1024) return "Image must be under 5 MB";
      return true;
    }
  }
};

export function isValidStatus(status) {
  return ISSUE_STATUSES.includes(status);
}
