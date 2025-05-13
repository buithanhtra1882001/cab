import { API } from '..';
import { FetchShareFriendParams } from '../../models/share.model';

export const sharePostsFriendApi = (params: FetchShareFriendParams) => {
  return new Promise<String[]>((resolve, reject) => {
    API.post('/v1/post-service/posts/shares-post-users', params)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
