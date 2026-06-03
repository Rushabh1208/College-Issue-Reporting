import { Outlet } from "react-router-dom";

export function AuthLayout() {
  return (
    <main className="min-h-screen px-4 py-6 sm:grid sm:place-items-center">
      <div className="mx-auto w-full max-w-md">
        <div className="mb-7">
          <p className="text-sm font-bold uppercase tracking-wide text-brand-600">CampusCare</p>
          <h1 className="mt-2 text-3xl font-black text-slate-950">College Issue Reporting</h1>
          <p className="mt-2 text-sm leading-6 text-slate-600">Report, assign, and resolve campus issues from any device.</p>
        </div>
        <Outlet />
      </div>
    </main>
  );
}
