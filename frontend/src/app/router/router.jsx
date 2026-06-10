import { lazy, Suspense } from "react";
import { createBrowserRouter } from "react-router-dom";
import { AuthLayout } from "../layouts/AuthLayout.jsx";
import { AppLayout } from "../layouts/AppLayout.jsx";
import { RequireAuth, PublicOnly, RoleRedirect } from "./guards.jsx";
import { ROLES } from "../../shared/constants/api";
import { SkeletonList } from "../../shared/ui/Skeleton.jsx";
import { ErrorState } from "../../shared/ui/ErrorState.jsx";

const LoginPage = lazy(() => import("../../features/auth/pages/LoginPage.jsx"));
const StudentIssuesPage = lazy(() => import("../../features/student/pages/StudentIssuesPage.jsx"));
const ReportIssuePage = lazy(() => import("../../features/student/pages/ReportIssuePage.jsx"));
const StaffIssuesPage = lazy(() => import("../../features/staff/pages/StaffIssuesPage.jsx"));
const AdminIssuesPage = lazy(() => import("../../features/admin/pages/AdminIssuesPage.jsx"));
const AdminUsersPage = lazy(() => import("../../features/users/pages/AdminUsersPage.jsx"));
const AdminStudentsPage = lazy(() => import("../../features/admin/pages/AdminStudentsPage.jsx"));

function PageLoader() {
  return <div className="py-6"><SkeletonList rows={3} /></div>;
}

export const router = createBrowserRouter([
  {
    path: "/",
    element: <RoleRedirect />
  },
  {
    element: <PublicOnly />,
    children: [
      {
        element: <AuthLayout />,
        children: [
          { path: "/login", element: <Suspense fallback={<PageLoader />}><LoginPage /></Suspense> }
        ]
      }
    ]
  },
  {
    element: <RequireAuth roles={[ROLES.STUDENT, ROLES.STAFF, ROLES.ADMIN, ROLES.WOMENCELL]} />,
    children: [
      {
        element: <AppLayout />,
        children: [
          {
            element: <RequireAuth roles={[ROLES.STUDENT]} />,
            children: [
              { path: "/student/issues", element: <Suspense fallback={<PageLoader />}><StudentIssuesPage /></Suspense> },
              { path: "/student/report", element: <Suspense fallback={<PageLoader />}><ReportIssuePage /></Suspense> }
            ]
          },
          {
            element: <RequireAuth roles={[ROLES.STAFF, ROLES.ADMIN]} />,
            children: [
              { path: "/staff/issues", element: <Suspense fallback={<PageLoader />}><StaffIssuesPage /></Suspense> }
            ]
          },
          {
            element: <RequireAuth roles={[ROLES.ADMIN]} />,
            children: [
              { path: "/admin/issues", element: <Suspense fallback={<PageLoader />}><AdminIssuesPage /></Suspense> },
              { path: "/admin/users", element: <Suspense fallback={<PageLoader />}><AdminUsersPage /></Suspense> },
              { path: "/admin/students", element: <Suspense fallback={<PageLoader />}><AdminStudentsPage /></Suspense> }
            ]
          }
        ]
      }
    ]
  },
  {
    path: "*",
    element: (
      <main className="grid min-h-screen place-items-center px-4">
        <ErrorState title="Page not found" message="The screen you opened does not exist." />
      </main>
    )
  }
]);
