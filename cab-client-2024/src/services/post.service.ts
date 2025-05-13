import { API } from '../api';
import {
  FetchPostByUserIdParams,
  FetchPostParams,
  FetchPostVideoSuggestionParams,
  ICommentResponse,
  IFetchCommentParams,
  IFetchReplyCommentParams,
  IPostResponse,
  IReplyCommentParams,
  LikeCommentParams,
  IReplyCommentResponse,
  IPostModel,
} from '../models';
import { IHashTag, ISearchHashtagPayload } from '../types/hashtag';

const endPoint = {
  homePost: 'v1/post-service/posts/home-post',
  postDetail: 'v1/post-service/posts',
  postsByUser: 'v1/post-service/posts/get-posts-by-user',
  homeComment: 'v1/post-service/comments/get-comment-by-postId',
  homeReplyComment: 'v1/post-service/comments/get-reply-by-commentId',
  homeCreateReplyComment: 'v1/post-service/comments/reply',
  likeComment: 'v1/post-service/comments/like-toggle',
  homeLikePost: 'v1/post-service/posts/post/up',
  homeDisLikePost: 'v1/post-service/posts/post/down',
  hashTag: 'v1/post-service/posts/get-hashtag',
  searchHashTag: 'v1/post-service/posts/search-hashtag',
  postsTypeVideo: 'v1/post-service/postvideos/post-videos',
  postsVideoSuggest: 'v1/post-service/posts/get-videos-suggestion',
};

export const postService = {
  async getPosts(payload: FetchPostParams) {
    const { data } = await API.post<IPostResponse>(endPoint.homePost, payload);
    return data;
  },

  async getDetailPost(postId: string) {
    const { data } = await API.get<IPostModel>(`${endPoint.postDetail}/${postId}`);
    return data;
  },

  async getPostsTypeVideo(payload: FetchPostParams) {
    const { data } = await API.post<IPostResponse>(endPoint.postsTypeVideo, payload);
    return data;
  },
  async getPostsVideoSuggestion(payload: FetchPostVideoSuggestionParams) {
    const { data } = await API.post<IPostResponse>(endPoint.postsVideoSuggest, payload);
    return data;
  },
  async getPostsByUser(payload: FetchPostByUserIdParams) {
    const { data } = await API.post<IPostResponse>(endPoint.postsByUser, payload);
    return data;
  },

  async getCommentByPostId(payload: IFetchCommentParams) {
    const { data } = await API.post<ICommentResponse>(endPoint.homeComment, payload);
    return data;
  },

  async getReplyCommentByCommentId(payload: IFetchReplyCommentParams) {
    const { data } = await API.post<IReplyCommentResponse>(endPoint.homeReplyComment, payload);
    return data;
  },

  async replyComment(payload: IReplyCommentParams) {
    const { data } = await API.post<IReplyCommentResponse>(endPoint.homeCreateReplyComment, payload);
    return data;
  },

  async likeComment(payload: LikeCommentParams) {
    const { data } = await API.put<LikeCommentParams>(endPoint.likeComment, payload);
    return data;
  },

  async likePost(postId: string) {
    const { data } = await API.put<IReplyCommentResponse>(endPoint.homeLikePost, { postId });
    return data;
  },

  async diLikePost(postId: string) {
    const { data } = await API.put<IReplyCommentResponse>(endPoint.homeDisLikePost, { postId });
    return data;
  },

  async getHashTag(total?: number) {
    const url = `${endPoint.hashTag}/${total || 10}`;
    const { data } = await API.get<IHashTag[]>(url);
    return data;
  },

  async searchHashTag(payload: ISearchHashtagPayload) {
    const { data } = await API.post<IHashTag[]>(endPoint.searchHashTag, payload);
    return data;
  },
};
