import ProductCard from "./ProductCard";
import Link from "next/link";
import Sort from "./Sort";
import { fetchData } from "@/shared/helpers/apiHelper";
import Filters, { FilterItem } from "./Filters";
import Box from "./Box";
import { PagedResultType, ProductType } from "@/types";
import CategoriesBar from "./CategoriesBar";

export interface ProductListProps {
  category?: string;
  color?: string;
  brand?: string;
  in_stock?: string;
  min_price?: string;
  max_price?: string;
  sort?: string;
  search_slug?: string;
  page_number?: string;
  page_size?: string;
}

const ProductList = async ({
  category,
  color,
  brand,
  in_stock,
  min_price,
  max_price,
  sort,
  search_slug,
  page_number,
  page_size
}: ProductListProps) => {

  const query: Record<string, string> = {};

  if (category) query.category = category;
  if (color) query.color = color;
  if (brand) query.brand = brand;
  if (in_stock) query.in_stock = in_stock;
  if (min_price) query.min_price = min_price;
  if (max_price) query.max_price = max_price;
  if (search_slug) query.search_slug = search_slug;
  if (page_number) query.page_number = page_number;
  if (page_size) query.page_size = page_size;

  if (sort) {
    switch (sort) {
      case "oldest":
        query.sort_by = "created_at";
        query.sort_direction = "0";
        break;
      case "newest":
        query.sort_by = "created_at";
        query.sort_direction = "1";
        break;
      case "cheapest":
        query.sort_by = "price";
        query.sort_direction = "0";
        break;
      case "expensive":
        query.sort_by = "price";
        query.sort_direction = "1";
        break;
      default:
        break;
    }
  };

  const [products, brands, colors]: [PagedResultType<ProductType>, string[], string[]] = await Promise.all([
    fetchData<PagedResultType<ProductType>>({ url: "products", params: query }),
    fetchData<string[]>({ url: "brands" }),
    fetchData<string[]>({ url: "colors" }),
  ]);

  const filterItems: FilterItem[] = [
    { type: "Slider", key: "price", title: "Price", min: 0, max: 20000000 },
    {
      type: "Searchable",
      key: "brand",
      title: "Brand",
      data: brands,
    },
    { type: "Color", key: "color", title: "Color", data: colors },
    { type: "Toggle", key: "in_stock", title: "In Stock" },
  ];

  const totalPages = Math.ceil(products.total_count / products.page_size);

  const createPageUrl = (page: number) => {
    const params = new URLSearchParams(query);
    params.set("page_number", page.toString());
    return `/products?${params.toString()}`;
  };

  return (
    <div className="flex flex-col md:flex-row md:gap-4 w-full">
      <Box>
        <Filters filterItems={filterItems} />
      </Box>
      <div className="w-full flex flex-col">
        <CategoriesBar category={category ?? ""} />
        <Box>
          {!products.data || products.data.length === 0 ?
            <p className="p-10 flex w-full justify-center">No items exists. Please change the filters.</p> :
            <div className="flex flex-col w-full px-2 gap-2">
              <div className="w-full px-2 sm:px-0">
                <Sort />
                <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
                  {products.data.map((product) => (
                    <ProductCard key={product.id} product={product} />
                  ))}
                </div>
              </div>
              <div className="flex justify-center items-center gap-3 mt-6">
                {/* Prev button */}
                {products.page_number !== 1 &&
                  <Link
                    href={createPageUrl(products.page_number - 1)}
                    className="px-3 py-1 border border-gray-300 rounded-md shadow-md"
                  >
                    Prev
                  </Link>
                }

                {/* First page */}
                <Link
                  href={createPageUrl(1)}
                  className={`px-3 py-1 border border-gray-300 rounded-md shadow-md ${products.page_number === 1 ? "bg-teal-500 text-white" : ""}`}
                >
                  1
                </Link>

                {/* Ellipsis before current page if needed */}
                {products.page_number > 2 && <span className="px-1">...</span>}

                {/* Current page (if not first or last) */}
                {products.page_number !== 1 && products.page_number !== totalPages && (
                  <Link
                    href={createPageUrl(products.page_number)}
                    className="px-3 py-1 border border-gray-300 rounded-md shadow-md bg-teal-500 text-white"
                  >
                    {products.page_number}
                  </Link>
                )}

                {/* Ellipsis after current page if needed */}
                {products.page_number < totalPages - 1 && <span className="px-1">...</span>}

                {/* Last page */}
                {totalPages > 1 && (
                  <Link
                    href={createPageUrl(totalPages)}
                    className={`px-3 py-1 border border-gray-300 rounded-md shadow-md ${products.page_number === totalPages ? "bg-teal-500 text-white" : ""}`}
                  >
                    {totalPages}
                  </Link>
                )}

                {/* Next button */}
                {products.page_number < totalPages && (
                  <Link
                    href={createPageUrl(products.page_number + 1)}
                    className="px-3 py-1 border border-gray-300 rounded-md shadow-md"
                  >
                    Next
                  </Link>
                )}
              </div>
            </div>
          }
        </Box>
      </div>
    </div>
  );
};

export default ProductList;
