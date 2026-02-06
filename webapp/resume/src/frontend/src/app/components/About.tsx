import Image from 'next/image';
import { FC, memo } from 'react';
import { AboutData } from "../models/data";
import Socials from './Socials';
import { SectionId } from '../models/data';
import { ChevronDown } from 'lucide-react';
import classNames from 'classnames';
import Section from './Section';

const About: FC = memo(() => {
  const { backImageSrc, imageSrc, name, description, actions, aboutItems } = AboutData;

  return (
    <>
      <Section noPadding sectionId={SectionId.About}>
        <div className="relative flex md:h-screen lg:h-screen w-full items-center justify-center">
          <Image
            alt={`${name}-image`}
            className="absolute z-0 h-full w-full object-cover"
            placeholder="blur"
            priority
            src={backImageSrc}
            fill
          />
            
          <div className="flex flex-col-reverse mt-12 md:flex-row md:mt-20 lg:flex-row sm:mt-20">
            <div className="lg:pr-28 lg:pl-10 z-10 max-w-screen-lg px-4 lg:px-0 relative flex grow lg:basis-5/6 flex-col items-center gap-y-6 rounded-xl bg-gray-800/40 p-6 text-center shadow-lg backdrop-blur-sm">
              <h1 className="text-4xl font-bold text-white sm:text-5xl lg:text-6xl mt-6 ">{name}</h1>
              {description}
              <ul className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                {aboutItems.map(({ label, text, Icon }, idx) => (
                  <li className="col-span-1 flex  items-start gap-x-2" key={idx}>
                    {Icon && <Icon className="h-5 w-5 text-white" />}
                    <span className="text-sm font-bold text-white">{label}:</span>
                    <span className=" text-sm text-gray-300">{text}</span>
                  </li>
                ))}
              </ul>
              <div className='flex justify-between items-center gap-12'>
                <div className="flex gap-x-4 text-neutral-100">
                  <Socials />
                </div>
                <div className="flex w-full justify-center gap-x-4">
                  {actions.map(({ href, text, primary, Icon }) => (
                    <a
                      className={classNames(
                        'flex gap-x-2 rounded-full border-2 bg-none px-4 py-2 text-sm font-medium text-white ring-offset-gray-700/80 hover:bg-gray-700/80 focus:outline-none focus:ring-2 focus:ring-offset-2 sm:text-base',
                        primary ? 'border-orange-500 ring-orange-500' : 'border-white ring-white',
                      )}
                      href={href}
                      key={text}
                    >
                      {text}
                      {Icon && <Icon className="h-5 w-5 text-white sm:h-6 sm:w-6" />}
                    </a>
                  ))}
                </div>
              </div>
            </div>
            <div className="flex w-full grow items-end gap-x-4 lg:flex-1 lg:basis-1/6">
              <div className="-my-8 flex w-full flex-1 justify-center self-center lg:-ml-24 lg:block  lg:py-16">
                <div
                  className="relative z-20 aspect-square w-32 sm:w-48 md:w-64 overflow-hidden rounded-3xl shadow-xl shadow-black/40 mt-12 ">
                  <Image
                    alt="profile-image"
                    src={imageSrc}
                    className="object-cover"
                    fill
                    priority
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </Section>
    </>
  );
});

About.displayName = 'About';
export default About;
