export interface IUserProfile {
  avatar: string;
  balance: number;
  canReceiveDonation: boolean;
  categoryFavorites: any; // Adjust the type as necessary
  city: string;
  company: string;
  coverImage: string;
  createdAt: string;
  description: string;
  dob: string;
  email: string;
  fullname: string;
  homeLand: string;
  identityCardNumber: string;
  isFollow: boolean;
  isFriend: boolean;
  isFriendRequest: boolean;
  isShowCompany: boolean;
  isShowHomeLand: boolean;
  isShowMarry: boolean;
  isShowSchool: boolean;
  isShowSexualOrientation: boolean;
  isUpdateProfile: boolean;
  marry: number;
  phone: string;
  school: string;
  sex: string;
  sexualOrientation: number;
  totalFriend: number;
  updatedAt: string;
  userId: string;
  userType: string;
  username: string;
  coin: string;
  isVerifyEmail: boolean;
  isCreateRequestReciveDonate: boolean;
}

export interface IUploadAvatarResponse {
  id: string;
  fileName: string;
  url: string;
  status: string;
  createdAt: Date;
}

export interface IUpdateProfilePayload {
  username: string;
  email: string;
  dob: string;
  gender: string;
  city: string;
  description: string;
}
export interface UserProfileStats {
  post: number;
  like: number;
  disLike: number;
  comment: number;
  newFriend: number;
  isUpdateProfile: boolean;
}
export interface ICategory {
  id: string;
  name: string;
  avatar: string;
}
