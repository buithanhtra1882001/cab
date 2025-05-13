/* eslint-disable react/jsx-indent */
import React from 'react';
import { IChatProps } from './chat.type';

export const ChatPopup: React.FC<IChatProps> = (props) => {
  const { selectedUser, onClose } = props;

  return (
    <div className="fixed bottom-8 left-8 w-80 h-96 bg-white dark:bg-zinc-900 dark:text-gray-3 border border-gray-300 rounded-md shadow-md">
      <div className="bg-blue-500 text-white px-4 py-2 flex justify-between items-center rounded-t-md">
        <div className="flex items-center space-x-2 w-full">
          <img src={selectedUser?.url} alt={selectedUser?.name} className="w-8 h-8 rounded-full" />
          <span className="text-base">{selectedUser?.name}</span>
        </div>
        <button onClick={() => onClose && onClose()}>&times;</button>
      </div>
      <div className="p-4 h-72 overflow-y-auto">{/* Nội dung chat ở đây */}</div>
      <div className="w-full h-12">
        <input
          type="text"
          placeholder="Nhập tin nhắn của bạn..."
          className="w-full h-full px-4 py-2 border border-gray-300 dark:bg-zinc-900 dark:text-gray-3 rounded-md focus:outline-none focus:border-blue-500"
        />
      </div>
    </div>
  );
};
