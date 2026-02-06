"use client";

import useCartStore from "@/stores/cartStore";
import { ProductType } from "@/types";
import { Minus, Plus, ShoppingCart } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { toast } from "react-toastify";

type ComponentType = "ProductPage" | "ProductCard";

interface ProductInteractionProps {
  component: ComponentType;
  product: ProductType;
}

const ProductInteraction = ({ component, product }: ProductInteractionProps) => {

  const { addToCart, cart, removeFromCart, changeQuantity } = useCartStore();
  const [selectedOptions, setSelectedOptions] = useState({
    productVariant: product.product_variants.find(pv => pv.stock > 0),
    quantity: 1
  });

  const currentProductVariant = useMemo(() => {
    return product.product_variants.find(
      pv => pv.id === selectedOptions.productVariant?.id
    ) || null;
  }, [selectedOptions.productVariant]);

  const currentCartItem = useMemo(() => {
    return cart.find(
      c => c.productId === product.id && c.selectedProductVariantId === selectedOptions.productVariant?.id
    ) || null;
  }, [selectedOptions.productVariant, cart]);

  useEffect(() => {
    if (!currentCartItem) {
      setSelectedOptions(prev => ({ ...prev, quantity: 1 }));
      return;
    }
    if (currentCartItem.quantity !== selectedOptions.quantity) {
      setSelectedOptions(prev => ({ ...prev, quantity: currentCartItem.quantity }));
    }
  }, [currentCartItem]);

  const stockMap = useMemo(() => {
    const map: Record<string, Record<string, number>> = {};
    product.product_variants.forEach(v => {
      if (!map[v.color_title]) map[v.color_title] = {};
      map[v.color_title][v.size_title] = v.stock;
    });
    return map;
  }, [product.product_variants]);

  const uniqueColors = useMemo(
    () => [...new Set(product.product_variants.map(v => v.color_title))],
    [product.product_variants]
  );

  // Available sizes for selected color
  const availableSizes = useMemo(() => {
    if (!selectedOptions.productVariant) return [];
    const color = selectedOptions.productVariant.color_title;
    return [
      ...new Set(
        product.product_variants
          .filter(v => v.color_title === color)
          .map(v => v.size_title)
      )
    ].sort((a, b) => b.localeCompare(a));
  }, [product.product_variants, selectedOptions.productVariant]);

  // Fix variant if current size is not available
  useEffect(() => {
    if (!selectedOptions.productVariant || !availableSizes.length) return;

    if (!availableSizes.includes(selectedOptions.productVariant.size_title)) {
      const newVariant = product.product_variants.find(
        v => v.color_title === selectedOptions.productVariant?.color_title &&
          availableSizes.includes(v.size_title)
      );
      if (newVariant) {
        setSelectedOptions(prev => ({ ...prev, productVariant: newVariant }));
      }
    }
  }, [availableSizes, selectedOptions.productVariant, product.product_variants]);

  const handleChangeSelectedOption = (
    type: "size" | "color" | "quantity",
    value: string | number
  ) => {
    setSelectedOptions(prev => {
      if (type === "quantity") {
        const numValue = Number(value);
        if (prev.productVariant && numValue > 0 && numValue <= prev.productVariant.stock) {
          return { ...prev, quantity: numValue };
        }
        return prev;
      }

      if (type === "color") {
        const match = product.product_variants.find(
          v => v.color_title === value && v.stock > 0
        );
        return match ? { ...prev, productVariant: match } : prev;
      }

      if (type === "size" && prev.productVariant) {
        const match = product.product_variants.find(
          v => v.size_title === value && v.color_title === prev.productVariant!.color_title
        );
        return match ? { ...prev, productVariant: match } : prev;
      }

      return prev;
    });
  };

  const handleAddToCart = () => {
    if (!selectedOptions.productVariant || selectedOptions.productVariant.stock <= 0) {
      toast.error("The item is out of stock.");
      return;
    }

    addToCart({
      productId: product.id,
      quantity: selectedOptions.quantity,
      selectedProductVariantId: selectedOptions.productVariant.id
    });
    toast.success("Item added to cart");
  };

  const handleQuantityChange = (type: "increment" | "decrement") => {
    if (type === "increment" && currentProductVariant && selectedOptions.quantity < currentProductVariant.stock) {
      handleChangeSelectedOption("quantity", selectedOptions.quantity + 1);
      if (currentCartItem) changeQuantity(currentCartItem, selectedOptions.quantity + 1);
    } else if (selectedOptions.quantity > 1) {
      handleChangeSelectedOption("quantity", selectedOptions.quantity - 1);
      if (currentCartItem) changeQuantity(currentCartItem, selectedOptions.quantity - 1);
    }
  };

  const isInCart = useMemo(() => {
    if (!selectedOptions.productVariant) return false;
    return cart.some(
      i => i.productId === product.id && i.selectedProductVariantId === selectedOptions.productVariant!.id
    );
  }, [cart, product.id, selectedOptions.productVariant]);

  const SelectSize = () => {
    if (component === "ProductCard") {
      return (
        <div className="flex gap-2 flex-col">
          <span className="text-gray-500">Size</span>
          <select
            name="size"
            id="size"
            className="ring ring-gray-300 rounded-md px-2 py-1"
            value={selectedOptions.productVariant?.size_title || ""}
            onChange={(e) => handleChangeSelectedOption("size", e.target.value)}
          >
            {availableSizes.map((size) => {
              if (!selectedOptions.productVariant) return null;
              const stock = stockMap[selectedOptions.productVariant.color_title]?.[size] || 0;
              if (stock === 0) return null;

              return (
                <option key={size} value={size}>
                  {size.toUpperCase()}
                </option>
              );
            })}
          </select>
        </div>
      );
    }

    return (
      <div className="flex flex-col gap-1">
        <span className="text-gray-500">Size</span>
        <div className="flex items-center gap-1">
          {availableSizes.map((size) => {
            const stock = selectedOptions.productVariant
              ? stockMap[selectedOptions.productVariant.color_title]?.[size] || 0
              : 0;
            const isOut = stock === 0;
            const isSelected = selectedOptions.productVariant?.size_title === size;

            return (
              <div
                key={size}
                className={`relative cursor-pointer border-1 p-[2px] ${isSelected ? "border-gray-600" : "border-gray-300"
                  }`}
                style={{ opacity: isOut ? 0.5 : 1 }}
                onClick={() => !isOut && handleChangeSelectedOption("size", size)}
              >
                <div
                  className={`w-6 h-6 text-center flex items-center justify-center ${isSelected ? "bg-black text-white" : "bg-white text-black"
                    }`}
                >
                  {isOut && (
                    <div className="pointer-events-none absolute inset-0 before:content-[''] before:absolute before:left-1/2 before:top-1/2 before:w-[140%] before:h-[2px] before:bg-red-500 before:opacity-70 before:-translate-x-1/2 before:-translate-y-1/2 before:rotate-45 after:content-[''] after:absolute after:left-1/2 after:top-1/2 after:w-[140%] after:h-[2px] after:bg-red-500 after:opacity-70 after:-translate-x-1/2 after:-translate-y-1/2 after:-rotate-45" />
                  )}
                  {size.toUpperCase()}
                </div>
              </div>
            );
          })}
        </div>
      </div>
    );
  };

  const SelectColor = () => {
    return (
      <div className="flex gap-2 flex-col">
        <span className="text-gray-500">Color</span>
        <div className="flex items-center gap-1">
          {uniqueColors.map((color) => {
            const isOut = uniqueColors.filter(color => Object.values(stockMap[color] || {}).every(stock => stock === 0)).includes(color);
            const isSelected = selectedOptions.productVariant?.color_title === color;

            return (
              <div
                key={color}
                className={`cursor-pointer rounded-full border-1 p-[1.5px] ${isSelected ? "border-gray-500" : "border-white"
                  }`}
                style={{ opacity: isOut ? 0.3 : 1 }}
                onClick={() => !isOut && handleChangeSelectedOption("color", color)}
              >
                <div
                  className={`rounded-full ${component === "ProductCard" ? "w-[14px] h-[14px]" : "w-6 h-6"
                    }`}
                  style={{ backgroundColor: color }}
                />
              </div>
            );
          })}
        </div>
      </div>
    );
  };

  const SelectQuantity = () => {
    return (
      <div className="flex flex-col gap-1">
        <span className="text-gray-500">Quantity</span>
        <div className="flex items-center gap-2">
          <button
            className="cursor-pointer border-1 border-gray-300 p-1"
            onClick={() => handleQuantityChange("decrement")}
            disabled={selectedOptions.quantity <= 1}
          >
            <Minus className="w-4 h-4" />
          </button>
          <span>{selectedOptions.quantity}</span>
          <button
            className="cursor-pointer border-1 border-gray-300 p-1"
            onClick={() => handleQuantityChange("increment")}
            disabled={
              !selectedOptions.productVariant ||
              selectedOptions.quantity >= selectedOptions.productVariant.stock
            }
          >
            <Plus className="w-4 h-4" />
          </button>
        </div>
      </div>
    );
  };

  const PriceAndButtons = () => {
    const variant = selectedOptions.productVariant;
    if (!variant) {
      return <span className="text-red-400 font-semibold">Out of stock</span>;
    }

    const finalPrice = variant.price - variant.discount;
    const discountPercent = variant.discount > 0
      ? Math.round((variant.discount / variant.price) * 100)
      : 0;

    return (
      <div className={`${component === "ProductCard"
        ? "flex items-center justify-between"
        : "flex flex-col gap-4"
        }`}>
        <div className="flex gap-1 flex-col">
          {component === "ProductPage" && <span className="text-gray-500">Price</span>}
          {discountPercent > 0 && (
            <div className="flex items-center gap-2">
              <span className="text-sm opacity-30 line-through">
                ${variant.price}
              </span>
              <span className="bg-red-500 text-white text-xs font-bold px-2 py-1 rounded-full">
                {discountPercent}%
              </span>
            </div>
          )}
          <span className="font-medium text-sm">${finalPrice}</span>
        </div>

        <div className="shadow-md rounded-md cursor-pointer transition-all flex justify-center duration-300">
          {isInCart ? (
            <button
              onClick={() => {
                if (currentCartItem) {
                  removeFromCart(currentCartItem);
                  handleChangeSelectedOption("quantity", 1);
                  toast.info("Item removed from cart");
                }
              }}
              className="hover:text-red-400 shadow-md w-full bg-red-500 hover:bg-gray-900 disabled:bg-gray-400 disabled:cursor-not-allowed text-white p-3 rounded-md cursor-pointer flex items-center justify-center gap-2"
            >
              <ShoppingCart className="w-4 h-4" />
              <span>Remove from Cart</span>
            </button>
          ) : (
            <button
              onClick={handleAddToCart}
              className="shadow-md w-full bg-gray-800 hover:bg-gray-900 disabled:bg-gray-400 disabled:cursor-not-allowed text-white p-3 rounded-md cursor-pointer flex items-center justify-center gap-2"
            >
              <ShoppingCart className="w-4 h-4" />
              <span>Add to Cart</span>
            </button>
          )}
        </div>
      </div>
    );
  };

  return (
    <div className={`flex flex-col ${component === "ProductCard" ? "text-xs gap-2 mt-2" : "text-sm gap-4 mt-4"
      }`}>
      <div className={`${component === "ProductCard"
        ? "flex items-center gap-4"
        : "flex flex-col gap-4"
        }`}>
        <SelectSize />
        <SelectColor />
      </div>
      {component === "ProductPage" && <SelectQuantity />}
      <PriceAndButtons />
    </div>
  );
};

export default ProductInteraction;
