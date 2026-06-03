import { create } from "zustand";

let toastId = 0;

export const useUiStore = create((set) => ({
  toasts: [],
  pushToast: (toast) => {
    const id = ++toastId;
    set((state) => ({
      toasts: [...state.toasts, { id, type: "info", ...toast }]
    }));
    window.setTimeout(() => {
      set((state) => ({ toasts: state.toasts.filter((item) => item.id !== id) }));
    }, toast.duration || 4200);
  },
  removeToast: (id) => set((state) => ({ toasts: state.toasts.filter((item) => item.id !== id) }))
}));
