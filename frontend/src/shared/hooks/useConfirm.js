import { useConfirmStore } from "../../app/store/confirmStore";

export function useConfirm() {
  const request = useConfirmStore((state) => state.request);
  return request; // call as: await confirm({ title, description, confirmLabel, variant })
}
