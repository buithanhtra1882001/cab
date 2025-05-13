import React, { useContext, useEffect, useState } from 'react';
import ListFriendChat from '../../components/list-friend-chat/list-friend-chat.component';
import { IFriend } from '../../types/friend';
import ChatDetail from '../../components/chat-detail/chat-detail.component';
import { SignalRContext } from '../../context/signalR';
import { IChat, SendMessagePayload } from '../../types/chat';
import { HubEvent } from '../../enums/hubnames';

export const ChatPage = () => {
  const {
    state: { connectionChat },
  } = useContext(SignalRContext);

  const [currentChat, setCurrentChat] = useState<IFriend | null>(null);
  const [newMessage, setNewMessage] = useState<IChat | null>(null);
  const [listNewMessage, setListNewMessage] = useState<IChat[]>([]);

  const handleSendMessage = (payload: SendMessagePayload) => {
    connectionChat?.invoke('SendMessage', payload);
  };

  useEffect(() => {
    connectionChat?.on(HubEvent.NewMessage, (message: IChat) => {
      setNewMessage(message);
      setListNewMessage([message]);
      setListNewMessage((prev) => {
        if (newMessage) {
          return [newMessage, ...(prev ?? [])];
        }

        return prev;
      });
    });
  }, [connectionChat]);

  return (
    <div className=" relative overflow-hidden border -m-2.5 dark:border-slate-700">
      <div className="flex bg-white dark:bg-dark2">
        <div className="flex-1 border">
          {currentChat ? (
            <div>
              <ChatDetail user={currentChat} handleSendMessage={handleSendMessage} newMessage={newMessage} />
            </div>
          ) : (
            <div className="flex items-center justify-center mt-10">
              <p className="text-sm text-slate-800 dark:text-slate-50">Chọn người để bắt đầu cuộc trò chuyện</p>
            </div>
          )}
        </div>

        <ListFriendChat
          onSelect={(friend) => setCurrentChat(friend)}
          currentChat={currentChat}
          listNewMessage={listNewMessage}
        />
      </div>
    </div>
  );
};
