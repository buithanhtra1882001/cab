import { API } from '../api';
import {
  ApiDetailResponse,
  ApiResponse,
  ApiResponseSuggestions,
  ApiRoleResponse,
  ApiUserResponse,
  CreateGroupRequest,
  FetchPostParams,
} from '../models';

const endPoint = {
  groups: 'v1/group-service/group/get-list-group',
  detail: 'v1/group-service/group/get-group-detail',
  groupSuggestion: 'v1/group-service/group/get-list-suggested-groups',
  groupOfUser: '/v1/group-service/group/get-list-of-joined-user-groups',
  groupPermission: '/v1/group-service/group/get-member-permissions-in-group',
  groupMembers: '/v1/group-service/group/get-list-of-group-members',
  groupMemberRequest: '/v1/group-service/group/list-group-member-pending-approval',
  joinGroup: '/v1/group-service/group/join-group',
  cancelJoinGroup: '/v1/group-service/group/cancel-request',
  groupRequestJoinOfUser: '/v1/group-service/group/get-list-of-group-request-join',
  createGroup: '/v1/group-service/group/add-group',
};

export const groupService = {
  async getGroup(payload: FetchPostParams) {
    const queryString = toQueryParams(payload);
    const { data } = await API.get<ApiResponse>(`${endPoint.groups}?${queryString}`);
    return data;
  },
  async getGroupDetail(groupId: string) {
    const { data } = await API.get<ApiDetailResponse>(`${endPoint.detail}?groupId=${groupId}`);
    return data;
  },
  async getGroupSuggestion() {
    const { data } = await API.get<ApiResponseSuggestions>(`${endPoint.groupSuggestion}`);
    return data;
  },
  async getGroupOfUser(payload: FetchPostParams) {
    const queryString = toQueryParams(payload);
    const { data } = await API.get<ApiResponse>(`${endPoint.groupOfUser}?${queryString}`);
    return data;
  },
  async getGroupPermission(groupId: string) {
    const { data } = await API.get<ApiRoleResponse>(`${endPoint.groupPermission}?groupId=${groupId}`);
    return data;
  },
  async getGroupMembers(groupId: string, payload: FetchPostParams) {
    const queryString = toQueryParams(payload);
    const { data } = await API.get<ApiUserResponse>(`${endPoint.groupMembers}?GroupID=${groupId}&${queryString}`);
    return data;
  },
  async getGroupMemberRequest(groupId: string, payload: FetchPostParams) {
    const queryString = toQueryParams(payload);
    const { data } = await API.get<ApiUserResponse>(`${endPoint.groupMemberRequest}?GroupID=${groupId}&${queryString}`);
    return data;
  },
  async joinGroup(groupID: string, joinMethod: number, joinReason: string) {
    const { data } = await API.post(`${endPoint.joinGroup}`, { groupID, joinMethod, joinReason });
    return data;
  },
  async cancelJoinGroup(groupID: string) {
    const { data } = await API.post(`${endPoint.cancelJoinGroup}`, { groupID });
    return data;
  },
  async getGroupRequestJoinOfUser(payload: FetchPostParams) {
    const queryString = toQueryParams(payload);
    const { data } = await API.get<ApiResponse>(`${endPoint.groupRequestJoinOfUser}?${queryString}`);
    return data;
  },
  async createGroup(payload: CreateGroupRequest) {
    const { data } = await API.post(`${endPoint.createGroup}`, payload);
    return data;
  },
};

export function toQueryParams(payload: Record<string, any>): string {
  const queryParams = Object.entries(payload)
    .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
    .join('&');
  return queryParams;
}
