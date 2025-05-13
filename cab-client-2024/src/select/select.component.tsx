import React from 'react';
import ReactSelect, { GroupBase, Props } from 'react-select';
import { useDarkMode } from '../hooks';

export interface IOption {
  label: string;
  value: any;
}

type SelectProps<
  Option = unknown,
  IsMulti extends boolean = false,
  Group extends GroupBase<Option> = GroupBase<Option>,
> = {
  name?: string;
  isValid?: boolean;
  clearable?: boolean;
  readonly?: boolean;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  options: IOption[];
  placeholder?: string;
  CustomOptionCompo?: React.FunctionComponent;
  CustomNoOptionCompo?: React.FunctionComponent;
} & Props<Option, IsMulti, Group>;

const Select = ({ options, ...rest }: SelectProps) => {
  const { isDarkMode } = useDarkMode();

  return (
    <ReactSelect
      {...rest}
      value={options.find((val: IOption) => val.value === rest.value)}
      components={{
        IndicatorSeparator: () => null,
      }}
      menuPlacement="auto"
      isSearchable={false}
      options={options}
      placeholder="Lựa chọn..."
      className="text-center text-sm  rounded-md dark:border-zinc-50/10 text-slate-800 dark:text-slate-50"
      styles={{
        placeholder: (base) => ({ ...base, textAlign: 'left' }),
        menu: (base) => ({
          ...base,
          width: 'max-content',
          minWidth: '100%',
        }),
        menuList: (base) => ({
          ...base,
          overflow: 'initial',
          paddingTop: 0,
          textAlign: 'left',
        }),
        singleValue: (base) => ({
          ...base,
          color: isDarkMode ? '#f8fafc' : '#1e293b',
          textAlign: 'left',
        }),
        control: (base) => ({
          ...base,
          backgroundColor: isDarkMode ? '#27272A' : '#f1f5f9',
          border: 'none !important',
          borderRadius: '6px !important',
          boxShadow: '0 !important',
          '&:hover': {
            border: '0 !important',
          },
          textAlign: 'right',
          position: 'static',
          transform: 'none',
        }),
        option: (base, state) => ({
          ...base,
          backgroundColor: state.isSelected ? (isDarkMode ? '#27272A' : '#f1f5f9') : isDarkMode ? '#27272A' : '#f1f5f9',
          color: state.isSelected ? '#2797ed' : '#1e293b',
          cursor: 'pointer',

          '&:hover': {
            backgroundColor: isDarkMode ? '#27272A' : '#f1f5f9',
          },
        }),
      }}
    />
  );
};

export default Select;
