import classNames from 'classnames';
import {FC, memo, PropsWithChildren} from 'react';

import { SectionId } from '../models/data';

const Section: FC<
  PropsWithChildren<{sectionId: SectionId; sectionTitle?: string; noPadding?: boolean; className?: string}>
> = memo(({children, sectionId, noPadding = false, className}) => {
  return (
    <section className={classNames(className, {'px-4 py-16': !noPadding})} id={sectionId}>
      <div className={classNames({'mx-auto max-w-screen-lg': !noPadding})}>{children}</div>
    </section>
  );
});

Section.displayName = 'Section';
export default Section;
