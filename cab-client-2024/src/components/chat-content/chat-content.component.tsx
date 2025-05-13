import React, { useEffect, useRef, useState } from 'react';
import { IFriend } from '../../types/friend';
import { useChatDetail } from '../../hooks/use-chat-detail/useChatDetail';
import Loading from '../loading/loading.component';
import classNames from 'classnames';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import { IChat, IGetChatByIdPayload } from '../../types/chat';
import InfiniteScroll from 'react-infinite-scroll-component';
import _size from 'lodash/size';
// import dayjs from 'dayjs';

interface ChatContentProps {
  user: IFriend;
  newMessage: IChat | null;
}

const selfId = 'wrapper-chat-content';

const ChatContent = ({ user, newMessage }: ChatContentProps) => {
  const userProfile = useSelector((state: RootState) => state.features.auth.profile);

  const selfRef = useRef<HTMLDivElement>(null);

  const [pageNumber, setPageNumber] = useState(1);

  const { listChat, appendMessage, loading, hasMore, loadMoreChat, pagingStateFirst, pagingStateLast } = useChatDetail({
    pageNumber,
    friendId: user.userId,
  });

  const handleLoadMoreChat = async () => {
    setPageNumber((prev) => prev + 1);

    const payload: IGetChatByIdPayload = {
      pagingStateFirst,
      pagingStateLast,
      friendUserId: user.userId,
      pageNumber: pageNumber + 1,
      pageSize: 20,
    };
    await loadMoreChat(payload);
  };

  useEffect(() => {
    if (!newMessage) {
      return;
    }

    const isMyMessage = newMessage.senderUserId === user.userId || newMessage.senderUserId === userProfile?.userId;
    if (isMyMessage) {
      appendMessage(newMessage);
    }
  }, [newMessage, user]);

  return (
    <div
      className={classNames(`overflow-auto flex flex-col-reverse w-full`, {
        'justify-center': !_size(listChat),
      })}
      style={{
        height: 'calc(100vh - 230px)',
      }}
      ref={selfRef}
      id={selfId}
    >
      <InfiniteScroll
        dataLength={_size(listChat)}
        next={async () => {
          await handleLoadMoreChat();
        }}
        hasMore={hasMore}
        loader={<Loading classname="mx-auto" />}
        endMessage={null}
        scrollableTarget={selfId}
        inverse
        style={{ display: 'flex', flexDirection: 'column-reverse', overflow: 'visible' }}
      >
        <ul className="space-y-1.5">
          {listChat &&
            listChat.map((chat) =>
              chat.content ? (
                <li
                  key={chat.createdAt}
                  className={classNames(`flex gap-2  `, {
                    'items-end flex-row-reverse': chat.senderUserId === userProfile?.userId,
                  })}
                >
                  <img src={userProfile?.avatar} alt="" className="w-5 h-5 rounded-full shadow" />
                  <div
                    className={classNames(`px-3 py-2 rounded-xl`, {
                      'px-4 py-2 rounded-[20px] max-w-sm bg-gradient-to-tr from-sky-500 to-blue-500 text-white shadow':
                        chat.senderUserId === userProfile?.userId,
                      'px-4 py-2 rounded-[20px] max-w-sm bg-secondery': chat.senderUserId !== userProfile?.userId,
                    })}
                  >
                    {chat.content}
                  </div>

                  {/* <div className="mt-1">
                    <p className="text-xs text-slate-600 dark:text-slate-100 text-right">
                      {dayjs(chat.createdAt).fromNow()}
                    </p>
                  </div> */}
                </li>
              ) : null,
            )}
        </ul>

        {!_size(listChat) && !loading ? (
          <p className="text-sm text-slate-800 dark:text-slate-50 text-center">
            Bạn và {user.fullName} chưa có cuộc trò chuyện nào
          </p>
        ) : null}
      </InfiniteScroll>
    </div>
  );
};

export default ChatContent;
