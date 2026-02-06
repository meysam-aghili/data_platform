"use client";

import { usePathname, useRouter, useSearchParams } from "next/navigation";

const Sort = () => {
  const searchParams = useSearchParams();
  const router = useRouter();
  const pathname = usePathname();
  const defaultSortParam: string = new URLSearchParams(searchParams).get("sort") || "newest";
  const handleFilter = (value: string) => {
    const params = new URLSearchParams(searchParams);
    params.set("sort", value);
    router.push(`${pathname}?${params.toString()}`);
  };

  return (
    <div className="flex items-center justify-end gap-2 text-sm text-gray-500 my-6">
      <span>Sort by:</span>
      <select
        name="sort"
        id="sort"
        className="ring-1 ring-gray-200 shadow-md p-1 rounded-sm"
        onChange={(e) => handleFilter(e.target.value)}
        defaultValue={defaultSortParam}
      >
        <option value="newest">Newest</option>
        <option value="oldest">Oldest</option>
        <option value="cheapest">Cheapest</option>
        <option value="expensive">Expensive</option>
      </select>
    </div>
  );
};

export default Sort;
