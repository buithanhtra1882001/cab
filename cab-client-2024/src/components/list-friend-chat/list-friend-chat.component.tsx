import React, { useEffect, useState } from 'react';
import { useFriend } from '../../hooks/use-friend/useFriend';
import { IFriend } from '../../types/friend';
import classNames from 'classnames';
import _size from 'lodash/size';
import { IChat } from '../../types/chat';
import { FcSettings } from 'react-icons/fc';
import { SearchIcon } from 'lucide-react';

interface ListFriendChatProps {
  listNewMessage: IChat[];
  currentChat: IFriend | null;
  onSelect: (friend: IFriend) => void;
}

const ListFriendChat = ({ listNewMessage, currentChat, onSelect }: ListFriendChatProps) => {
  const { data } = useFriend({ pageSize: 100 });
  const [filteredFriends, setFilteredFriends] = useState(data);
  const [searchQuery, setSearchQuery] = useState('');
  const handleSearchChange = (event) => {
    setSearchQuery(event.target.value);
  };
  useEffect(() => {
    setFilteredFriends(data);
  }, [data]);
  useEffect(() => {
    const filtered = data.filter((friend) => friend.fullName.toLowerCase().includes(searchQuery.toLowerCase()));
    console.log('🚀 ~ useEffect ~ filtered:', filtered);
    setFilteredFriends(filtered);
  }, [searchQuery]);

  return (
    <div className="md:w-[360px] relative border-r dark:border-slate-700">
      <div className="top-0 left-0 max-md:fixed max-md:w-5/6 max-md:h-screen bg-white z-50 max-md:shadow max-md:-translate-x-full dark:bg-dark2">
        <div className="p-4 border-b dark:border-slate-700">
          <div className="flex mt-2 items-center justify-between">
            <h2 className="text-2xl font-bold text-black ml-1 dark:text-white"> Chats </h2>

            <div className="flex items-center gap-2.5">
              <button>
                <FcSettings size={24} />
              </button>
            </div>
          </div>
          <div className="relative mt-4">
            <div className="absolute left-3 bottom-1/2 translate-y-1/2 flex">
              <SearchIcon size={20} className="text-gray-400 dark:text-slate-400" />
            </div>
            <input
              value={searchQuery}
              onChange={handleSearchChange}
              type="text"
              placeholder="Search"
              className="w-full !pl-10 !py-2 !rounded-lg"
            />
          </div>
        </div>
        <div className="space-y-2 p-2 overflow-y-auto md:h-[calc(100vh-204px)] h-[calc(100vh-130px)]">
          {filteredFriends?.map((friend) => (
            <div
              key={friend.userId}
              className={classNames(`relative flex items-center gap-4 p-2 duration-200 rounded-xl hover:bg-secondery`, {
                'bg-secondery dark:bg-zinc-900': currentChat?.userId === friend.userId,
                '': _size(listNewMessage) && listNewMessage.some((message) => message.senderUserId === friend?.userId),
              })}
              onClick={() => onSelect(friend)}
            >
              <div className="relative w-14 h-14 shrink-0">
                <img src={friend.avatar} alt="" className="object-cover w-full h-full rounded-full" />
                <div
                  className={`w-4 h-4 absolute bottom-0 right-0  ${
                    friend.isOnline ? 'bg-green-500 ' : 'bg-gray-200 '
                  }rounded-full border border-white dark:border-slate-800`}
                />
              </div>
              <div className="flex-1">
                <h4 className="text-sm text-slate-800 dark:text-slate-50">{friend.fullName}</h4>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ListFriendChat;
