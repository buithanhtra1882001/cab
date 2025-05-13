export interface IFriendRequestModel {
  requestUserId?: string | number;
  avatar?: string;
  requestFullName?: string;
  requestType?: number;
}

export interface IListFriendRequestModel {
  userId?: string;
  fullName?: string;
  avatar?: string;
  status?: 'ACCEPTED' | 'DELETE' | 'PENDING';
}

export interface IAddFriendRequestModel {
  message: string;
  isSuccess: boolean;
}

export interface IGetFriendRequestModel {
  userId?: string;
  fullName?: string;
  avatar?: string;
  status?: 'ACCEPTED' | 'DELETE' | 'PENDING';
}

export interface IAcceptFriendParamsModel {
  userId?: string;
  requestUserId?: string;
  acceptStatus?: string;
}

export type FetchFriendRequestParams = {
  pageNumber?: number;
  pageSize?: number;
};

export type FetchAddFriendRequestParams = {
  userId?: string;
  requestUserId?: string | number;
  statusAction?: string;
  typeRequest?: string;
};
