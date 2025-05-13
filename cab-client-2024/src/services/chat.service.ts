import { API } from '../api';
import { IGetChatByIdPayload, IGetChatByIdResponse } from '../types/chat';

const endPoint = {
  chatDetail: 'v1/user-service/users/content-message',
};

export const chatService = {
  async getChatById(payload: IGetChatByIdPayload) {
    const { data } = await API.post<IGetChatByIdResponse>(endPoint.chatDetail, payload);
    return data;
  },
};
