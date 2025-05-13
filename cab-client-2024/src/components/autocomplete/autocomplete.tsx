import React, { useState, useRef, useEffect } from 'react';
import InputComponent from '../input/input.component';
import classNames from 'classnames';
import { useOnClickOutside } from 'usehooks-ts';
import * as yup from 'yup';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import _debounce from 'lodash/debounce';

export interface AutocompleteItem {
  label: string;
  value: string;
}

interface AutocompleteProps {
  items: AutocompleteItem[];
  placeholder?: string;
  onChange?: (value: string | string[]) => void;
  onNoneItem?: (inputValue: string) => void;
}

interface FormValues {
  keyword?: string;
}

const schema = yup.object().shape({
  keyword: yup.string(),
});

const Autocomplete = ({ items, onChange, placeholder, onNoneItem }: AutocompleteProps) => {
  const { register, getValues, watch, setValue } = useForm<FormValues>({
    resolver: yupResolver(schema),
  });

  const selfRef = useRef<HTMLDivElement>(null);

  const [visible, setVisible] = useState<boolean>(false);

  const searchItems = _debounce(() => {
    onNoneItem?.(getValues('keyword') || '');
  }, 300);

  useEffect(() => {
    const subscription = watch((formValues) => {
      onChange?.(formValues.keyword || '');
      searchItems();
    });

    return () => {
      subscription.unsubscribe();
    };
  }, []);

  useOnClickOutside(selfRef, () => {
    setVisible(false);
  });

  return (
    <div className="relative" onClick={() => setVisible((prev) => !prev)} ref={selfRef}>
      <InputComponent register={register('keyword')} size="sm" placeholder={placeholder} />

      {items.length ? (
        <ul
          className={classNames(
            `absolute top-full min-w-max p-4 rounded-md bg-zinc-50 dark:bg-zinc-700 shadow-md w-full
           transition-all duration-100 space-y-3 z-50`,
            {
              'opacity-0 invisible': !visible,
              'opacity-100 visible': visible,
            },
          )}
        >
          {items.map((data) => (
            <li
              key={data.value}
              onClick={() => {
                setValue('keyword', data.label);
              }}
              className="text-xs text-slate-600 dark:text-slate-50 cursor-pointer hover:text-slate-800
              transition-all duration-100"
            >
              {data.label}
            </li>
          ))}
        </ul>
      ) : null}
    </div>
  );
};

export default Autocomplete;
