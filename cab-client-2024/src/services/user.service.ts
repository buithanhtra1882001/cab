import { API } from '../api';
import { IUploadAvatarResponse, IUserProfile } from '../models';
import { IDataResponse } from '../types/common';
import {
  IAddFriendRequestPayload,
  IFriend,
  IFriendSuggestion,
  IGetFriendPayload,
  IGetFriendResponse,
  ISendFriendRequestPayload,
  ISendFriendRequestResponse,
} from '../types/friend';

const endPoint = {
  profile: 'v1/user-service/me',
  profileUser: 'v1/user-service/users',
  updateProfile: 'v1/user-service/users',
  uploadAvatar: 'v1/user-service/users/upload-avatar',
  uploadCoverImage: 'v1/user-service/users/upload-cover-image',
  getFriends: '/v1/user-service/users/friend-isonline',
  sendFriendRequest: 'v1/user-service/users/add-friend-request',

  getListFriendRequest: 'v1/user-service/users/get-friend-request',
  getListFriendSuggestion: 'v1/user-service/users/get-friend-suggestion',
  followUser: 'v1/user-service/users/add-follower',

  addFriend: 'v1/user-service/users/add-friend',

  unFriend: 'v1/user-service/users/unfriend',
  unFollow: 'v1/user-service/users/unfollower',
};

export const userService = {
  async profile() {
    const { data } = await API.get<IDataResponse<IUserProfile>>(endPoint.profile);
    return data;
  },

  async profileUser(userId: string) {
    const { data } = await API.get<IDataResponse<IUserProfile>>(`${endPoint.profileUser}/${userId}`);
    return data;
  },

  async updateProfile(payload: IUserProfile) {
    const { data } = await API.post<IDataResponse<Omit<IUserProfile, 'email'>>>(endPoint.updateProfile, payload);
    return data;
  },

  async updateAvatar(avatar: File) {
    const formData = new FormData();
    formData.append('file', avatar);

    const { data } = await API.post<IUploadAvatarResponse[]>(endPoint.uploadAvatar, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return data;
  },

  async updateCoverImage(avatar: File) {
    const formData = new FormData();
    formData.append('file', avatar);

    const { data } = await API.post<IUploadAvatarResponse[]>(endPoint.uploadCoverImage, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return data;
  },

  async getFriends(payload: IGetFriendPayload) {
    const { data } = await API.post<IGetFriendResponse>(endPoint.getFriends, payload);
    return data;
  },

  async sendFriendRequest(payload: ISendFriendRequestPayload) {
    const { data } = await API.post<ISendFriendRequestResponse>(endPoint.sendFriendRequest, payload);
    return data;
  },

  async getListFriendRequest() {
    const { data } = await API.post<IFriend[]>(endPoint.getListFriendRequest);
    return data;
  },

  async getListFriendSuggestion() {
    const { data } = await API.get<IFriendSuggestion[]>(endPoint.getListFriendSuggestion);
    return data;
  },

  async addFriend(payload: IAddFriendRequestPayload) {
    const { data } = await API.post<ISendFriendRequestResponse>(endPoint.addFriend, payload);
    return data;
  },

  async followUser(userFollowId: string) {
    const { data } = await API.post(`${endPoint.followUser}?userFollowId=${userFollowId}`);
    return data;
  },

  async unFollowUser(followId: string) {
    const { data } = await API.delete(`${endPoint.unFollow}/${followId}`);
    return data;
  },

  async unFriend(friendId: string) {
    const { data } = await API.delete(`${endPoint.unFriend}/${friendId}`);
    return data;
  },
};
