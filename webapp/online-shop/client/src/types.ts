import { z } from "zod";

export type CategoryType = {
  id: number;
  title: string;
  slug: string;
  children: CategoryType[];
};

export type ImageType = {
  id: number;
  title: string;
  slug: string;
  url: string;
}

export type ProductVariantType = {
  id: number;
  title: string;
  slug: string;
  price: number;
  color_title: string;
  size_title: string;
  description: string;
  images: ImageType[];
  stock: number;
  discount: number;
}

export type ProductType = {
  id: number;
  title: string;
  slug: string;
  description: string;
  category: CategoryType;
  product_variants: ProductVariantType[];
};

export type PagedResultType<T> = {
  data: Array<T>;
  total_count: number;
  page_number: number;
  page_size: number;
};

export const shippingFormSchema = z.object({
  name: z.string().min(1, "Name is required"),
  phone: z
    .string()
    .min(7, "Phone number must be between 7 and 10 digits")
    .max(10, "Phone number must be between 7 and 10 digits")
    .regex(/^\d+$/, "Phone number must contain only numbers"),
  address: z.string().min(1, "Address is required"),
  state_title: z.string("State is required").min(1, "State is required"),
  city_title: z.string("City is required").min(1, "City is required"),
  postalCode: z.string()
    .min(10, "Postal code must have 10 digits")
    .max(10, "Postal code must have 10 digits")
    .regex(/^\d+$/, "Postal code must contain only numbers"),
});

export type ShippingFormInputs = z.infer<typeof shippingFormSchema>;

export const paymentFormSchema = z.object({
  cardHolder: z.string().min(1, "Card holder is required"),
  cardNumber: z
    .string()
    .min(16, "Card Number is required")
    .max(16, "Card Number is required"),
  expirationDate: z
    .string()
    .regex(
      /^(0[1-9]|1[0-2])\/\d{2}$/,
      "Expiration date must be in MM/YY format"
    ),
  cvv: z.string().min(3, "CVV is required").max(3, "CVV is required"),
});

export type PaymentFormInputs = z.infer<typeof paymentFormSchema>;

export type CartItemType = {
  productId: number;
  selectedProductVariantId: number;
  quantity: number;
};

export type CartStoreStateType = {
  cart: CartItemType[];
  hasHydrated: boolean;
};

export type CartStoreActionsType = {
  addToCart: (cartItem: CartItemType) => void;
  removeFromCart: (cartItem: CartItemType) => void;
  clearCart: () => void;
  changeQuantity: (cartItem: CartItemType, newQuantity: number) => void;
};

export type AuthStoreStateType = {
  token: string;
};

export type AuthStoreActionsType = {
  setToken: (token: string) => void;
  removeToken: () => void;
  isAuthenticated: () => boolean;
};

export const loginFormSchema = z.object({
  phone: z
    .string()
    .min(7, "Phone number must be between 7 and 15 digits")
    .max(15, "Phone number must be between 7 and 15 digits")
    .regex(/^\d+$/, "Phone number must contain only numbers"),
});

export type LoginFormInputs = z.infer<typeof loginFormSchema>;

export const loginCodeSchema = z.object({
  code: z
    .string()
    .min(4, "Code must be at least 4 digits")
    .max(8, "Code must be at most 8 digits")
    .regex(/^\d+$/, "Code must contain only numbers"),
});

export type LoginCodeInputs = z.infer<typeof loginCodeSchema>;

export type UserAddressType = {
  id: number;
  title: string;
  state_title: string;
  city_title: string;
  address: string;
  postal_code: string;
}
export type UserType = {
  title: string;
  slug: string;
  phone: string;
  user_addresses?: UserAddressType[];
}
