/* eslint-disable react/jsx-indent */
import React from 'react';
import { IChatListProps } from './chat.type';

export const ChatUserList: React.FC<IChatListProps> = (props) => {
  const { userList, onUserSelect } = props;

  return (
    <div className="fixed left-4 top-0 mt-20 bg-opacity-50 bg-black flex items-center justify-center">
      <div className="w-80 h-screen bg-white dark:bg-zinc-900 dark:text-gray-3 border border-gray-300 rounded-md shadow-md overflow-y-auto">
        <div className="bg-blue-500 text-white px-4 py-2 flex justify-between items-center rounded-t-md">
          <h3 className="text-lg font-semibold">Đoạn chat</h3>
          <button onClick={() => onUserSelect && onUserSelect(null)}>&times;</button>
        </div>
        <div>
          <ul>
            {userList?.map((user) => (
              <li
                key={user?.userId}
                className="flex items-center justify-between py-3 px-6 border-b hover:bg-gray-100 dark:hover:bg-gray-700 transition duration-300 hover:shadow-md w-full"
                onClick={() => onUserSelect && onUserSelect(user)}
              >
                <div className="flex items-center space-x-4 w-full">
                  <img src={user?.url} alt={user?.name} className="w-12 h-12 rounded-full" />
                  <span className="text-lg">{user?.name}</span>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
};
