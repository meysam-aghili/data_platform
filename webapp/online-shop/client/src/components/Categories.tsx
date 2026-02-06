"use client";

import { fetchData } from "@/shared/helpers/apiHelper";
import { CategoryType } from "@/types";
import Link from "next/link";
import { useState, useCallback, useEffect } from "react";

export default function Categories() {
  const [activeCategories, setActiveCategories] = useState<number[]>([]);
  const [data, setData] = useState<CategoryType[]>([]);


  useEffect(() => {
    const load = async () => {
      const data = await fetchData<CategoryType[]>({ url: "categories" })
      setData(data);
    };
    load();
  }, []);

  const handleHover = useCallback((level: number, index: number) => {
    setActiveCategories(prev => {
      const updated = prev.slice(0, level);
      updated[level] = index;
      return updated;
    });
  }, []);

  const getLevelCategories = (level: number): CategoryType[] => {
    let cats = data;
    for (let i = 0; i < level; i++) {
      const idx = activeCategories[i];
      if (!cats[idx]) return [];
      cats = cats[idx].children;
    }
    return cats;
  };

  return (
    <div className="relative group text-gray-600">
      <button className="p-2 text-gray-700 hover:text-red-600 font-medium transition-colors">
        Categories
      </button>

      {/* Dropdown Container */}
      <div className="absolute pointer-events-none opacity-0 top-full left-0
        bg-gray-50 shadow-xl rounded-md group-hover:flex group-hover:opacity-100 group-hover:pointer-events-auto 
          transition-all duration-200 p-6 gap-6 z-50">
        {Array.from({ length: 3 }).map((_, level) => {
          const categories = getLevelCategories(level);
          if (!categories.length) return null;
          return (
            <LevelList
              key={level}
              categories={categories}
              level={level}
              onHover={handleHover}
              activeCategories={activeCategories}
            />
          );
        })}
      </div>
    </div>
  );
}

function LevelList({
  categories,
  level,
  onHover,
  activeCategories
}: {
  categories: CategoryType[];
  level: number;
  onHover: (level: number, index: number) => void;
  activeCategories: number[]
}) {
  return (
    <ul className="w-full border-l pl-4 space-y-1 border-gray-300 flex flex-col gap-1">
      {categories.map((category, index) => (
        <Link key={index} href={`/products?category=${category.slug}`}>
          <li
            onMouseEnter={() => onHover(level, index)}
            className={`min-w-30 p-1.5 rounded-md cursor-pointer transition-all tracking-wide hover:bg-red-50 whitespace-nowrap w-full
            ${index === activeCategories[level] ? "font-semibold bg-red-50 border-l-4 border-red-500 pl-3" : ""}`}
          >
            {category.title}
          </li>
        </Link>
      ))}
    </ul>
  );
}
