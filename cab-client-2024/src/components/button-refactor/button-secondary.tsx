import classNames from 'classnames';
import React, { ButtonHTMLAttributes, PropsWithChildren } from 'react';

interface ButtonSecondaryProps extends ButtonHTMLAttributes<HTMLButtonElement>, PropsWithChildren {
  loading?: boolean;
}

const ButtonSecondary = ({ children, ...rest }: ButtonSecondaryProps) => {
  return (
    <button
      {...rest}
      className={classNames(
        `px-4 py-2 rounded-md border border-solid border-zinc-800 text-sm text-slate-800 dark:text-slate-800 font-medium bg-slate-50`,
        rest.className,
        {
          'opacity-50 cursor-not-allowed pointer-events-none': rest.disabled,
        },
      )}
    >
      {children}
    </button>
  );
};

export default ButtonSecondary;
