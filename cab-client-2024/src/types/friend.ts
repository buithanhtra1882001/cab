export enum FriendRequestType {
  FRIEND = 'FRIEND',
  CREATOR = 'CREATOR',
}

export enum FriendRequestAction {
  CANCEL = 'CANCEL',
  SEND_REQUEST = 'SEND_REQUEST',
}

export interface IFriend {
  userId: string;
  fullName: string;
  avatar: string;
  isOnline: boolean;
}

export interface IFriendSuggestion {
  requestUserId: string;
  requestFullName: string;
  avatar: string;
  requestType: number;
  isFollow: boolean;
}

export interface IGetFriendPayload {
  pageNumber: number;
  pageSize: number;
}

export interface IGetFriendResponse {
  total: number;
  data: IFriend[];
}

export interface ISendFriendRequestPayload {
  userId: string;
  requestUserId: string;
  statusAction: FriendRequestAction;
  typeRequest: FriendRequestType;
}

export interface ISendFriendRequestResponse {
  message: string;
  isSuccess: boolean;
}

export interface IAddFriendRequestPayload {
  userId: string;
  requestUserId: string;
  acceptStatus: 'NORMAL' | 'ACCEPTED';
}
