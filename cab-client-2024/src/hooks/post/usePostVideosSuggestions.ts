import { useEffect, useState } from 'react';
import { FetchPostVideoSuggestionParams, IPostModel } from '../../models';
import { postService } from '../../services/post.service';

export const usePostVideosSuggestionHook = ({ postId, pageNumber }: { postId: any; pageNumber: number }) => {
  const [posts, setPosts] = useState<IPostModel[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [hasMore, setHasMore] = useState<boolean>(false);

  const appendNewPost = (newPost: IPostModel) => {
    setPosts((prev) => [newPost, ...prev]);
  };

  const getPosts = async (payload: FetchPostVideoSuggestionParams) => {
    try {
      setLoading(true);
      const { data, hasMore } = await postService.getPostsVideoSuggestion(payload);

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
      postId,
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
