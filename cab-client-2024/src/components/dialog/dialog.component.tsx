import classNames from 'classnames';
import { X } from 'lucide-react';
import React, { FC, memo, ReactNode } from 'react';
import ReactDOM from 'react-dom';
import { useScrollLock } from 'usehooks-ts';

type PlacementDialog = 'CENTER' | 'TOP_CENTER' | 'TOP_LEFT' | 'TOP-RIGHT' | 'BOTTOM_LEFT' | 'BOTTOM_RIGHT';

export type SizeDialog = 'sm' | 'md' | 'lg' | 'xl' | 'xxl' | '3xl' | 'full';

interface IDialogProps {
  visible?: boolean;
  title?: ReactNode;
  children?: ReactNode;
  footer?: ReactNode;
  placement?: PlacementDialog;
  size?: SizeDialog;
  hideOnClickOutside?: boolean;
  onClose?: () => void;
}

const Dialog: FC<IDialogProps> = ({
  visible = false,
  placement = 'TOP_CENTER',
  size = 'md',
  title,
  children,
  onClose,
}) => {
  const getSizeDialog = (): string => {
    const sizeObj: Record<SizeDialog, string> = {
      sm: 'max-w-sm',
      md: 'max-w-md',
      lg: 'max-w-2xl',
      xl: 'max-w-3xl',
      xxl: 'max-w-4xl',
      '3xl': 'max-w-6xl',
      full: 'max-w-full',
    };
    return sizeObj[size];
  };

  useScrollLock({
    autoLock: visible,
    lockTarget: 'body',
  });

  return ReactDOM.createPortal(
    visible ? (
      <div
        className={classNames({
          'hidden lg:p-20 uk- open uk-modal uk-open': visible,
          ' hidden lg:p-20 uk- open uk-modal': !visible,
        })}
      >
        <div
          className={classNames(['mx-auto w-full h-max flex py-8', getSizeDialog()], {
            'justify-center items-center h-full': placement === 'CENTER',
          })}
        >
          <div
            className={classNames(
              `dialog-content py-5 px-8 bg-white dark:bg-zinc-800 rounded-md w-full
             text-zinc-900`,
            )}
          >
            <div className="flex items-center">
              <h4 className="text-zinc-900 dark:text-zinc-50 text-xl font-semibold mr-3">{title}</h4>

              {onClose ? (
                <button
                  type="button"
                  className="text-xl text-zinc-900 dark:text-zinc-50 font-semibold ml-auto"
                  onClick={() => {
                    onClose();
                  }}
                >
                  <X />
                </button>
              ) : null}
            </div>

            <div>{children}</div>
          </div>
        </div>
      </div>
    ) : null,
    document.getElementById('dialog-root') || ({} as HTMLElement),
  );
};

export default memo(Dialog);
