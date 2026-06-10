function base64UrlDecode(value) {
  const base64 = value.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");
  return decodeURIComponent(
    atob(padded)
      .split("")
      .map((char) => `%${(`00${char.charCodeAt(0).toString(16)}`).slice(-2)}`)
      .join("")
  );
}

export function decodeJwt(token) {
  if (!token) return null;
  try {
    const payload = JSON.parse(base64UrlDecode(token.split(".")[1]));
    return {
      id: payload.nameid || payload.sub || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
      email: payload.unique_name || payload.email || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
      role: payload.role || payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
      studentId: payload.studentId || null,
      gender: payload.gender || null,
      exp: payload.exp
    };
  } catch {
    return null;
  }
}

export function isExpired(user) {
  return Boolean(user?.exp && Date.now() >= user.exp * 1000);
}

export function roleHome(role) {
  if (role === "Admin") return "/admin/issues";
  if (role === "Staff") return "/staff/issues";
  if (role === "WomenCell") return "/womencell/issues";
  return "/student/issues";
}
