/* eslint-disable react/self-closing-comp */
import classNames from 'classnames';
import React from 'react';

interface LoadingProps {
  classname?: string;
}

const Loading = ({ classname }: LoadingProps) => {
  return (
    <div
      className={classNames(
        `w-6 h-6 border-2 border-solid border-primary-5 rounded-full
         border-t-transparent animate-spin`,
        classname,
      )}
    ></div>
  );
};

export default Loading;
