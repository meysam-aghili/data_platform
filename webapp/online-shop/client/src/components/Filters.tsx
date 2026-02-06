"use client";

import { useEffect, useMemo, useState } from "react";
import { ChevronDown } from "lucide-react";
import Nouislider from "nouislider-react";
import "nouislider/dist/nouislider.css";
import { useRouter, useSearchParams } from "next/navigation";
import { debounce } from "@/shared/helpers/generalHelper";
import BottomSheet from "./BottomSheet";
import { div } from "framer-motion/client";

type FilterType = "Searchable" | "Color" | "Toggle" | "Slider";

export type FilterItem = {
  type: FilterType;
  key: string;
  title: string;
  data?: string[];
  min?: number;
  max?: number;
};

export default function Filters({ filterItems }: { filterItems: FilterItem[] }) {
  const router = useRouter();
  const searchParams = useSearchParams();

  const initialFilters = useMemo(() => {
    const filters: Record<string, any> = {};

    filterItems.forEach((item) => {
      switch (item.type) {
        case "Slider":
          filters[item.key] = [
            Number(searchParams.get(`min_${item.key}`) ?? item.min ?? 0),
            Number(searchParams.get(`max_${item.key}`) ?? item.max ?? 100),
          ];
          break;

        case "Searchable":
        case "Color":
          filters[item.key] = searchParams.get(item.key)?.split(",") ?? [];
          break;

        case "Toggle":
          filters[item.key] = searchParams.get(item.key) === "true";
          break;

        default:
          filters[item.key] = null;
      }
    });

    return filters;
  }, [searchParams, filterItems]);

  const [filters, setFilters] = useState<Record<string, any>>(initialFilters);
  useEffect(() => (
    setFilters(initialFilters)
  ), [searchParams])

  const updateRoute = (params: Record<string, any>) => {
    const query = new URLSearchParams(window.location.search);

    Object.entries(params).forEach(([key, value]) => {
      if (value === "" || value === null || value === undefined || (Array.isArray(value) && value.length === 0)) {
        query.delete(key);
      } else {
        query.set(key, String(value));
      }
    });

    const newQuery = "?" + query.toString();
    if (newQuery !== window.location.search) {
      router.push(newQuery, { scroll: false });
    }
  };

  const setFilterValue = (key: string, value: any) => {
    const filter = filterItems.find((f) => f.key === key);
    if (!filter) return;

    if (filter.type === "Slider") {
      updateRoute({ [`min_${key}`]: value[0], [`max_${key}`]: value[1] });
    } else if (Array.isArray(value)) {
      updateRoute({ [key]: value.join(",") });
    } else {
      updateRoute({ [key]: value });
    }
  };

  const debouncedSetFilterValue = useMemo(
    () => debounce(setFilterValue, 300),
    []
  );

  const [open, setOpen] = useState(false);

  return (
    <div className="w-full">

      <div className="px-3 mb-3 md:hidden">
        <button
          className="w-full py-2 bg-teal-600 text-white rounded-md"
          onClick={() => setOpen(true)}
        >
          Show Filters
        </button>
      </div>

      <BottomSheet open={open} onClose={() => setOpen(false)}>

        <div className="gap-2 flex-col text-gray-700 p-4">
          <div className="flex items-center justify-between">
            <h2 className="font-semibold text-lg">Filters</h2>
            <button
              className="text-teal-500 text-sm cursor-pointer"
              onClick={() => router.push("?")}
            >
              Remove all filters
            </button>
          </div>

          {filterItems.map((item) => {
            switch (item.type) {
              case "Color":
                return (
                  <Accordion title={item.title} key={item.key}>
                    <Color
                      data={item.data!}
                      initial={filters[item.key]}
                      onChange={(values) => debouncedSetFilterValue(item.key, values)}
                    />
                  </Accordion>
                );

              case "Toggle":
                return (
                  <Toggle
                    label={item.title}
                    key={item.key}
                    initial={filters[item.key]}
                    onChange={(value) => debouncedSetFilterValue(item.key, value)}
                  />
                );

              case "Searchable":
                return (
                  <Accordion title={item.title} key={item.key}>
                    <Searchable
                      data={item.data!}
                      initial={filters[item.key]}
                      onChange={(values) => debouncedSetFilterValue(item.key, values)}
                    />
                  </Accordion>
                );

              case "Slider":
                return (
                  <Accordion title={item.title} key={item.key}>
                    <Slider
                      min={item.min!}
                      max={item.max!}
                      initial={filters[item.key]}
                      onChange={(values) => debouncedSetFilterValue(item.key, values)}
                    />
                  </Accordion>
                );

              default:
                return null;
            }
          })}
        </div>

      </BottomSheet>

      <div className="gap-2 flex-col text-gray-700 hidden md:block">
        <div className="flex items-center justify-between">
          <h2 className="font-semibold text-lg">Filters</h2>
          <button
            className="text-teal-500 text-sm cursor-pointer"
            onClick={() => router.push("?")}
          >
            Remove all filters
          </button>
        </div>

        {filterItems.map((item) => {
          switch (item.type) {
            case "Color":
              return (
                <Accordion title={item.title} key={item.key}>
                  <Color
                    data={item.data!}
                    initial={filters[item.key]}
                    onChange={(values) => debouncedSetFilterValue(item.key, values)}
                  />
                </Accordion>
              );

            case "Toggle":
              return (
                <Toggle
                  label={item.title}
                  key={item.key}
                  initial={filters[item.key]}
                  onChange={(value) => debouncedSetFilterValue(item.key, value)}
                />
              );

            case "Searchable":
              return (
                <Accordion title={item.title} key={item.key}>
                  <Searchable
                    data={item.data!}
                    initial={filters[item.key]}
                    onChange={(values) => debouncedSetFilterValue(item.key, values)}
                  />
                </Accordion>
              );

            case "Slider":
              return (
                <Accordion title={item.title} key={item.key}>
                  <Slider
                    min={item.min!}
                    max={item.max!}
                    initial={filters[item.key]}
                    onChange={(values) => debouncedSetFilterValue(item.key, values)}
                  />
                </Accordion>
              );

            default:
              return null;
          }
        })}
      </div>
    </div>
  );
}

