import { ProductType } from "@/types";
import Image from "next/image";
import Link from "next/link";
import ProductInteraction from "./ProductInteraction";

const ProductCard = ({ product }: { product: ProductType }) => {

  return (
    <div className="shadow-lg rounded-md overflow-hidden flex flex-row gap-2 sm:flex-col">

      <Link href={`/products/${product.slug}`} className="sm:w-full">
        <div className="relative aspect-square w-32 sm:w-full">
          <Image
            src={product.product_variants[0]?.images[0]?.url || "/no_image_placeholder.svg.png"}
            alt={product.title}
            fill
            className="object-cover hover:scale-105 transition-all duration-300"
          />
        </div>
      </Link>

      <div className="flex flex-col p-2 justify-around w-full h-full">
        <h1 className="font-semibold text-xs">{product.title}</h1>
        <ProductInteraction
          component="ProductCard"
          product={product}
        />
      </div>
    </div>
  );
};

export default ProductCard;
