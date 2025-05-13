import classnames from 'classnames';
import React, { PropsWithChildren } from 'react';

export type AlertType = 'error' | 'warning' | 'success' | 'default';

interface AlertProps extends PropsWithChildren {
  type?: AlertType;
  classNames?: string;
  visible?: boolean;
}

const Alert = ({ type = 'default', classNames, visible, children }: AlertProps) => {
  const styles: Record<AlertType, string> = {
    success: 'bg-green-50 border-green-800 text-green-800',
    error: 'bg-red-50 border-red-800 text-red-800',
    warning: 'bg-yellow-50 border-yellow-800 text-yellow-800',
    default: 'bg-zinc-50 border-zinc-800 text-zinc-800',
  };

  if (!visible) {
    return null;
  }

  return (
    <div
      className={classnames(
        `w-full px-4 py-2 rounded-md text-sm font-medium border border-solid`,
        styles[type],
        classNames,
      )}
    >
      {children}
    </div>
  );
};

export default Alert;
