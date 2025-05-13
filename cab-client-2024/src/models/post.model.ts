import { IUserCommentResponse } from './post-user-comment-response.model';

export interface IPostModel {
  url: string;
  id: string;
  userAvatar: string;
  userId: string;
  userFullName: string;
  postType: string;
  categoryId: string;
  hashtag_Ids: string;
  hashtags: string;
  content: string;
  likesCount: string;
  commentsCount: string;
  isDonateOpen: boolean;
  isViolence: boolean;
  updatedAt: string;
  createdAt: string;
  title: string;
  viewCount: number;
  voteUpCount: number;
  voteDownCount: number;
  userCommentResponses: IUserCommentResponse[];
  currentUserHasVoteUp: boolean;
  currentUserHasVoteDown: boolean;
  comments: IComment[];
  postImageResponses: {
    imageId: string;
    url: string;
    isViolence: boolean;
  }[];
  postVideoResponses: {
    videoId: string;
    url: string;
    isViolence: boolean;
  }[];
}

export type IPostResponse = {
  pageSize: number;
  pageNumber: number;
  pagingState: string;
  total: number;
  hasMore: boolean;
  data: IPostModel[];
};

export type ICommentResponse = {
  pageSize: number;
  pageNumber: number;
  pagingState: string;
  total: number;
  hasMore: boolean;
  data: IComment[];
};

export type IReplyCommentResponse = {
  pageSize: number;
  pageNumber: number;
  pagingState: string;
  total: number;
  hasMore: boolean;
  data: IReplyComment[];
};

export type FetchPostParams = {
  pageNumber?: number;
  pageSize?: number;
};

export type FetchPostVideoSuggestionParams = {
  pageNumber?: number;
  pageSize?: number;
  postId: string;
};

export type FetchPostByUserIdParams = {
  pageNumber?: number;
  pageSize?: number;
  userId: string;
};

export type IFetchCommentParams = {
  postId: string;
  pagingState: string;
  pageSize: number;
  pageNumber: number;
};

export type IFetchReplyCommentParams = {
  commentId: string;
  pagingState: string;
  pageSize: number;
  pageNumber: number;
};

export type IReplyCommentParams = {
  replyToCommentId: string;
  content: string;
};

export type LikeCommentParams = {
  commentId: string;
  type: number;
};

export type LikeDislikePostParams = {
  postId?: string;
  flag?: string;
};

export interface IComment {
  id: string;
  postId: string;
  userId: string;
  userFullName: string;
  avatar: string;
  content: string;
  upvotesCount: number;
  downvotesCount: number;
  status: number;
  replies: IComment[];
  updatedAt: Date;
  createdAt: Date;
  totalReply: number;
  currentUserHasLike: boolean;
  currentUserHasUnlike: boolean;
}

export interface IReplyComment {
  id: string;
  commentId: string;
  userId: string;
  userFullName: string;
  avatar: string;
  parentReplyId: string;
  replyLevel: number;
  content: string;
  upvotesCount: string;
  downvotesCount: string;
  status: number;
  updatedAt: Date;
  createdAt: Date;
}
