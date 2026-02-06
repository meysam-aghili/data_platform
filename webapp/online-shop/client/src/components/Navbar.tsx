"use client";

import Image from "next/image";
import Link from "next/link";
import { useEffect, useState } from "react";
import SearchBar from "./SearchBar";
import { Home, User, LogIn, LogOut, Package } from "lucide-react";
import ShoppingCartIcon from "./ShoppingCartIcon";
import { shopConfig } from "@/configs";
import Categories from "./Categories";
import useAuthStore from "@/stores/AuthStore";

type UserMenuItemType = {
  label: string;
  href: string;
  needLogin: boolean;
  icon: React.ElementType;
}

const Navbar = () => {

  const {isAuthenticated} = useAuthStore();
  const [showUserMenu, setShowUserMenu] = useState<boolean>(false);
  const userMenuItems: UserMenuItemType[] = [
    { label: "Orders", href: "/orders", needLogin: true, icon: Package },
    { label: "Sign Out", href: "/logout", needLogin: true, icon: LogOut },
    { label: "Sign In", href: "/login", needLogin: false, icon: LogIn },
  ];
  const filteredMenu = userMenuItems.filter((i) => i.needLogin === (isAuthenticated() || false))

  return (
    <nav className="w-full flex items-center justify-between border-b border-gray-200 bg-white py-2 fixed z-999 px-4">
      {/* LEFT */}
      <div className="flex gap-6 items-center">
        <Link href="/" className="flex items-center">
          <Image
            src="/logo.png"
            alt={shopConfig.title}
            width={36}
            height={36}
            className="w-6 h-6 md:w-9 md:h-9"
          />
          <p className="hidden md:block text-md font-medium tracking-wider">
            {shopConfig.title.toUpperCase()}
          </p>
        </Link>
        <span>|</span>
        <Link href={"/products"}>Shop</Link>
        <Categories />
      </div>
      {/* RIGHT */}
      <div className="flex items-center gap-4">
        <SearchBar />
        <Link href="/">
          <Home className="flex items-center justify-center p-1 hover:bg-gray-100 rounded transition-colors cursor-pointer" />
        </Link>
        <ShoppingCartIcon />
        {/* User Menu */}
        <div className="relative">
          <button
            onClick={() => setShowUserMenu(!showUserMenu)}
            className="flex items-center justify-center p-1 hover:bg-gray-100 rounded transition-colors cursor-pointer"
            aria-label="User menu"
          >
            <User className="w-4 h-4 text-gray-600" />
          </button>

          {/* CLICK-OUTSIDE OVERLAY */}
          {showUserMenu && (
            <div
              className="fixed inset-0 z-40"
              onClick={() => setShowUserMenu(false)}
            />
          )}

          {/* Dropdown Menu */}
          {showUserMenu && (
            <div className="absolute right-0 p-2 top-full mt-2 bg-white shadow-md rounded-md border border-gray-200 py-2 min-w-[160px] z-50">

              {filteredMenu.map((item) => {
                const Icon = item.icon;
                return (
                  <Link
                    key={item.label}
                    href={item.href}
                    onClick={() => setShowUserMenu(false)}
                    className="flex border-b last:border-b-0 rounded-md border-b-gray-300 items-center gap-2 px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
                  >
                    <Icon className="w-4 h-4" />
                    {item.label}
                  </Link>
                );
              })}

            </div>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
