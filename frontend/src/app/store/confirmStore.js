import { create } from "zustand";

export const useConfirmStore = create((set) => ({
  dialog: null, // { title, description, confirmLabel, cancelLabel, variant, resolve }
  request: (options) =>
    new Promise((resolve) => {
      set({ dialog: { ...options, resolve } });
    }),
  resolveDialog: (result) =>
    set((state) => {
      state.dialog?.resolve(result);
      return { dialog: null };
    })
}));
