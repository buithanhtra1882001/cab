import { API } from '..';
import { FetchShareParams, IShareModel } from '../../models/share.model';

export const sharePostsApi = (params: FetchShareParams) => {
  return new Promise<IShareModel>((resolve, reject) => {
    API.post('/v1/post-service/posts/shares', params)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
