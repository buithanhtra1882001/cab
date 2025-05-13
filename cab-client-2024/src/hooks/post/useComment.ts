import { useEffect, useState } from 'react';
import { IFetchCommentParams, IComment } from '../../models';
import { postService } from '../../services/post.service';
import _ from 'lodash';

export const useComment = ({
  pageNumber,
  visible,
  postId,
}: {
  pageNumber: number;
  visible: boolean;
  postId: string;
}) => {
  const [comments, setComments] = useState<IComment[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [hasMore, setHasMore] = useState<boolean>(false);
  const [pagingState, setPagingState] = useState<string>('');

  const refreshCommentClient = async () => {
    try {
      const {
        data,
        hasMore,
        pagingState: pagingStateResponse,
      } = await postService.getCommentByPostId({
        pageNumber: 1,
        pageSize: 1 * pageNumber,
        pagingState: '',
        postId,
      });

      setComments(data);
      setHasMore(hasMore);
      setPagingState(pagingStateResponse);
    } finally {
      setLoading(false);
    }
  };

  const getCommentWhenComment = async () => {
    try {
      const {
        data,
        hasMore,
        pagingState: pagingStateResponse,
      } = await postService.getCommentByPostId({
        pageNumber: 1,
        pageSize: 1,
        pagingState: '',
        postId,
      });

      setComments(data);
      setHasMore(hasMore);
      setPagingState(pagingStateResponse);
    } finally {
      setLoading(false);
    }
  };

  const getComment = async (payload: IFetchCommentParams) => {
    try {
      const {
        data,
        hasMore,
        pagingState: pagingStateResponse,
      } = await postService.getCommentByPostId({
        ...payload,
        pagingState,
      });

      setComments((prev) => _.uniqBy([...prev, ...data], 'id'));
      setHasMore(hasMore);
      setPagingState(pagingStateResponse);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!visible) {
      setComments([]);
      setPagingState('');
      return;
    }

    const payload: IFetchCommentParams = {
      pageNumber,
      pageSize: 1,
      pagingState: '',
      postId,
    };
    getComment(payload);
  }, [pageNumber, visible]);

  return {
    data: comments,
    loading,
    hasMore,
    getComment,
    getCommentWhenComment,
    refreshCommentClient,
  };
};
