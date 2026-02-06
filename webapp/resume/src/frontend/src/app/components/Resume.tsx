import { FC, memo } from 'react';
import { educations, experiences, SectionId, skillItemsV2 } from '../models/data';
import Section from './Section';
import { skillItems } from '../models/data';
import { ResumeHistoryItem } from '../models/interfaces';

const ResumeSkillV2: FC = memo(() => {
  return (
    <div>
      <h2 className="mb-6 text-3xl font-extrabold text-green-400">
        Core Tech Stack
      </h2>

      <div className="space-y-8">
        {/* Experienced In */}
        <div>
          <h3 className="text-xl font-bold text-white mb-3 flex items-center gap-2">
            <span className="w-2 h-2 bg-orange-500 rounded-full"></span>
            Experienced In
          </h3>
          <div className="flex flex-wrap gap-2 ml-6">
            {skillItemsV2.experiencedIn.map((item) => (
              <span
                key={item}
                className="px-3 py-1 text-md rounded-full border border-green-400/30 text-gray-300"
              >
                {item}
              </span>
            ))}
          </div>
        </div>

        {/* Experimented With */}
        <div>
          <h3 className="text-xl font-bold text-white mb-3 flex items-center gap-2">
            <span className="w-2 h-2 bg-orange-500 rounded-full"></span>
            Experimented With
          </h3>
          <div className="flex flex-wrap gap-2 ml-6">
            {skillItemsV2.experimentedWith.map((item) => (
              <span
                key={item}
                className="px-3 py-1 text-md rounded-full border border-green-400/30 text-gray-300"
              >
                {item}
              </span>
            ))}
          </div>
        </div>

        {/* Interested In */}
        <div>
          <h3 className="text-xl font-bold text-white mb-3 flex items-center gap-2">
            <span className="w-2 h-2 bg-orange-500 rounded-full"></span>
            Interested In
          </h3>
          <div className="flex flex-wrap gap-2 ml-6">
            {skillItemsV2.interestedIn.map((item) => (
              <span
                key={item}
                className="px-3 py-1 text-md rounded-full border border-green-400/30 text-gray-300"
              >
                {item}
              </span>
            ))}
          </div>
        </div>
      </div>
    </div>

  )
})

const ResumeSkill: FC = memo(() => {
  const maxLevel = 10;

  return (
    <div>
      {/* Section Title */}
      <h2 className="mb-6 text-3xl font-extrabold text-green-400">
        Core Tech Stack
      </h2>

      {/* Two-column layout */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {skillItems.map((group) => (
          <div key={group.title}>
            {/* Group title */}
            <h3 className="text-xl font-bold text-white mb-2">{group.title}</h3>

            {/* Skill rows */}
            <div className="space-y-2">
              {group.items.map((skill) => (
                <div key={skill.title} className="flex flex-col sm:flex-row sm:items-center">
                  <span className="text-sm font-medium text-gray-300 mb-1 w-40">
                    {skill.title}
                  </span>

                  <div className="flex space-x-1">
                    {Array.from({ length: maxLevel }).map((_, i) => (
                      <div
                        key={i}
                        className={`h-2 w-5 rounded-sm ${i < skill.level ? 'bg-green-600' : 'bg-gray-700'
                          }`}
                      />
                    ))}
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
})

interface ResumeHistoryProps {
  title: string;
  items: ResumeHistoryItem[];
}

const ResumeHistory: FC<ResumeHistoryProps> = memo(({ title, items }) => {
  return (
    <div className='mb-6'>
      {/* Section Title */}
      <h2 className="mb-6 text-3xl font-extrabold text-green-400">
        {title}
      </h2>

      <ul>
        {items.map((item) => (
          <li key={item.timeWindow} className="relative border-b border-l border-gray-700 py-4 pl-4 last:border-b-0">
            <div className=" absolute -left-2 top-3 flex items-center space-x-2 md:-left-4">
              <span className="whitespace-nowrap rounded-full border border-green-700 bg-gray-950 px-2 py-1 text-xs font-bold text-white shadow-xl shadow-black/30">
                {item.timeWindow}
              </span>
              <span className="text-xs font-medium text-gray-200">
                {item.location}
              </span>
            </div>
            <div className="prose prose-sm flex flex-col pt-10 prose-ul:list-none">
              <h2 className="font-bold text-orange-500 mb-2">{item.title}</h2>
              {item.description}
            </div>
          </li>
        ))}
      </ul>
    </div>
  )
})

const Resume: FC = memo(() => {
  return (
    <Section
      sectionId={SectionId.Resume}
      className="bg-neutral-800"
    >
      <ResumeHistory title='Experiences' items={experiences} />
      <ResumeHistory title='Education' items={educations} />
      <ResumeSkillV2 />
    </Section>
  );
});

Resume.displayName = 'Resume';
export default Resume;
