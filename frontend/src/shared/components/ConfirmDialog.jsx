import { useConfirmStore } from "../../app/store/confirmStore";
import { Button } from "../ui/Button";

export function ConfirmDialog() {
  const dialog = useConfirmStore((state) => state.dialog);
  const resolveDialog = useConfirmStore((state) => state.resolveDialog);

  if (!dialog) return null;

  const {
    title = "Are you sure?",
    description,
    confirmLabel = "Confirm",
    cancelLabel = "Cancel",
    variant = "primary"
  } = dialog;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm px-4">
      <div className="w-full max-w-sm rounded-xl bg-white p-6 shadow-xl">
        <h3 className="text-lg font-bold text-slate-950">{title}</h3>
        {description && <p className="mt-2 text-sm leading-6 text-slate-600">{description}</p>}
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={() => resolveDialog(false)}>
            {cancelLabel}
          </Button>
          <Button variant={variant} onClick={() => resolveDialog(true)}>
            {confirmLabel}
          </Button>
        </div>
      </div>
    </div>
  );
}
