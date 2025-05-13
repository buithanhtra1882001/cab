import { FetchShareFriendParams } from '../../models/share.model';
import { sharePostsFriendApi } from '../../api/share/fetch-share-friend.api';
import { useCallback, useState } from 'react';

export const useSharePostFriend = () => {
  const [postShareFriendItems, setPostShareFriendItems] = useState<String[]>();

  const [isSharePost, setIsSharePost] = useState(false);

  const fetchPostShareFriend = useCallback((params: FetchShareFriendParams) => {
    sharePostsFriendApi(params)
      .then((data) => {
        setPostShareFriendItems(data);
        setIsSharePost(true);
      })
      .catch(() => {
        setIsSharePost(false);
      })
      .finally();
  }, []);

  return {
    postShareFriendItems,
    isSharePost,
    fetchPostShareFriend,
  };
};
