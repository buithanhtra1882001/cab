import { useCallback, useEffect, useState } from 'react';
import { chatService } from '../../services/chat.service';
import { IChat, IGetChatByIdPayload } from '../../types/chat';

export const useChatDetail = ({ friendId, pageNumber }: { friendId: string; pageNumber: number }) => {
  const [listChat, setListChat] = useState<IChat[] | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [hasMore, setHasMore] = useState<boolean>(false);
  const [pagingStateFirst, setPagingStateFirst] = useState<string>('');
  const [pagingStateLast, setPagingStateLast] = useState<string>('');

  const loadMoreChat = useCallback(
    async (payload: Omit<IGetChatByIdPayload, 'pagingStateFirst' | 'pagingStateLast'>) => {
      try {
        const _payload = {
          ...payload,
          pagingStateFirst,
          pagingStateLast,
        };

        setLoading(true);
        const {
          data,
          hasNext,
          pagingStateFirst: pagingStateFirstResponse,
          pagingStateLast: pagingStateLastResponse,
        } = await chatService.getChatById(_payload);

        setListChat((prev) => [...data, ...(prev ?? [])]);
        setHasMore(hasNext);
        setPagingStateFirst(pagingStateFirstResponse);
        setPagingStateLast(pagingStateLastResponse);
      } catch {
        //
      } finally {
        setLoading(false);
      }
    },
    [pagingStateFirst, pagingStateLast],
  );

  const getChatById = async (friendId: string) => {
    try {
      setLoading(true);
      const payload: IGetChatByIdPayload = {
        friendUserId: friendId,
        pageNumber,
        pageSize: 20,
        pagingStateFirst: '',
        pagingStateLast: '',
      };

      const {
        data,
        hasNext,
        pagingStateFirst: pagingStateFirstResponse,
        pagingStateLast: pagingStateLastResponse,
      } = await chatService.getChatById(payload);

      setListChat(data.reverse());
      setHasMore(hasNext);
      setPagingStateFirst(pagingStateFirstResponse);
      setPagingStateLast(pagingStateLastResponse);
    } catch {
      setListChat(null);
    } finally {
      setLoading(false);
    }
  };

  const appendMessage = (newChat: IChat) => {
    setListChat((prev) => [...(prev ?? []), newChat]);
  };

  const handleResetHook = () => {
    setListChat(null);
    setPagingStateFirst('');
    setPagingStateLast('');
  };

  useEffect(() => {
    handleResetHook();
    getChatById(friendId);
  }, [friendId]);

  return {
    loading,
    listChat,
    hasMore,
    appendMessage,
    loadMoreChat,
    pagingStateFirst,
    pagingStateLast,
  };
};
