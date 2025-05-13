import { API } from '..';
import { ICategory, IUserProfile, UserProfileStats } from '../../models';

export const fetchUserProfileByIdApi = (id: string) => {
  return new Promise<IUserProfile>((resolve, reject) => {
    API.get<IUserProfile>(`/v1/user-service/users/${id}`)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
export const fetchStatisticalByUserIdApi = (id: string) => {
  return new Promise<UserProfileStats>((resolve, reject) => {
    API.get<UserProfileStats>(`v1/post-service/users/statistical-user?userid=${id}`)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
export const fetchAllCategory = () => {
  return new Promise<[ICategory]>((resolve, reject) => {
    API.get<[ICategory]>(`/v1/user-service/users/get-categories`)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
