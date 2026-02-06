import Box from "@/components/Box";
import CategoriesBar from "@/components/CategoriesBar";
import ProductImageGallery from "@/components/ProductImageGallery";
import ProductInteraction from "@/components/ProductInteraction";
import { fetchData } from "@/shared/helpers/apiHelper";
import { ProductType } from "@/types";

const ProductPage = async ({ params }: { params: Promise<{ id: string }>; }) => {

  const product: ProductType = await fetchData({ url: `products/${(await params).id}` })

  return (
    <>
      <CategoriesBar category={product.category.slug} />
      <Box>
        {/* IMAGE */}
        <div className="w-full lg:w-5/12">
          <ProductImageGallery
            images={product.product_variants.flatMap(pv => pv.images)}
          />
        </div>
        {/* DETAILS */}
        <div className="w-full lg:w-7/12 flex flex-col gap-4 bg-gray-50 rounded-md p-4 h-full">
          <h1 className="text-2xl font-medium">{product.title}</h1>
          <ProductInteraction
            component="ProductPage"
            product={product}
          />
        </div>
      </Box>
      <Box>
        <div className="flex flex-col gap-4 text-gray-600 w-full">
          <span className="font-semibold text-lg">Description:</span>
          <div className="bg-gray-100 rounded-md p-4">
            <span>{product.description}</span>
          </div>

        </div>
      </Box>
    </>
  );
};

export default ProductPage;
