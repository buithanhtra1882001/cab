import { IPostModel } from '../../models';

export interface IShareProps {
  post?: IPostModel;
  UserId?: string;
}

export interface IUserInfoshares {
  userInfo: IUserInfo[];
}

export interface IUserInfo {
  id: string | number;
  name: string;
  avatar: string;
}
