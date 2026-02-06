import { ProductType } from "@/types";
import Image from "next/image";
import Link from "next/link";

const ProductSearchCard = ({ product }: { product: ProductType }) => {

    return (
        <Link href={`/products/${product.slug}`} className="sm:w-full">
            <div className="shadow-lg rounded-md overflow-hidden flex flex-row gap-2">
                <div className="relative aspect-square md:w-2/8 w-3/8">
                    <Image
                        src={product.product_variants[0].images[0].url}
                        alt={product.title}
                        fill
                        className="object-cover"
                    />
                </div>

                <div className="flex flex-col p-2 justify-around md:w-6/8 w-5/8">
                    <h1 className="font-semibold text-xs">{product.title}</h1>
                </div>
            </div>
        </Link >
    );
};

export default ProductSearchCard;
