import React, { useEffect, useState } from 'react';
import { LANGUAGE_CORE_UI_MODAL_NAMESPACE, ModalService } from './modal.service';
import { IModal, IModalPlaceholder } from './modal.type';
import { useLangTranslation } from '../../configuration';
import { LazyLoadImage } from '../lazy-load-image';
import { X } from 'lucide-react';

export const Modal: React.FC<IModal> = (props) => {
  const {
    title,
    content,
    type,
    onClose = (): Promise<void> => {
      return Promise.resolve();
    },
    onConfirm = (): Promise<void> => {
      return Promise.resolve();
    },
    overlayColor,
    confirmValidator,
    placeholderUuid,
    uuid,
    closeOnOverlay,
    /**
     * !     size = 'md',
     * TODO: Handle size for modal Instead of using className
     */
    isLightBox = false,
    image = '',
    fixedTop = true,
    className = 'lg:w-max lg:min-w-[40%]',
  } = props;

  const { translator } = useLangTranslation(LANGUAGE_CORE_UI_MODAL_NAMESPACE);

  const confirm = () => {
    if (confirmValidator) {
      if (confirmValidator()) {
        onConfirm().then(() => {
          close();
        });
      }
    } else {
      onConfirm().then(() => {
        close();
      });
    }
  };

  const close = () => {
    onClose().then(() => {
      placeholderUuid && uuid && ModalService.removeModal(placeholderUuid, uuid);
    });
  };

  const overlayClick = (e: React.MouseEvent<HTMLDivElement, MouseEvent>) => {
    e.stopPropagation();
    if (closeOnOverlay) {
      close();
    }
  };

  useEffect(() => {
    const handleEsc = (event) => {
      if (event.key === 'Escape') {
        close();
      }
    };
    window.addEventListener('keydown', handleEsc);

    return () => {
      window.removeEventListener('keydown', handleEsc);
    };
  }, []);

  return (
    <div
      className={`toto-modal ${
        fixedTop ? 'fixed top-0 left-0 z-50' : ''
      } w-screen h-screen bg-zinc-100 dark:bg-zinc-900 bg-opacity-90 dark:bg-opacity-90`}
      style={{ backgroundColor: overlayColor }}
      onClick={overlayClick}
    >
      {!isLightBox && (
        <div className="flex justify-center items-center h-full" onClick={(e) => e.stopPropagation()}>
          <div className={`w-full lg:w-max lg:min-w-[50%] lg:max-w-[700px] ${className}`}>
            <div className="rounded-xl flex transition duration-200 bg-white dark:bg-zinc-900 flex-col">
              <div className="flex justify-between items-center p-6">
                <div className="flex-1 text-2xl font-semibold line-clamp-1">{title}</div>
                <button className="rounded-full" onClick={() => close()}>
                  <X />
                </button>
              </div>
              {content}

              {type === 'confirm' && (
                <div className="flex flex-row-reverse">
                  <button
                    onClick={() => confirm()}
                    type="submit"
                    className="rounded-md  flex justify-center items-center gap-2  h-10 text-sm px-4 py-2 bg-primary-4 dark:bg-primary text-white dark:text-gray-100 hover:bg-primary dark:hover:bg-primary-6 break-words transition duration-200 ml-5"
                    tabIndex={-1}
                  >
                    <div className="line-clamp-1">
                      {translator('button_confirm_label', { defaultValue: 'Cập nhật' })}
                    </div>
                  </button>
                  {/**
                   * TODO handle button reset modal
                   */}
                  {/* <button
                    type="reset"
                    className="rounded-md  flex justify-center items-center gap-2  h-10 text-sm px-4 py-2 bg-primary-4 dark:bg-primary text-white dark:text-gray-100 hover:bg-primary dark:hover:bg-primary-6 break-words transition duration-200 "
                    tabIndex={-1}
                  >
                    <div className="line-clamp-1">Reset</div>
                  </button> */}
                  <button
                    onClick={() => close()}
                    type="reset"
                    className="rounded-md  flex justify-center items-center gap-2  h-10 text-sm px-4 py-2 bg-primary-4 dark:bg-primary text-white dark:text-gray-100 hover:bg-primary dark:hover:bg-primary-6 break-words transition duration-200 "
                    tabIndex={-1}
                  >
                    <div className="line-clamp-1"> {translator('button_cancel_label', { defaultValue: 'Cancel' })}</div>
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {isLightBox && (
        <div className="flex justify-center items-center h-full">
          <div className={`w-full lg:w-max lg:min-w-[50%] lg:max-w-[700px] ${className}`}>
            <div className="rounded-xl flex transition duration-200 bg-white dark:bg-zinc-900 flex-col">
              <div className="flex justify-between items-center p-4">
                <div className="flex-1 text-2xl font-semibold line-clamp-1">{title}</div>
                <button
                  className="rounded-full flex justify-center items-center w-9 h-9 bg-zinc-100 dark:bg-zinc-700 hover:bg-zinc-200 hover:dark:bg-zinc-600"
                  onClick={() => close()}
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width={24}
                    height={24}
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth={2}
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    className="tabler-icon tabler-icon-x "
                  >
                    <path d="M18 6l-12 12" />
                    <path d="M6 6l12 12" />
                  </svg>
                </button>
              </div>
              <LazyLoadImage src={image} alt={image} />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export const ModalPlaceholder: React.FC<IModalPlaceholder> = (props) => {
  const { uuid } = props;
  const [modal, setModal] = useState<IModal | null>(null);

  // eslint-disable-next-line no-console
  useEffect(() => {
    const subscription = ModalService.ModalServiceSubject().subscribe((data) => {
      if (data && uuid && data[uuid]) {
        setModal({ ...data[uuid] });
      } else {
        setModal(null);
      }
    });

    return () => {
      subscription.unsubscribe();

      uuid && ModalService.removeModalPlaceholder(uuid);
    };
  }, []);

  return <div className="toto-modal-placeholder">{modal && <Modal {...modal} />}</div>;
};
