export interface IShareModel {
  id: string;
  postId: string;
  userId: string;
  shareLink: string;
  updatedAt: string;
  createdAt: string;
  isPublic: boolean;
}

export type FetchShareParams = {
  userId?: string;
  postId?: string;
  shareLink?: string;
  isPublic?: boolean;
};

export type FetchGetListUserParams = {
  pageSize?: number;
  pageNumber?: number;
};

export type FetchShareFriendParams = {
  userId?: string;
  postId?: string;
  sharedUserIds?: string[];
  shareLink?: string;
  isPublic?: boolean;
};
