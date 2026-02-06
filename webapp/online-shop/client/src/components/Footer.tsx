import { shopConfig } from "@/configs";
import Image from "next/image";
import Link from "next/link";

const Footer = () => {
  return (
    <div className="mt-16 flex flex-col items-center gap-8 md:flex-row md:items-start md:justify-between md:gap-0 bg-gray-800 p-8">
      <div className="flex flex-col gap-4 items-center md:items-start">
        <Link href="/" className="flex items-center">
          <Image src="/logo.png" alt="TrendLama" width={36} height={36} />
          <p className="hidden md:block text-md font-medium tracking-wider text-white">
            {shopConfig.title.toUpperCase()}
          </p>
        </Link>
        <p className="text-sm text-gray-400">Support phone: {shopConfig.supportPhone}</p>
        <p className="text-sm text-gray-400 w-90">Address: <br />{shopConfig.address}</p>
        <p className="text-sm text-gray-400">Copyright © {new Date().getFullYear()} {shopConfig.url}</p>
      </div>
      <div className="flex flex-col gap-4 text-sm text-gray-400 items-center md:items-start">
        <p className="text-sm text-amber-50">Customer Service</p>
        <Link href="/">FAQ</Link>
        <Link href="/browse/return-policy">Return Policy</Link>
        <Link href="/browse/terms-of-service">Terms of Service</Link>
        <Link href="/browse/privacy-policy">Privacy Policy</Link>
      </div>
      <div className="flex flex-col gap-4 text-sm text-gray-400 items-center md:items-start">
        <p className="text-sm text-amber-50">Information</p>
        <Link href="/browse/about">About</Link>
        <Link href="/browse/contact-us">Contact Us</Link>
      </div>
      <div className="flex flex-col gap-4 text-sm text-gray-400 items-center md:items-start">
        <p className="text-sm text-amber-50">Links</p>
        <div className="flex gap-2">
        <Link href={shopConfig.instagram}>
          <svg className="text-gray-400" xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" fill="none" viewBox="0 0 24 24" data-testid="svg_IconInstagram">
            <path fill="currentColor" fillRule="evenodd" d="M7 12a5 5 0 1 1 10 0 5 5 0 0 1-10 0m5-3a3 3 0 1 0 0 6 3 3 0 0 0 0-6" clipRule="evenodd"></path>
            <path fill="currentColor" d="M17.25 8a1.25 1.25 0 1 0 0-2.5 1.25 1.25 0 0 0 0 2.5"></path>
            <path fill="currentColor" fillRule="evenodd" d="M7.858 2.07c-1.064.05-1.79.22-2.425.47-.658.256-1.215.6-1.77 1.156a4.9 4.9 0 0 0-1.15 1.772c-.246.637-.413 1.364-.46 2.429s-.057 1.407-.052 4.122c.005 2.716.017 3.056.069 4.123.05 1.064.22 1.79.47 2.426.256.657.6 1.214 1.156 1.769a4.9 4.9 0 0 0 1.774 1.15c.636.245 1.363.413 2.428.46 1.064.046 1.407.057 4.122.052s3.056-.017 4.123-.068c1.552-.074 3.073-.5 4.194-1.626 1.121-1.127 1.542-2.647 1.61-4.2.046-1.068.057-1.409.052-4.124s-.018-3.056-.068-4.122c-.074-1.554-.5-3.072-1.626-4.196-1.125-1.122-2.648-1.542-4.201-1.61-1.065-.045-1.407-.057-4.123-.052s-3.056.017-4.123.069m.098 1.998h-.003c-.876.041-1.383.174-1.789.333l-.005.002a2.9 2.9 0 0 0-1.079.705c-.345.347-.54.664-.701 1.081-.158.409-.289.919-.328 1.796-.044 1.018-.055 1.324-.05 4.03s.017 3.01.067 4.03v.002c.042.876.174 1.384.333 1.79l.002.003c.162.417.358.734.705 1.08a2.9 2.9 0 0 0 1.082.7c.41.159.92.29 1.795.328 1.019.045 1.326.056 4.03.05 2.706-.004 3.012-.016 4.033-.065 1.281-.06 2.235-.4 2.871-1.04.638-.64.974-1.595 1.03-2.876.044-1.021.055-1.327.05-4.032s-.017-3.01-.066-4.031c-.06-1.282-.4-2.236-1.04-2.875-.638-.636-1.594-.972-2.876-1.027-1.02-.044-1.328-.056-4.032-.05-2.707.004-3.01.016-4.03.066" clipRule="evenodd"></path>
          </svg>
        </Link>
        <Link href={shopConfig.telegram}>
          <svg className="text-gray-400" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
            <path fill="currentColor" d="M9.964 15.568 9.8 19.406c.344 0 .492-.148.672-.324l3.217-3.073 3.34 2.447c.612.337 1.044.16 1.206-.566l2.185-10.24.002-.002c.194-.907-.328-1.264-.92-1.04L2.63 10.36c-.88.342-.867.834-.15 1.06l4.86 1.516 11.278-7.1c.53-.36 1.014-.162.616.197" />
          </svg>
        </Link>
        </div>
      </div>
    </div>
  );
};

export default Footer;
