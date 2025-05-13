import { useEffect, useState } from 'react';
import { IFetchReplyCommentParams, IReplyComment } from '../../models';
import { postService } from '../../services/post.service';
import _ from 'lodash';

export const useReplyComment = ({
  pageNumber,
  visible,
  commentId,
}: {
  pageNumber: number;
  visible: boolean;
  commentId: string;
}) => {
  const [replyComments, setReplyComments] = useState<IReplyComment[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [hasMore, setHasMore] = useState<boolean>(false);
  const [pagingState, setPagingState] = useState<string>('');

  const getReplyCommentWhenReply = async (commentId: string) => {
    try {
      const {
        data,
        hasMore,
        pagingState: pagingStateResponse,
      } = await postService.getReplyCommentByCommentId({
        pageNumber: 1,
        pageSize: 500, // improvement
        pagingState: '',
        commentId,
      });

      setReplyComments(data);
      setHasMore(hasMore);
      setPagingState(pagingStateResponse);
    } finally {
      setLoading(false);
    }
  };

  const getReplyComment = async (payload: IFetchReplyCommentParams) => {
    try {
      const {
        data,
        hasMore,
        pagingState: pagingStateResponse,
      } = await postService.getReplyCommentByCommentId({
        ...payload,
        pagingState,
      });

      setReplyComments((prev) => _.uniqBy([...prev, ...data], 'id'));
      setHasMore(hasMore);
      setPagingState(pagingStateResponse);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!visible) {
      setReplyComments([]);
      setPagingState('');
      return;
    }

    const payload: IFetchReplyCommentParams = {
      pageNumber,
      pageSize: 5,
      pagingState: '',
      commentId,
    };
    getReplyComment(payload);
  }, [pageNumber, visible]);

  return {
    data: replyComments,
    loading,
    hasMore,
    getReplyCommentWhenReply,
  };
};
