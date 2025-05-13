import { useEffect, useState } from 'react';
import { FetchPostParams, IPostModel } from '../../models';
import { postService } from '../../services/post.service';

export const usePostVideosHook = ({ pageNumber }: { pageNumber?: number }) => {
  const [posts, setPosts] = useState<IPostModel[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [hasMore, setHasMore] = useState<boolean>(false);
  const [postDetail, setPostDetail] = useState<IPostModel | null>(null);

  const appendNewPost = (newPost: IPostModel) => {
    setPosts((prev) => [newPost, ...prev]);
  };

  const getPosts = async (payload: FetchPostParams) => {
    try {
      setLoading(true);
      const { data, hasMore } = await postService.getPostsTypeVideo(payload);

      setPosts((prev) => [...prev, ...data]);
      setHasMore(hasMore);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };

  const getDetailPost = async (id: string) => {
    try {
      setLoading(true);
      const data = await postService.getDetailPost(id);
      setPostDetail(data);
      return data;
    } catch {
      setPostDetail(null);
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
    postDetail,
    getDetailPost,
  };
};
