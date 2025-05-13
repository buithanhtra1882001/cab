import React, { useState, useRef, useEffect } from 'react';
import { IAsideLeftProps } from './aside-left.type';
import { ChatUserList, ChatPopup, userListChat } from '../../components/chat';
import { Link } from 'react-router-dom';
import { navbar } from '../../constants/menu/menu';
import { RootState } from '../../configuration';
import { useSelector } from 'react-redux';
import { fetchStatisticalByUserIdApi } from '../../api';
import { UserProfileStats } from '../../models';
import { BookPlus, Settings } from 'lucide-react';

export const AsideLeft: React.FC<IAsideLeftProps> = () => {
  const [showUserList, setShowUserList] = useState(false);
  const [statisticalUser, setStatisticalUser] = useState<UserProfileStats>();
  const [selectedUser, setSelectedUser] = useState(null);
  const profileUser = useSelector((state: RootState) => state.features.auth.profile);
  const userListRef = useRef(null);

  const handleUserClick = (user) => {
    setShowUserList(false);
    setSelectedUser(user);
  };

  const handleCloseChatPopup = () => {
    setSelectedUser(null);
  };

  useEffect(() => {
    const fetchStatisticalByUserId = async (id) => {
      try {
        const response = await fetchStatisticalByUserIdApi(id);
        setStatisticalUser(response);
      } catch (error) {
        console.error('Failed to fetch leaderboard:', error);
      }
    };

    fetchStatisticalByUserId(profileUser?.userId);
  }, [profileUser]);

  return (
    <aside className="fixed top-0 left-0 z-[99] pt-[--m-top] overflow-hidden transition-transform xl:duration-500 max-xl:w-full max-xl:-translate-x-full">
      <div className="p-2 max-xl:bg-white shadow-sm 2xl:w-72 sm:w-64 w-[80%] h-[calc(100vh-64px)] relative z-30 max-lg:border-r dark:max-xl:!bg-slate-700 dark:border-slate-700">
        <div className="pr-4">
          <nav id="side">
            <ul ref={userListRef}>
              {navbar.map((item) => {
                const { link, name, id, icon } = item;
                return (
                  <li key={id}>
                    <Link type={name === 'Chat' ? 'button' : 'link'} title={name} to={link}>
                      {icon}

                      {name}
                    </Link>
                  </li>
                );
              })}
            </ul>
          </nav>
          <nav
            id="side"
            className="font-medium text-sm text-black border-t pt-3 mt-2 dark:text-white dark:border-slate-800"
          >
            <div className="px-3 pb-2 text-sm font-medium">
              <div className="text-black dark:text-white">Trang</div>
            </div>
            <ul className="mt-2 -space-y-2">
              <li>
                <Link to="/settings">
                  <Settings />
                  <span> Cài đặt </span>
                </Link>
              </li>
              <li>
                <Link to="/account/cabplus">
                  <BookPlus />
                  <span> Nâng cấp CAB+ </span>
                </Link>
              </li>
            </ul>
          </nav>

          <div className="text-xs font-medium flex flex-wrap gap-2 gap-y-0.5 p-2 mt-2">
            <a href="#" className="hover:underline">
              About
            </a>
            <a href="#" className="hover:underline">
              Blog{' '}
            </a>
            <a href="#" className="hover:underline">
              Careers
            </a>
            <a href="#" className="hover:underline">
              Support
            </a>
            <a href="#" className="hover:underline">
              Contact Us{' '}
            </a>
            <a href="#" className="hover:underline">
              Developer
            </a>
          </div>
        </div>
        {showUserList && <ChatUserList userList={userListChat} onUserSelect={handleUserClick} />}
        {selectedUser && <ChatPopup selectedUser={selectedUser} onClose={handleCloseChatPopup} />}
      </div>
    </aside>
  );
};
