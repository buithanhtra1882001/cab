import React, { useState, useRef } from 'react';
import { IAsideLeftProps } from './aside-left.type';
import { ChatUserList, ChatPopup, userListChat } from '../../components/chat';
import { Link } from 'react-router-dom';
import { navbarVideos } from '../../constants/menu/menu';
import { RootState } from '../../configuration';
import { useSelector } from 'react-redux';

export const AsideVideoLeft: React.FC<IAsideLeftProps> = () => {
  const [showUserList, setShowUserList] = useState(false);

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

  return (
    <aside className="overflow-y-auto no-scrollbar" style={{ height: 'calc(100vh - 64px)' }}>
      <div className="pt-8">
        <section className="my-5">
          <div className="text-lg font-bold mb-2">Video</div>
        </section>

        <section>
          <ul className="text-sm font-medium space-y-2" ref={userListRef}>
            {navbarVideos.map((item) => {
              const { link, name, id, icon } = item;
              return (
                <li key={id}>
                  <Link
                    className="rounded-md flex justify-start items-center gap-2
                  h-10 text-sm px-4 text-zinc-800 dark:text-zinc-50 hover:bg-primary-6/
                  dark:hover:bg-zinc-800 transition duration-200 py-2"
                    type={name === 'Chat' ? 'button' : 'link'}
                    title={name}
                    to={link}
                  >
                    {icon}
                    {name}
                  </Link>
                </li>
              );
            })}
          </ul>
        </section>
      </div>
      {showUserList && <ChatUserList userList={userListChat} onUserSelect={handleUserClick} />}
      {selectedUser && <ChatPopup selectedUser={selectedUser} onClose={handleCloseChatPopup} />}
    </aside>
  );
};