/* ----------------------------- UI COMPONENTS ----------------------------- */

const Accordion = ({ title, children }: { title: string; children: React.ReactNode }) => {
  const [open, setOpen] = useState(false);

  return (
    <div className="border-b border-gray-200 pb-3">
      <button
        onClick={() => setOpen(!open)}
        className="w-full flex items-center justify-between py-3"
      >
        <span>{title}</span>
        <ChevronDown className={`w-5 h-5 transition-transform ${open ? "rotate-180" : ""}`} />
      </button>

      <div className={`transition-all overflow-hidden ${open ? "max-h-[500px]" : "max-h-0"} duration-300`}>
        {children}
      </div>
    </div>
  );
};

const Searchable = ({ data, initial, onChange }: { data: string[]; initial: string[]; onChange: (v: string[]) => void }) => {
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState(initial);

  // sync with prop changes
  useEffect(() => {
    setSelected(initial);
  }, [initial]);

  const toggle = (value: string) => {
    const updated = selected.includes(value) ? selected.filter((x) => x !== value) : [...selected, value];
    setSelected(updated);
    onChange(updated);
  };

  const filtered = data.filter((b) => b.toLowerCase().includes(search.toLowerCase()));

  return (
    <div className="px-2 py-3">
      <input
        type="text"
        placeholder="search..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm mb-3"
      />

      <div className="space-y-3 max-h-48 overflow-auto pr-1">
        {filtered.map((item, idx) => (
          <label key={idx} className="flex items-center justify-between">
            <span>{item}</span>
            <input
              type="checkbox"
              checked={selected.includes(item)}
              className="w-5 h-5 accent-teal-600"
              onChange={() => toggle(item)}
            />
          </label>
        ))}
      </div>
    </div>
  );
};

const Color = ({ data, initial, onChange }: { data: string[]; initial: string[]; onChange: (v: string[]) => void }) => {
  const [selected, setSelected] = useState(initial);

  useEffect(() => {
    setSelected(initial);
  }, [initial]);

  const toggle = (color: string) => {
    const updated = selected.includes(color) ? selected.filter((c) => c !== color) : [...selected, color];
    setSelected(updated);
    onChange(updated);
  };

  return (
    <div className="p-3 flex gap-3">
      {data.map((item) => (
        <span
          key={item}
          onClick={() => toggle(item)}
          className={`w-6 h-6 rounded-md cursor-pointer border-2 ${selected.includes(item) ? "border-black" : "border-transparent"}`}
          style={{ backgroundColor: item }}
        />
      ))}
    </div>
  );
};

function Toggle({ label, initial, onChange }: { label: string; initial: boolean; onChange: (v: boolean) => void }) {
  const [active, setActive] = useState(initial);

  useEffect(() => {
    setActive(initial);
  }, [initial]);

  const toggle = () => {
    const newVal = !active;
    setActive(newVal);
    onChange(newVal);
  };

  return (
    <div className="flex items-center justify-between py-3">
      <button
        onClick={toggle}
        className={`relative w-12 h-6 rounded-full transition duration-300 ${active ? "bg-indigo-600" : "bg-gray-300"}`}
      >
        <span className={`absolute top-1 w-4 h-4 bg-white rounded-full transition duration-300 ${active ? "right-1" : "left-1"}`} />
      </button>
      <span>{label}</span>
    </div>
  );
}

const Slider = ({ min, max, initial, onChange }: { min: number; max: number; initial: number[]; onChange: (v: number[]) => void }) => {
  const [values, setValues] = useState(initial);

  useEffect(() => {
    setValues(initial);
  }, [initial]);

  const handleUpdate = (values: number[]) => {
    const intValues = values.map((v) => Math.round(v)); // convert to integers
    setValues(intValues);
    onChange(intValues);
  };

  return (
    <div className="p-5">
      <Nouislider
        range={{ min, max }}
        start={initial}
        connect
        step={(max - min) / 100}
        onChange={handleUpdate}
      />
      <div className="mt-4 text-sm">
        Selected: {values[0].toLocaleString()} - {values[1].toLocaleString()}
      </div>
    </div>
  );
};
