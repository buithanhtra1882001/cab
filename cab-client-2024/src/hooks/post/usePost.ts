import { useEffect, useState } from 'react';
import { FetchPostParams, IPostModel } from '../../models';
import { postService } from '../../services/post.service';

export const usePostHook = ({ pageNumber }: { pageNumber: number }) => {
  const [posts, setPosts] = useState<IPostModel[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [hasMore, setHasMore] = useState<boolean>(false);

  const appendNewPost = (newPost: IPostModel) => {
    setPosts((prev) => [newPost, ...prev]);
  };

  const getPosts = async (payload: FetchPostParams) => {
    try {
      setLoading(true);
      const { data, hasMore } = await postService.getPosts(payload);

      setPosts((prev) => [...prev, ...data]);
      setHasMore(hasMore);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getPosts({
      pageNumber,
      pageSize: 10,
    });
  }, [pageNumber]);

  return {
    data: posts,
    loading,
    hasMore,
    appendNewPost,
  };
};
