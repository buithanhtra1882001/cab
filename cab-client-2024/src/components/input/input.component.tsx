import React, { HTMLInputTypeAttribute, InputHTMLAttributes, memo, useState } from 'react';
import { UseFormRegisterReturn } from 'react-hook-form';
import _toString from 'lodash/toString';
import { Size } from '../../types/common';
import classnames from 'classnames';
import { Eye, EyeOff } from 'lucide-react';

interface IInputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'size'> {
  size?: Size;
  register?: UseFormRegisterReturn;
  isError?: boolean;
}

const Input: React.FC<IInputProps> = (props) => {
  const { size = 'md', register, isError = false, ...rest } = props;

  const [type, setType] = useState<HTMLInputTypeAttribute>(rest.type || 'text');

  const getSize = (): string => {
    const sizeObj: Record<Size, string> = {
      xs: 'py-1',
      sm: 'py-2',
      md: 'py-3',
    };

    return sizeObj[size];
  };

  return (
    <div className="relative w-full font-normal">
      {rest.type && rest.type === 'textarea' ? (
        <textarea
          {...register}
          className={classnames(
            [
              `outline-none border border-solid rounded-md w-full text-sm px-4
            transition-all duration-200 placeholder:text-sm dark:bg-zinc-800`,
              getSize(),
              _toString(rest.className),
            ],
            {
              'border-red-500 focus:border-red-500 hover:border-red-500 placeholder:text-red-500 text-red-500': isError,
              'border-gray-5 dark:border-slate-50/10  placeholder:text-slate-400 text-slate-800 dark:text-zinc-50':
                !isError,
              'pr-10': type === 'password',
              'pointer-events-none': rest.readOnly,
            },
          )}
        />
      ) : (
        <input
          {...rest}
          {...register}
          type={type}
          className={classnames(
            [
              `outline-none border border-solid rounded-md w-full text-sm px-4
          transition-all duration-200 placeholder:text-sm dark:bg-zinc-800`,
              getSize(),
              _toString(rest.className),
            ],
            {
              'border-red-500 focus:border-red-500 hover:border-red-500 placeholder:text-red-500 text-red-500': isError,
              'border-gray-5 dark:border-slate-50/10  placeholder:text-slate-400 text-slate-800 dark:text-zinc-50':
                !isError,
              'pr-10': type === 'password',
              'pointer-events-none': rest.readOnly,
            },
          )}
        />
      )}

      {rest.type && rest.type === 'password' && (
        <div>
          {type === 'password' ? (
            <Eye
              className={classnames(`absolute right-4 top-1/2 transform -translate-y-1/2`, {
                'text-red-500': isError,
                'text-slate-500 dark:text-slate-50': !isError,
              })}
              onClick={() => {
                setType((prev) => {
                  return prev === 'password' ? 'text' : 'password';
                });
              }}
              size={16}
            />
          ) : (
            <EyeOff
              className={classnames(`absolute right-4 top-1/2 transform -translate-y-1/2`, {
                'text-red-500': isError,
                'text-slate-500 dark:text-slate-50': !isError,
              })}
              onClick={() => {
                setType((prev) => {
                  return prev === 'password' ? 'text' : 'password';
                });
              }}
              size={16}
            />
          )}
        </div>
      )}
    </div>
  );
};

export default memo(Input);
