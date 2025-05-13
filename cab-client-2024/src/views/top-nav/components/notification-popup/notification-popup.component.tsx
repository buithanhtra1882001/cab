import React from 'react';
import { INotificationPopupProps } from './notification-popup.type';
import { NotificationPage } from '../../../../pages';
import classNames from 'classnames';
import { Eye } from 'lucide-react';
import { Link } from 'react-router-dom';
import { routePaths } from '../../../../routes/routes-path';

export const NotificationPopup: React.FC<INotificationPopupProps> = ({ visible, onClose }) => {
  return (
    <div
      // id="notification-item"
      className={classNames(
        `z-20 w-[500px] max-w-sm bg-white divide-y divide-gray-100 rounded-lg shadow
         dark:bg-gray-800 dark:divide-gray-700`,
        {
          'hidden opacity-0': !visible,
          'visible opacity-100': visible,
        },
      )}
    >
      <div className="block px-4 py-2 font-medium text-center text-gray-700 rounded-t-lg bg-gray-50 dark:bg-gray-800 dark:text-white">
        Thông báo
      </div>
      <div className="divide-y divide-gray-100 dark:divide-gray-700">
        <NotificationPage isDropdown />
      </div>
      <Link
        to={routePaths.notifications}
        className="block py-2 text-sm font-medium text-center text-gray-900 rounded-b-lg bg-gray-50 hover:bg-gray-100 dark:bg-gray-800 dark:hover:bg-gray-700 dark:text-white"
        onClick={() => {
          onClose?.();
        }}
      >
        <div className="inline-flex items-center ">
          <Eye size={14} className="mr-2" />
          Xem tất cả
        </div>
      </Link>
    </div>
  );
};
