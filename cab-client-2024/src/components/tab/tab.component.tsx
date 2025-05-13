import classNames from 'classnames';
import React, { useState } from 'react';

export interface ITab {
  label: string;
  value: string;
}

interface ITabProps {
  items: ITab[];
  defaultValue?: string;
  onChange: (value: string) => void;
}

export const Tab = ({ items, defaultValue, onChange }: ITabProps) => {
  const [value, setValue] = useState<string>(defaultValue || '');

  const handleChange = (tabValue: string) => {
    setValue(tabValue);
    onChange(tabValue);
  };

  return (
    <ul className="flex items-center flex-wrap gap-3">
      {items.map((tab) => (
        <li
          key={tab.value}
          onClick={() => {
            handleChange(tab.value);
          }}
          className={classNames(`font-medium pb-1 cursor-pointer`, {
            'text-primary-5': value === tab.value,
            'text-slate-800': value !== tab.value,
          })}
        >
          {tab.label}
        </li>
      ))}
    </ul>
  );
};
