/* eslint-disable react/self-closing-comp */
import React, { useRef, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { routePaths } from '../../../../routes/routes-path';
import { googleLogout } from '@react-oauth/google';
import { useOnClickOutside } from 'usehooks-ts';
import { LocalStorageService, RootState } from '../../../../configuration';
import Avatar from '../../../../components/avatar/avatar.component';
import { IUserProfile } from '../../../../models';
import { useSelector } from 'react-redux';

interface MenuHeaderProps {
  profile: IUserProfile | null;
}

const MenuHeader = ({ profile }: MenuHeaderProps) => {
  const isAuthenticated = useSelector((state: RootState) => state.features.auth.profile);

  const navigate = useNavigate();

  const dropdownRef = useRef<HTMLDivElement>(null);

  const [visible, setVisible] = useState<boolean>(false);

  const handleClose = () => {
    setVisible(false);
  };

  const handleLogout = () => {
    LocalStorageService.removeItem('accessToken');
    LocalStorageService.removeItem('fingerprintHash');
    LocalStorageService.removeItem('refreshToken');

    googleLogout();
    navigate(routePaths.login);
  };

  useOnClickOutside(dropdownRef, handleClose);

  if (!profile) {
    return null;
  }

  return (
    <div className="relative" ref={dropdownRef}>
      <div onClick={() => setVisible((prev) => !prev)} className="cursor-pointer">
        <Avatar avatar={profile.avatar} label={profile.username} size="sm" />
      </div>

      {visible && (
        <div className="absolute top-14 right-0 rounded-md max-w-[256px] w-[500px] bg-white dark:bg-gray-900 space-y-3 p-4 drop-shadow-xl">
          <ul className="text-sm text-left text-slate-800 dark:text-slate-50">
            {isAuthenticated && (
              <li>
                <Link
                  to={routePaths.profile}
                  className="rounded-md px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 block w-full"
                  onClick={handleClose}
                >
                  Trang cá nhân
                </Link>
              </li>
            )}
            {isAuthenticated && (
              <li>
                <a
                  href="/settings"
                  className="rounded-md px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 block w-full"
                  target="_blank"
                  rel="noreferrer"
                >
                  Cài đặt
                </a>
              </li>
            )}
            {isAuthenticated && (
              <li>
                <a
                  href="/manage/histories"
                  className="rounded-md px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 block w-full"
                  target="_blank"
                  rel="noreferrer"
                >
                  Lịch sử giao dịch
                </a>
              </li>
            )}

            <li>
              <a
                href="https://about.cab.vn/help"
                className="rounded-md px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 block w-full"
                target="_blank"
                rel="noreferrer"
              >
                Trợ giúp
              </a>
            </li>
            <li>
              <a
                href="https://about.cab.vn/terms"
                className="rounded-md px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 block w-full"
                target="_blank"
                rel="noreferrer"
              >
                Điều khoản sử dụng
              </a>
            </li>
            <li>
              <a
                href="https://about.cab.vn/advertising"
                className="rounded-md px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 block w-full"
                target="_blank"
                rel="noreferrer"
              >
                Quảng cáo
              </a>
            </li>
            <li>
              <a
                href="https://about.cab.vn/contact"
                className="rounded-md px-4 py-2 hover:bg-slate-100 dark:hover:bg-slate-800 block w-full"
                target="_blank"
                rel="noreferrer"
              >
                Liên hệ
              </a>
            </li>
          </ul>

          {isAuthenticated && (
            <Link
              to="/account/topup"
              className="rounded-md flex justify-center items-center gap-2 h-10 text-sm px-4 py-2 bg-primary dark:bg-primary text-white dark:text-gray-100 hover:bg-primary dark:hover:bg-primary-6 break-words transition duration-200 w-full"
            >
              Nạp đồng+
            </Link>
          )}

          <Link
            to="/account/cabplus"
            className="rounded-md flex justify-center items-center gap-2 h-10 text-sm px-4 py-2 bg-violet-800 dark:bg-violet-800 text-white dark:text-gray-100 hover:bg-violet-900 dark:hover:bg-violet-800 transition duration-200 w-full"
          >
            Nâng cấp CAB+
          </Link>

          {isAuthenticated && (
            <button
              className="rounded-md flex justify-center items-center gap-2 h-10 text-sm px-4 py-2 bg-red-500 hover:bg-red-600 text-white dark:text-gray-100 transition duration-200 w-full"
              onClick={handleLogout}
            >
              Đăng xuất
            </button>
          )}
        </div>
      )}
    </div>
  );
};

export default MenuHeader;
