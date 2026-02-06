import React from "react";

const Box = ({ children }: {children: React.ReactNode}) => {
  return (
    <div className="flex flex-col gap-4 lg:flex-row md:gap-12 mt-4 shadow-lg rounded-lg overflow-hidden p-4 bg-white">
      {children}
    </div>
  );
};

export default Box;
