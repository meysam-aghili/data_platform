import { FC, ForwardRefExoticComponent, JSX } from "react";
import { LucideProps } from "lucide-react";
import { StaticImageData } from "next/image";
import { SectionId } from "./data";


interface AboutActionItem {
  href: string;
  text: string;
  primary?: boolean;
  Icon?: ForwardRefExoticComponent<Omit<LucideProps, 'ref'>>;
}

interface AboutItem {
  label: string;
  text: string;
  Icon?: ForwardRefExoticComponent<Omit<LucideProps, 'ref'>>;
}

export interface About {
  imageSrc: string | StaticImageData;
  backImageSrc: string | StaticImageData;
  name: string;
  description: JSX.Element;
  actions: AboutActionItem[];
  aboutItems: AboutItem[];
}

export interface IconProps extends React.HTMLAttributes<SVGSVGElement> {
  svgRef?: React.Ref<SVGSVGElement>;
  transform?: string;
}

export interface Social {
  label: string;
  Icon: FC<IconProps>;
  href: string;
}

export interface PortfolioItem {
  title: string;
  description: string;
  url: string;
  image: string | StaticImageData;
}

interface Skill {
  title: string;
  level: number;
}

export interface SkillGroup {
  title: string;
  items: Skill[];
}

export interface SkillGroupV2 {
  experiencedIn: string[];
  experimentedWith: string[];
  interestedIn: string[];
}

export interface ResumeHistoryItem {
  timeWindow: string;
  location: string;
  title: string;
  description: JSX.Element;
};