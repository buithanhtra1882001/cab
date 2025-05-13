export interface SendMessagePayload {
  RecipientUserId: string;
  Content: string;
}

export interface IGetChatByIdPayload {
  friendUserId: string;
  pageNumber: number;
  pageSize: number;
  pagingStateFirst: string;
  pagingStateLast: string;
}

export interface IChat {
  senderUserId: string;
  recipientUserId: string;
  content: string;
  createdAt: string;
  senderName: string;
}

export interface IGetChatByIdResponse {
  pagingStateFirst: string;
  pagingStateLast: string;
  total: number;
  hasNext: boolean;
  data: IChat[];
}
