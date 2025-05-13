/* eslint-disable react/self-closing-comp */
import classNames from 'classnames';
import React, { ButtonHTMLAttributes, PropsWithChildren } from 'react';

interface ButtonPrimaryProps extends ButtonHTMLAttributes<HTMLButtonElement>, PropsWithChildren {
  loading?: boolean;
}

const ButtonPrimary = ({ children, loading, ...rest }: ButtonPrimaryProps) => {
  return (
    <button
      {...rest}
      className={classNames(
        `px-4 py-2 rounded-md bg-primary  text-sm text-slate-50 font-medium flex items-center`,
        rest.className,
        {
          'opacity-50 cursor-not-allowed pointer-events-none': rest.disabled,
        },
      )}
    >
      {loading ? (
        <div
          className="w-3 h-3 rounded-full border border-solid border-t-transparent border-zinc-50
         animate-spin mr-2"
        ></div>
      ) : null}
      {children}
    </button>
  );
};

export default ButtonPrimary;
