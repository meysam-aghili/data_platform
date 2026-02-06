"use client";

import Box from "@/components/Box";
import PaymentForm from "@/components/PaymentForm";
import ShippingForm from "@/components/ShippingForm";
import { fetchData } from "@/shared/helpers/apiHelper";
import useCartStore from "@/stores/cartStore";
import { CartItemType, ProductType, ShippingFormInputs } from "@/types";
import { ArrowRight, Minus, Plus, Trash2 } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import { toast } from "react-toastify";

const steps = [
  { id: 1, title: "Shopping Cart" },
  { id: 2, title: "Shipping Address" },
  { id: 3, title: "Payment Method" },
];

type CartProductType = ProductType & CartItemType;

const CartPage = () => {
  const searchParams = useSearchParams();
  const router = useRouter();
  const [shippingForm, setShippingForm] = useState<ShippingFormInputs>();
  const activeStep = parseInt(searchParams.get("step") || "1");
  const { cart, removeFromCart, changeQuantity } = useCartStore();
  const [cartData, setCartData] = useState<CartProductType[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [refetch, setRefetch] = useState<boolean>(true); // replace by react query refetch

  useEffect(() => {
    const loadUpdatedCart = async () => {
      const updated = await Promise.all(
        cart.map(async (item) => {
          const product: ProductType = await fetchData({
            url: `products/id/${item.productId}`
          });

          return {
            ...product,
            productId: item.productId,
            selectedProductVariantId: item.selectedProductVariantId,
            quantity: item.quantity
          };
        })
      );

      setCartData(updated);
      setLoading(false);
    };

    if (cart.length > 0) {
      loadUpdatedCart();
    } else {
      setLoading(false);
    }
  }, [cart, refetch]);

  const subtotal = useMemo(() => {
    return cartData.reduce((acc, item) => {
      const variant = item.product_variants.find(
        (v) => v.id === item.selectedProductVariantId
      );

      const price = variant?.price ?? 0;

      return acc + price * item.quantity;
    }, 0);
  }, [cartData]);
  const discount = useMemo(() => {
    return cartData.reduce((acc, item) => {
      const variant = item.product_variants.find(
        (v) => v.id === item.selectedProductVariantId
      );
      const discount = variant?.discount ?? 0;
      return acc + discount * item.quantity;
    }, 0);
  }, [cartData]);
  const shipping = 10;
  const total = subtotal - discount + shipping;

  const hasStockIssue = useMemo(() => {
    return cartData.some(
      c => c.quantity > c.product_variants.find(pv => pv.id === c.selectedProductVariantId)!.stock
    );
  }, [cartData]);

  const handleChangeQuantity = (item: CartItemType, quantity: number) => {
    changeQuantity({
      productId: item.productId,
      selectedProductVariantId: item.selectedProductVariantId,
      quantity: item.quantity
    }, quantity
    )
    setRefetch((prev) => !prev)
  }

  const handlePreStepValidation = () => {
    if (hasStockIssue) {
      toast.error("Not in stock. please edit your cart.");
      router.push("/cart?step=1", { scroll: false });
      return false; // <<< IMPORTANT
    }
    return true;
  };

  const handleGotoStep = (stepId: number) => {
    if (stepId === 1) {
      router.push("/cart?step=1", { scroll: false });
      return;
    }
    if (!handlePreStepValidation()) return;
    router.push(`/cart?step=${stepId}`, { scroll: false });
  };

  const ShippingCartSection = () => {
    return (
      <>
        {cart.length > 0 ? (
          cartData.map((item) => {

            const selectedVariant = item.product_variants.find(
              (pv) => pv.id === item.selectedProductVariantId
            );

            return (
              // SINGLE CART ITEM
              <div
                className={`flex items-center justify-between border-b-1 pb-4 border-gray-300 last:border-b-0 rounded-md p-2 ${selectedVariant && item.quantity > selectedVariant?.stock ? "border-l-4 border-l-red-400" : ""}`}
                key={item.id + item.selectedProductVariantId}
              >
                {/* IMAGE AND DETAILS */}
                <div className="flex gap-8 items-center">
                  {/* IMAGE */}
                  <Link href={`/products/${item.slug}`}>
                    <div className="relative w-26 h-32 bg-gray-50 rounded-lg overflow-hidden">
                      <Image
                        src={selectedVariant?.images[0]?.url || "/no_image_placeholder.svg.png"}
                        alt={item.title}
                        fill
                        className="object-contain"
                        sizes="(max-width: 768px) 100vw, 33vw"
                      />
                    </div>
                  </Link>
                  {/* ITEM DETAILS */}
                  <div className="flex flex-col justify-between w-48">
                    <div className="flex flex-col gap-1">
                      <p className="text-sm font-medium">{item.title}</p>
                      <p className="text-xs text-gray-500">
                        Quantity: {item.quantity}
                      </p>
                      <p className="text-xs text-gray-500">
                        Size: {selectedVariant?.size_title}
                      </p>
                      <p className="text-xs text-gray-500">
                        Color: {selectedVariant?.color_title}
                      </p>
                      <p className="text-xs text-gray-500">
                        UnitPrice: {selectedVariant?.price}
                      </p>
                      {selectedVariant && selectedVariant.discount > 0 && (
                        <div className="flex gap-2 items-center">
                          <p className="text-xs text-gray-500">
                            Discount: {selectedVariant?.discount}
                          </p>
                          <span className="bg-red-500 text-white text-xs font-bold px-2 py-1 rounded-full">
                            {Math.round(
                              (selectedVariant.discount / selectedVariant.price) * 100
                            )}%
                          </span>
                        </div>
                      )}

                    </div>
                  </div>
                </div>
                {/* Select Quantity */}
                <div className="flex flex-col gap-3">
                  <div className="flex items-center gap-2">
                    <button
                      className="cursor-pointer border-1 border-gray-300 p-1"
                      onClick={() => handleChangeQuantity(item, item.quantity - 1)}
                      disabled={item.quantity <= 1}
                    >
                      <Minus className="w-4 h-4" />
                    </button>
                    <span>{item.quantity}</span>
                    <button
                      className="cursor-pointer border-1 border-gray-300 p-1"
                      onClick={() => handleChangeQuantity(item, item.quantity + 1)}
                      disabled={
                        item.quantity >= item.product_variants.find(pv => pv.id === item.selectedProductVariantId)!.stock
                      }
                    >
                      <Plus className="w-4 h-4" />
                    </button>
                  </div>
                  {selectedVariant && item.quantity > selectedVariant?.stock &&
                    <span className="font-semibold text-sm bg-red-400 text-white p-1 rounded-md">Not in stock</span>
                  }
                </div>

                {/* DELETE BUTTON */}
                <button
                  onClick={() => removeFromCart({
                    productId: item.id,
                    selectedProductVariantId: item.selectedProductVariantId,
                    quantity: item.quantity
                  })}
                  className="w-8 h-8 rounded-full bg-red-100 hover:bg-red-200 transition-all duration-300 text-red-400 flex items-center justify-center cursor-pointer"
                >
                  <Trash2 className="w-3 h-3" />
                </button>
              </div>
            )
          })
        ) : (<p className="text-sm text-gray-500">Cart is empty.</p>)}
      </>
    )
  }

  const ShippingAddressSection = () => {
    if (hasStockIssue) return (
      <p className="text-sm text-gray-500">
        Not in stock. please edit your cart.
      </p>
    );
    return (
      <>
        {cart.length > 0 ? (<ShippingForm setShippingForm={setShippingForm} />) : (
          <p className="text-sm text-gray-500">
            Please add some items to cart to continue.
          </p>
        )}
      </>
    )
  }

  const PaymentMethodSection = () => {
    if (hasStockIssue) return (
      <p className="text-sm text-gray-500">
        Not in stock. please edit your cart.
      </p>
    );
    if (!shippingForm) return (
      <p className="text-sm text-gray-500">
        Please fill in the shipping form to continue.
      </p>
    )
    return (
      <PaymentForm />
    )
  }

  return (
    <>
      {loading ? (
        <p className="text-center mt-12">Loading cart...</p>
      ) : (
        <div className="flex flex-col gap-6 items-center justify-center mt-10">
          {/* STEPS */}
          <div className="flex flex-col lg:flex-row items-center gap-8 lg:gap-16">
            {steps.map((step) => (
              <button key={step.id} onClick={() => handleGotoStep(step.id)}
                className={`cursor-pointer flex items-center gap-2 border-b-2 pb-4 ${step.id === activeStep ? "border-gray-800" : "border-gray-200"
                  }`}
              >
                <div
                  className={`w-6 h-6 rounded-full text-white p-4 flex items-center justify-center ${step.id === activeStep ? "bg-gray-800" : "bg-gray-400"
                    }`}
                >
                  {step.id}
                </div>
                <p
                  className={`text-sm font-medium ${step.id === activeStep ? "text-gray-800" : "text-gray-400"
                    }`}
                >
                  {step.title}
                </p>
              </button>
            ))}
          </div>
          {/* STEPS & DETAILS */}
          <div className="w-full flex flex-col lg:flex-row gap-16">
            {/* STEPS */}
            <div className="lg:w-7/12">
              <Box>
                <div className="flex flex-col gap-4 w-full p-3">
                  {activeStep === 1 ? <ShippingCartSection />
                    : activeStep === 2 ? <ShippingAddressSection />
                      : activeStep === 3 ? <PaymentMethodSection />
                        : (
                          <p className="text-sm text-gray-500">
                            Please fill in the shipping form to continue.
                          </p>
                        )}
                </div>
              </Box>
            </div>
            {/* DETAILS */}
            <div className="lg:w-5/12">
              <Box>
                <div className="flex flex-col gap-4 h-max w-full p-3">
                  <h2 className="font-semibold">Cart Details</h2>
                  <div className="flex flex-col gap-4">
                    <div className="flex justify-between text-sm">
                      <p className="text-gray-500">Subtotal</p>
                      <p className="font-medium">${subtotal.toFixed(2)}</p>
                    </div>

                    <div className="flex justify-between text-sm">
                      <p className="text-gray-500">Discount ({Math.round((discount / subtotal) * 100)}%)</p>
                      <p className="font-medium">-${discount.toFixed(2)}</p>
                    </div>

                    <div className="flex justify-between text-sm">
                      <p className="text-gray-500">Shipping Fee</p>
                      <p className="font-medium">${shipping.toFixed(2)}</p>
                    </div>

                    <hr className="border-gray-200" />

                    <div className="flex justify-between">
                      <p className="text-gray-800 font-semibold">Total</p>
                      <p className="font-medium">${total.toFixed(2)}</p>
                    </div>
                  </div>
                  {activeStep === 1 && (
                    <button
                      onClick={() => handleGotoStep(2)}
                      className="w-full bg-gray-800 hover:bg-gray-900 transition-all duration-300 text-white p-2 rounded-lg cursor-pointer flex items-center justify-center gap-2"
                    >
                      Continue
                      <ArrowRight className="w-3 h-3" />
                    </button>
                  )}
                </div>
              </Box>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default CartPage;
