import { FetchShareParams, IShareModel } from '../../models/share.model';
import { sharePostsApi } from '../../api/share/fetch-share.api';
import { useCallback, useState } from 'react';

export const useSharePost = () => {
  const [postShareItems, setPostShareItems] = useState<IShareModel>();

  const [isSharePost, setIsSharePost] = useState(false);

  const fetchPostShare = useCallback((params: FetchShareParams) => {
    sharePostsApi(params)
      .then((data) => {
        setPostShareItems(data);
        setIsSharePost(true);
      })
      .catch(() => {
        setIsSharePost(false);
      })
      .finally();
  }, []);

  return {
    postShareItems,
    isSharePost,
    fetchPostShare,
  };
};
