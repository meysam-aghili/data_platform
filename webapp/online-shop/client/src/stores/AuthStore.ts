import { AuthStoreActionsType, AuthStoreStateType } from "@/types";
import { create } from "zustand";
import Cookies from "js-cookie";

const useAuthStore = create<AuthStoreStateType & AuthStoreActionsType>()(
  (set, get) => ({
    token: Cookies.get("token") || "",
    setToken: (token: string) => {
      Cookies.set("token", token, {
        expires: 7,
        path: "/",
        sameSite: "strict",
        secure: process.env.NODE_ENV === "production",
      });
      set({ token });
    },
    removeToken: () => {
      Cookies.remove("token", { path: "/" });
      set({ token: "" });
    },
    isAuthenticated: () => {
      return Boolean(get().token || Cookies.get("token"));
    }
  })
);

export default useAuthStore;