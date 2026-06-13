import { ToastHost } from "../../shared/components/ToastHost.jsx";
import { ConfirmDialog } from "../../shared/components/ConfirmDialog.jsx";

export function AppProviders({ children }) {
  return (
    <>
      {children}
      <ToastHost />
      <ConfirmDialog />
    </>
  );
}
