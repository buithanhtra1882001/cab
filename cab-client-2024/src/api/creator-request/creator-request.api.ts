import { API } from '..';
import { IPostModel } from '../../models';

export const creatorRequest = () => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.post('v1/user-service/users/request-creator')
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
