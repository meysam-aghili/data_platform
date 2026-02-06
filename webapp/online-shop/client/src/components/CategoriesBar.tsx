"use client"

import Link from "next/link"
import Box from "./Box"
import { fetchData } from "@/shared/helpers/apiHelper"
import { useEffect, useState } from "react"


export default function CategoriesBar({ category }: { category: string }) {

    const [data, setData] = useState<string[]>([]);

    useEffect(() => {
        const load = async () => {
            if (category.trim().length > 0) {
                const data = await fetchData<string[]>({ url: `categories/${category}/parent` })
                setData(data);
                return;
            }
            setData([]);
        }
        load()
    }, [category])

    return (
        <Box>
            <div className="w-full flex items-center text-sm text-gray-500 gap-1">

                <Link
                    href="/products"
                    className="hover:text-black transition-colors"
                >
                    Products
                </Link>

                {data?.map((category, i) => (
                    <div key={i} className="flex items-center gap-1">
                        <span className="text-gray-400">/</span>
                        <Link
                            href={`/products?category=${category}`}
                            className="capitalize hover:text-black transition-colors"
                        >
                            {category}
                        </Link>
                    </div>
                ))}
            </div>

        </Box>
    )

}