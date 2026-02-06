import { CartStoreActionsType, CartStoreStateType } from "@/types";
import { create } from "zustand";
import { persist, createJSONStorage } from "zustand/middleware";

const useCartStore = create<CartStoreStateType & CartStoreActionsType>()(
  persist(
    (set) => ({
      cart: [],
      hasHydrated: false,
      addToCart: (product) =>
        set((state) => {
          const existingIndex = state.cart.findIndex(
            (p) =>
              p.productId === product.productId &&
              p.selectedProductVariantId === product.selectedProductVariantId
          );

          if (existingIndex !== -1) {
            const updatedCart = [...state.cart];
            updatedCart[existingIndex].quantity += product.quantity || 1;
            return { cart: updatedCart };
          }

          return {
            cart: [
              ...state.cart,
              {
                ...product,
                quantity: product.quantity || 1,
                selectedProductVariantId: product.selectedProductVariantId,
              },
            ],
          };
        }),
      removeFromCart: (product) =>
        set((state) => ({
          cart: state.cart.filter(
            (p) =>
              !(
                p.productId === product.productId &&
                p.selectedProductVariantId === product.selectedProductVariantId
              )
          ),
        })),
      clearCart: () => set({ cart: [] }),
      changeQuantity: (product, newQuantity) =>
        set((state) => {
          const updatedCart = state.cart.map((item) => {
            const isSameProduct =
              item.productId === product.productId &&
              item.selectedProductVariantId === product.selectedProductVariantId;

            if (!isSameProduct) return item;

            return {
              ...item,
              quantity: newQuantity,
            };
          });

          return { cart: updatedCart };
        }),
    }),
    {
      name: "cart",
      storage: createJSONStorage(() => localStorage),
      onRehydrateStorage: () => (state) => {
        if (state) {
          state.hasHydrated = true;
        }
      },
    }
  )
);

export default useCartStore;
