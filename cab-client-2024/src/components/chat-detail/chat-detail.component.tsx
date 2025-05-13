import React from 'react';
import { IFriend } from '../../types/friend';
import ChatEditor from '../chat-editor/chat-editor.component';
import { IChat, SendMessagePayload } from '../../types/chat';
import ChatContent from '../chat-content/chat-content.component';

interface ChatDetailProps {
  user: IFriend;
  newMessage: IChat | null;
  handleSendMessage: (payload: SendMessagePayload) => void;
}

const ChatDetail = ({ user, newMessage, handleSendMessage }: ChatDetailProps) => {
  return (
    <div className="flex">
      <div className="flex-1">
        <div
          className="flex flex-col justify-between"
          style={{
            height: 'calc(100vh - 64px)',
          }}
        >
          <div className="flex items-center justify-between gap-2 w- px-6 py-3.5  z-10 border-b dark:border-slate-700 uk-animation-slide-top-medium ">
            <div className="flex items-center sm:gap-4 gap-2">
              <div className="relative cursor-pointer max-md:hidden">
                <img alt="user" src={user.avatar} className="w-8 h-8 rounded-full shadow" />
                <div className="w-2 h-2 bg-teal-500 rounded-full absolute right-0 bottom-0 m-px" />
              </div>
              <div className="cursor-pointer">
                <div className="text-base font-bold"> {user.fullName}</div>
                <div className="text-xs text-green-500 font-semibold"> Online</div>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <button className="hover:bg-slate-100 p-1.5 rounded-full">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth="1.5"
                  stroke="currentColor"
                  className="w-6 h-6"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z"
                  />
                </svg>
              </button>
            </div>
          </div>
          <div className="w-full p-5 py-10 overflow-y-auto md:h-[calc(100vh-204px)] h-[calc(100vh-195px)]">
            <ChatContent user={user} newMessage={newMessage} />
          </div>
          <ChatEditor user={user} handleSendMessage={handleSendMessage} />
        </div>
      </div>
    </div>
  );
};

export default ChatDetail;
