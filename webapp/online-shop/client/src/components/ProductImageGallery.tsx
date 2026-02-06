"use client";

import { ImageType } from "@/types";
import { Expand, Minimize } from "lucide-react";
import Image from "next/image";
import { useEffect, useState } from "react";

export default function ProductImageGallery({ images }: { images: ImageType[]; }) {

    const [selected, setSelected] = useState(0);
    const [isFullscreen, setIsFullscreen] = useState(false);

    // Disable page scroll when fullscreen is open
    useEffect(() => {
        if (isFullscreen) {
            document.body.style.overflow = "hidden";
        } else {
            document.body.style.overflow = "";
        }
    }, [isFullscreen]);

    return (
        <div className="flex flex-col gap-4">
            {/* Main Image */}
            <div className="relative w-full h-[500px] cursor-pointer" onClick={() => setIsFullscreen(true)}>
                {/* Expand Button */}
                <button
                    className="z-1 cursor-pointer absolute top-2 right-8 bg-white/80 hover:bg-white text-black p-2 rounded-full shadow-lg"
                    onClick={() => setIsFullscreen(true)}
                >
                    <Expand />
                </button>
                <Image
                    src={images[selected].url}
                    alt={images[selected].title}
                    fill
                    className="object-contain rounded"
                />
            </div>

            {/* Thumbnails */}
            <div className="flex gap-3 overflow-x-auto">
                {images.map((img, idx) => (
                    <button
                        key={idx}
                        onClick={() => setSelected(idx)}
                        className={`relative w-20 h-20 rounded border-2
                            ${idx === selected ? "border-gray-500" : "border-gray-100"}`}
                    >
                        <Image
                            src={img.url}
                            alt={`${img.title} ${idx}`}
                            fill
                            className="object-cover rounded"
                        />
                    </button>
                ))}
            </div>

            {/* FULLSCREEN */}
            {isFullscreen && (
                <div className="fixed inset-0 z-1000 flex items-center justify-center bg-black bg-opacity-95">

                    {/* Close Button */}
                    <button
                        className="z-1001 cursor-pointer absolute top-5 right-5 bg-white/80 hover:bg-white text-black p-2 rounded-full shadow-lg"
                        onClick={() => setIsFullscreen(false)}
                    >
                        <Minimize />
                    </button>

                    {/* Fullscreen Image */}
                    <div className="relative w-full h-full max-w-6xl max-h-[90vh]">
                        <Image
                            src={images[selected].url}
                            alt={images[selected].title}
                            fill
                            className="object-contain cursor-pointer"
                            onClick={() => setIsFullscreen(false)}
                        />
                    </div>
                </div>
            )}
        </div>
    );
}
