import { Link, NavLink, Outlet, useLocation, useNavigate } from "react-router-dom";
import { ClipboardList, LogOut, PlusCircle, UsersRound, Wrench, LayoutDashboard, ShieldAlert, GraduationCap } from "lucide-react";
import { Button } from "../../shared/ui/Button";
import { ROLES } from "../../shared/constants/api";
import { cn } from "../../shared/utils/cn";
import { useAuthStore } from "../../features/auth/store/authStore";

const roleNav = {
  [ROLES.STUDENT]: [
    { to: "/student/issues", label: "Issues", icon: ClipboardList },
    { to: "/student/report", label: "Report", icon: PlusCircle }
  ],
  [ROLES.STAFF]: [
    { to: "/staff/issues", label: "Assigned", icon: Wrench }
  ],
  [ROLES.ADMIN]: [
    { to: "/admin/issues", label: "Issues", icon: LayoutDashboard },
    { to: "/admin/users", label: "Users", icon: UsersRound },
    { to: "/admin/students", label: "Students", icon: GraduationCap }
  ],
  [ROLES.WOMENCELL]: [
    { to: "/womencell/issues", label: "Complaints", icon: ShieldAlert }
  ]
};

function NavItems({ items, compact = false }) {
  return items.map((item) => {
    const Icon = item.icon;
    return (
      <NavLink
        key={item.to}
        to={item.to}
        className={({ isActive }) =>
          cn(
            "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-semibold transition",
            compact && "flex-1 flex-col gap-1 px-2 py-2 text-xs",
            isActive ? "bg-brand-50 text-brand-700" : "text-slate-600 hover:bg-slate-100 hover:text-slate-950"
          )
        }
      >
        <Icon className="h-5 w-5" aria-hidden="true" />
        <span>{item.label}</span>
      </NavLink>
    );
  });
}

export function AppLayout() {
  const { user, logout } = useAuthStore();
  const navigate = useNavigate();
  const location = useLocation();
  const navItems = roleNav[user?.role] || roleNav[ROLES.STUDENT];
  const pageTitle = navItems.find((item) => location.pathname.startsWith(item.to))?.label || "Dashboard";

  function handleLogout() {
    logout();
    navigate("/login", { replace: true });
  }

  return (
    <div className="min-h-screen lg:grid lg:grid-cols-[260px_1fr]">
      <aside className="fixed inset-y-0 left-0 z-30 hidden w-64 border-r border-slate-200 bg-white px-4 py-5 lg:block">
        <Link to="/" className="block">
          <p className="text-sm font-black text-brand-600">CampusCare</p>
          <p className="mt-1 text-lg font-black text-slate-950">Issue Desk</p>
        </Link>
        <nav className="mt-8 grid gap-1">
          <NavItems items={navItems} />
        </nav>
        <div className="absolute inset-x-4 bottom-4 rounded-lg border border-slate-200 bg-slate-50 p-3">
          <p className="text-sm font-bold text-slate-950">{user?.email}</p>
          <p className="mt-0.5 text-xs font-semibold text-slate-500">{user?.role}</p>
          <Button className="mt-3 w-full" variant="secondary" onClick={handleLogout}>
            <LogOut className="h-4 w-4" aria-hidden="true" />
            Logout
          </Button>
        </div>
      </aside>

      <div className="lg:col-start-2">
        <header className="sticky top-0 z-20 border-b border-slate-200 bg-white/90 px-4 py-3 backdrop-blur lg:px-8">
          <div className="mx-auto flex max-w-6xl items-center justify-between gap-3">
            <div>
              <p className="text-xs font-bold uppercase tracking-wide text-slate-500">{user?.role}</p>
              <h1 className="text-xl font-black text-slate-950">{pageTitle}</h1>
            </div>
            <Button className="hidden sm:inline-flex lg:hidden" variant="secondary" onClick={handleLogout}>
              <LogOut className="h-4 w-4" aria-hidden="true" />
              Logout
            </Button>
          </div>
        </header>

        <main className="mx-auto max-w-6xl px-4 pb-28 pt-5 lg:px-8 lg:pb-10">
          <Outlet />
        </main>
      </div>

      <nav className="fixed inset-x-0 bottom-0 z-40 border-t border-slate-200 bg-white/95 px-2 pb-[calc(env(safe-area-inset-bottom)+0.35rem)] pt-2 shadow-[0_-12px_30px_rgba(15,23,42,0.08)] backdrop-blur lg:hidden">
        <div className="flex items-center gap-1">
          <NavItems items={navItems} compact />
          <button
            className="flex flex-1 flex-col items-center gap-1 rounded-lg px-2 py-2 text-xs font-semibold text-slate-600 hover:bg-slate-100"
            onClick={handleLogout}
            type="button"
          >
            <LogOut className="h-5 w-5" aria-hidden="true" />
            Logout
          </button>
        </div>
      </nav>
    </div>
  );
}
