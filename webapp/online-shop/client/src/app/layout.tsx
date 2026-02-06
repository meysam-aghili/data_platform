import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import Navbar from "@/components/Navbar";
import Footer from "@/components/Footer";
import { ToastContainer } from "react-toastify";
import { shopConfig } from "@/configs";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: shopConfig.telegram,
  description: shopConfig.description,
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased bg-gray-100`}
      >
        <Navbar />
        <div className="mx-auto px-4 sm:px-0 sm:max-w-xl md:max-w-3xl lg:max-w-3xl xl:max-w-340 pt-14">
          {children}
        </div>
        <Footer />
        <ToastContainer position="bottom-right" />
      </body>
    </html>
  );
}
