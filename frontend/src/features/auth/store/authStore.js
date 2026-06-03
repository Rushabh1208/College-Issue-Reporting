import { create } from "zustand";
import { decodeJwt, isExpired } from "../utils";

const STORAGE_KEY = "campuscare_token";

function hydrateAuth() {
  const token = localStorage.getItem(STORAGE_KEY);
  const user = decodeJwt(token);
  if (!token || !user || isExpired(user)) {
    localStorage.removeItem(STORAGE_KEY);
    return { token: null, user: null };
  }
  return { token, user };
}

export const useAuthStore = create((set, get) => ({
  ...hydrateAuth(),
  setToken: (token) => {
    const user = decodeJwt(token);
    if (!user || isExpired(user)) {
      localStorage.removeItem(STORAGE_KEY);
      set({ token: null, user: null });
      return false;
    }
    localStorage.setItem(STORAGE_KEY, token);
    set({ token, user });
    return true;
  },
  logout: () => {
    localStorage.removeItem(STORAGE_KEY);
    set({ token: null, user: null });
  },
  ensureFreshSession: () => {
    const user = get().user;
    if (isExpired(user)) {
      get().logout();
      return false;
    }
    return Boolean(get().token);
  }
}));
