import { useEffect, useState } from 'react';
import { IPostModel } from '../../models';
import { postService } from '../../services/post.service';

export const usePostDetail = ({ id }: { id?: string }) => {
  const [loading, setLoading] = useState<boolean>(false);
  const [postDetail, setPostDetail] = useState<IPostModel | null>(null);

  const getDetailPost = async (id: string) => {
    try {
      if (id) setPostDetail(null);
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
    if (!id) return;
    getDetailPost(id);
  }, [id]);

  return {
    loading,
    postDetail,
    getDetailPost,
  };
};
