import { ToastHost } from "../../shared/components/ToastHost.jsx";

export function AppProviders({ children }) {
  return (
    <>
      {children}
      <ToastHost />
    </>
  );
}
