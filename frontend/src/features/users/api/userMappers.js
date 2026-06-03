export function normalizeUser(user) {
  if (!user) return user;

  return {
    id: user.id ?? user.Id,
    name: user.name ?? user.Name ?? "",
    email: user.email ?? user.Email ?? "",
    role: user.role ?? user.Role ?? "Student"
  };
}

export function normalizeUsers(users) {
  return Array.isArray(users) ? users.map(normalizeUser) : [];
}
