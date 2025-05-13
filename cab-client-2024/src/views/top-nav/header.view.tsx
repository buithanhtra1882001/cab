/* eslint-disable react/self-closing-comp */
import React, { useContext, useEffect, useRef, useState } from 'react';
import { ITopNavProps } from './top-nav.type';
import { NotificationPopup } from './components';
import Logo from '../../components/logo/logo.component';
import MenuHeader from './components/menu-header/menu-header';
import { Bell, Moon, NotebookPen, Search, Sun } from 'lucide-react';
import { useOnClickOutside } from 'usehooks-ts';
import { Link } from 'react-router-dom';
import { routePaths } from '../../routes/routes-path';
import { useDarkMode } from '../../hooks';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import { SignalRContext } from '../../context/signalR';
import { HubEvent } from '../../enums/hubnames';

export const TopNav: React.FC<ITopNavProps> = () => {
  const profile = useSelector((state: RootState) => state.features.auth.profile);
  const isAuthenticated = useSelector((state: RootState) => state.features.auth.profile);

  const [isShowNotifyPopup, setShowNotifyPopup] = useState<boolean>(false);
  const [isNewNoti, setIsNewNoti] = useState<boolean>(false);

  const { theme, toggleTheme } = useDarkMode();

  const notificationRef = useRef<HTMLButtonElement>(null);
  const {
    state: { connectionNoti },
  } = useContext(SignalRContext);

  useOnClickOutside(notificationRef, () => {
    setShowNotifyPopup(false);
  });
  useEffect(() => {
    connectionNoti?.on(HubEvent.NewNotification, (message: any) => {
      setIsNewNoti(true);
    });
  }, [connectionNoti]);

  return (
    <header className="z-[100] h-[--m-top] fixed top-0 left-0 w-full flex items-center bg-white/80 sky-50 backdrop-blur-xl border-b border-slate-200 dark:bg-dark2 dark:border-slate-800">
      <div className="flex items-center w-full xl:px-6 px-2 max-lg:gap-10">
        <div className="w-[18rem] lg:w-[16rem]">
          <Logo />
        </div>
        <div className="flex-1 relative">
          <div className="max-w-[1040px] mx-auto flex items-center">
            <div
              id="search--box"
              className="xl:w-[680px] sm:w-96 sm:relative rounded-xl overflow-hidden z-20 bg-secondery max-md:hidden w-screen left-0 max-sm:fixed max-sm:top-2 dark:!bg-white/5"
              aria-haspopup="true"
              aria-expanded="false"
            >
              <Search size={16} className="absolute top-1/2 left-3 transform -translate-y-1/2 " />
              <input
                type="text"
                placeholder="Search Friends, videos .."
                className="w-full !pl-10 !font-normal !bg-transparent h-12 !text-sm outline-none"
              />
            </div>
          </div>
          <div className="flex items-center sm:gap-4 gap-2 absolute right-5 top-1/2 -translate-y-1/2 text-black">
            <button
              className="hidden rounded-md  justify-center items-center gap-2 h-10 text-sm px-4 py-2 text-primary-6 dark:text-primary-3"
              // target="https://www.facebook.com/cabcomeback"
              title="Phản hồi"
            >
              <NotebookPen />
            </button>

            {isAuthenticated && (
              <button
                className="sm:p-2 p-1 rounded-full relative sm:bg-secondery dark:text-white"
                type="button"
                onClick={() => {
                  setShowNotifyPopup(!isShowNotifyPopup);
                  setIsNewNoti(false);
                }}
                ref={notificationRef}
              >
                <Bell size={24} />
                {isNewNoti && (
                  <div className="absolute -z-1 block w-3 h-3 bg-red-500 border-2 border-white rounded-full -top-0 end-0 dark:border-gray-900" />
                )}

                <div className="absolute right-0 top-10">
                  <NotificationPopup
                    visible={isShowNotifyPopup}
                    onClose={() => {
                      setShowNotifyPopup(false);
                    }}
                  />
                </div>
              </button>
            )}
            <button className="sm:p-2 p-1 rounded-full relative sm:bg-secondery dark:text-white">
              {theme === 'dark' ? (
                <Sun size={24} className="cursor-pointer" onClick={toggleTheme} />
              ) : (
                <Moon size={24} className="cursor-pointer" onClick={toggleTheme} />
              )}
            </button>

            {!isAuthenticated && profile && (
              <div className="ml-3">
                <Link
                  to={routePaths.login}
                  className="px-4 py-2 rounded-md bg-primary text-slate-50 text-sm font-medium mr-3"
                >
                  Đăng nhập
                </Link>
              </div>
            )}
            <div className="ml-3">
              <MenuHeader profile={profile} />
            </div>
          </div>
        </div>
      </div>
    </header>
  );
};
