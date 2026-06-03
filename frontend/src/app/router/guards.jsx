import { Navigate, Outlet, useLocation } from "react-router-dom";
import { roleHome } from "../../features/auth/utils";
import { useAuthStore } from "../../features/auth/store/authStore";

export function PublicOnly() {
  const { user, ensureFreshSession } = useAuthStore();
  if (ensureFreshSession() && user?.role) return <Navigate to={roleHome(user.role)} replace />;
  return <Outlet />;
}

export function RequireAuth({ roles }) {
  const { user, ensureFreshSession } = useAuthStore();
  const location = useLocation();

  if (!ensureFreshSession()) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  if (roles?.length && !roles.includes(user?.role)) {
    return <Navigate to={roleHome(user?.role)} replace />;
  }

  return <Outlet />;
}

export function RoleRedirect() {
  const { user, ensureFreshSession } = useAuthStore();
  if (!ensureFreshSession()) return <Navigate to="/login" replace />;
  return <Navigate to={roleHome(user?.role)} replace />;
}
