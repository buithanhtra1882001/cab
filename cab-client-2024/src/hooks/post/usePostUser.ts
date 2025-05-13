import { useEffect, useState } from 'react';
import { FetchPostByUserIdParams, IPostModel } from '../../models';
import { postService } from '../../services/post.service';

export const usePostUser = ({ pageNumber, userId }: { pageNumber: number; userId: string }) => {
  const [posts, setPosts] = useState<IPostModel[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [hasMore, setHasMore] = useState<boolean>(false);

  const appendNewPost = (newPost: IPostModel) => {
    setPosts((prev) => [newPost, ...prev]);
  };

  const loadMorePost = async (payload: FetchPostByUserIdParams) => {
    try {
      setLoading(true);
      const { data, hasMore } = await postService.getPostsByUser(payload);

      setPosts((prev) => [...prev, ...data]);
      setHasMore(hasMore);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };

  const getPostsUser = async (payload: FetchPostByUserIdParams) => {
    try {
      setLoading(true);
      const { data, hasMore } = await postService.getPostsByUser(payload);

      // setPosts((prev) => [...prev, ...data]);
      setPosts(data);
      setHasMore(hasMore);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!userId) {
      return;
    }

    getPostsUser({
      pageNumber: 1,
      pageSize: 10,
      userId,
    });
  }, [userId]);

  useEffect(() => {
    if (!userId || pageNumber === 1) {
      return;
    }

    loadMorePost({
      pageNumber,
      pageSize: 10,
      userId,
    });
  }, [pageNumber]);

  return {
    data: posts,
    loading,
    hasMore,
    appendNewPost,
  };
};
