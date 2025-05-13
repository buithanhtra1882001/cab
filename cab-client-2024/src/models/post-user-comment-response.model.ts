import { IUserCommentRepliesFromAPI } from './post-user-comment-reply.model';

export interface IUserCommentResponse {
  id: string;
  postId: string;
  userId: string;
  userFullName: string;
  avatar: string;
  content: string;
  upvotesCount: number;
  downvotesCount: number;
  status: number;
  replies: IUserCommentRepliesFromAPI;
  updatedAt: string;
  createdAt: string;
}

export type IUserCommentResponseFromAPI = {
  pagingState: string;
  pageSize: number;
  pageNumber: number;
  total: number;
  data: IUserCommentResponse[];
};

export type FetchCommentParams = {
  pageNumber?: number;
  pageSize?: number;
  pagingState?: string;
  total?: number;
};
