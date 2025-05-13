export interface IGroup {
  groupName: string;
  groupDescription: string;
  groupType: 0; // Loại nhóm là 0
  groupTagline: string;
  createdByUser: string; // UUID của người tạo nhóm
  privacySettings: string; // Các cài đặt quyền riêng tư, có thể là "public" hoặc "private"
  rules: string; // Quy tắc của nhóm
  location: string; // Vị trí của nhóm (thành phố, quốc gia, vv)
  websiteURL: string; // URL trang web của nhóm
  contactEmail: string; // Email liên hệ của nhóm
  tagList: string; // Danh sách các tag liên quan đến nhóm
  approvalRequired: boolean; // Cờ yêu cầu phê duyệt khi tham gia nhóm
}
export interface GroupElement {
  id: string;
  groupName: string;
  groupDescription: string;
  groupType: 0 | 1 | 2;
  groupTagline: string;
  coverPhoto: string | null;
  profilePicture: string | null;
  lastActivityDate: string;
  memberCount: number;
  privacySettings: string;
  rules: string;
  location: string;
  websiteURL: string;
  contactEmail: string;
  tagList: string;
  approvalRequired: boolean;
  createdByUser: string;
  groupAvatarUrl: string;
  groupCoverImageUrl: string;
}
export interface GroupDatas {
  joinedTheGroup: boolean;
  userStatus: string;
  group: GroupElement;
}
export interface GroupData {
  pageSize: number;
  pageNumber: number;
  total: number;
  elements: GroupDatas[];
}
export interface ApiRoleResponse {
  httpCode: number;
  message: string;
  data: string;
}
export interface ApiResponse {
  httpCode: number;
  message: string;
  data: GroupData;
}
export interface ApiResponseSuggestions {
  httpCode: number;
  message: string;
  data: GroupDatas[];
}
export interface ApiDetailResponse {
  httpCode: number;
  message: string;
  data: GroupElement;
}
export interface UserElement {
  userId: string;
  fullName: string;
  avatar: string | null;
}

export interface UserData {
  pageSize: number;
  pageNumber: number;
  totalPages: number;
  totalRecords: number;
  elements: UserElement[];
}

export interface ApiUserResponse {
  httpCode: number;
  message: string;
  data: UserData;
}
export interface CreateGroupRequest {
  groupName: string;
  groupDescription: string;
  groupType: number;
  groupTagline: string;
  privacySettings: string;
  rules: string;
  location: string;
  websiteURL: string;
  contactEmail: string;
  tagList: string;
  approvalRequired: boolean;
  requestPostReview: boolean;
  groupCoverImageUrl: string;
  groupAvatarUrl: string;
  categoryId: string;
}
