"use client";

import { fetchData } from "@/shared/helpers/apiHelper";
import { debounce } from "@/shared/helpers/generalHelper";
import { ProductType } from "@/types";
import { Search } from "lucide-react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useCallback, useEffect, useMemo, useState } from "react";
import { toast } from "react-toastify";
import ProductSearchCard from "./ProductSearchCard";

const SearchBar = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const pathname = usePathname()

  const [searchSlug, setSearchSlug] = useState(new URLSearchParams(searchParams.toString()).get("search_slug") || "");
  const [showPanel, setShowPanel] = useState(false);
  const [data, setData] = useState<ProductType[]>([]);

  useEffect(() => (setShowPanel(false)),[pathname])

  const fetchResults = useCallback(async (searchSlug: string) => {
    if (searchSlug.length <= 1) {
      setShowPanel(false);
      setData([]);
      return;
    }

    try {
      const data = await fetchData<ProductType[]>({
        url: "products",
        params: { search_slug: searchSlug.trim().toLowerCase() }
      });

      setData(data.slice(0, 5));
      setShowPanel(true);
    } catch (err) {
      toast.error(`Search error: ${err}`);
    }
  }, [searchSlug]);

  const debouncedFetch = useMemo(
    () => debounce(fetchResults, 400),
    [fetchResults]
  );

  const handleInputChange = (value: string) => { 
    setSearchSlug(value);
    debouncedFetch(value);
  };

  const handleSearch = useCallback(() => {
    const params = new URLSearchParams(searchParams.toString());

    if (searchSlug) params.set("search_slug", searchSlug.trim().toLowerCase());
    else params.delete("search_slug");

    const nextUrl = `/products?${params.toString()}`;

    if (nextUrl !== window.location.search) {
      setShowPanel(false);
      router.push(nextUrl);
    }
  }, [searchSlug, searchParams, router]);

  return (
    <div className="relative w-full md:w-100" >

      {showPanel && data.length > 0 && (
        <div
          className="fixed inset-0 bg-black/15 z-40"
          onClick={() => setShowPanel(false)}
        />
      )}

      {/* Search Input */}
      <div className="flex items-center gap-2 rounded-md ring-1 ring-gray-200 px-2 py-1 shadow-md">
        <Search className="w-4 h-4 text-gray-500" />
        <input
          id="search"
          placeholder="Search..."
          className="text-sm outline-0 w-full"
          value={searchSlug}
          onChange={(e) => handleInputChange(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSearch()}
          // onFocus={(e) => data.length > 0 && setShowPanel(true)}
        />
      </div>

      {/* Results Panel */}
      {showPanel && data.length > 0 && (
        <div className="absolute left-0 right-0 mt-2 bg-white shadow-lg rounded-md ring-1 ring-gray-200 p-3 z-50">
          {data.map((product, i) => (
            <div
              key={i}
              className="py-1 px-2 hover:bg-gray-100 rounded cursor-pointer"
            >
              <ProductSearchCard product={product} />
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default SearchBar;
