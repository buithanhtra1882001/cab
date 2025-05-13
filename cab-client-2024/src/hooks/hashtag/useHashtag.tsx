import { useEffect, useState } from 'react';
import { IHashTag, ISearchHashtagPayload } from '../../types/hashtag';
import { postService } from '../../services/post.service';

export const useHashTag = () => {
  const [hashtags, setHashTags] = useState<IHashTag[] | null>(null);
  const [loading, setLoading] = useState<boolean>(false);

  const getHashTags = async () => {
    try {
      setLoading(true);
      const data = await postService.getHashTag(5);
      setHashTags(data);
    } catch (error) {
      //
    } finally {
      setLoading(false);
    }
  };

  const searchHashTags = async (keyword: string) => {
    try {
      setLoading(true);
      const payload: ISearchHashtagPayload = {
        keyword,
        totalRecord: 5,
      };
      const data = await postService.searchHashTag(payload);
      setHashTags(data);
    } catch (error) {
      //
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getHashTags();
  }, []);

  return {
    hashtags,
    loading,
    getHashTags,
    searchHashTags,
  };
};
