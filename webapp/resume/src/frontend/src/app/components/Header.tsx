"use client"
import React, { memo, useEffect, useMemo, useState, type FC } from 'react';
import { SectionId } from '../models/data';
import Link from 'next/link';

const Header: FC = () => {
  const [currentSection, setCurrentSection] = useState<SectionId | null>(null);

  const navSections = useMemo(
    () => [
      SectionId.About,
      SectionId.Resume,
      SectionId.Portfolio
    ],
    []
  );

  useEffect(() => {
    const handleScroll = () => {
      let foundSection: SectionId | null = null;

      for (const section of navSections) {
        const el = document.getElementById(section);
        if (!el) continue;

        const rect = el.getBoundingClientRect();
        // Check if top of section is near top of viewport
        if (rect.top <= 100 && rect.bottom >= 100) {
          foundSection = section;
          break;
        }
      }

      setCurrentSection(foundSection);
    };
    
    window.addEventListener("scroll", handleScroll, { passive: true });
    handleScroll();

    return () => window.removeEventListener("scroll", handleScroll);
  }, [navSections]);

  return (
    <header className="fixed top-0 z-50 w-full bg-neutral-900/50 p-4 backdrop-blur">
      <nav className="flex justify-center gap-x-4 sm:gap-x-8">
        {navSections.map((section) => (
          <Link
            key={section}
            href={`/#${section}`}
            className={
              section === currentSection
                ? 'text-orange-500'
                : 'text-neutral-100'
            }
          >
            {section}
          </Link>
        ))}
      </nav>
    </header>
  );
};

export default memo(Header);
