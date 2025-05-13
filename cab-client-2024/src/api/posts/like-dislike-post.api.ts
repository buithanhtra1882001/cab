import { API } from '..';
import { LikeDislikePostParams } from '../../models';

export const likeDislikePostApi = (params: LikeDislikePostParams) => {
  return new Promise<boolean>((resolve, reject) => {
    API.put(`/v1/post-service/posts/post/${params?.flag === 'like' ? 'up' : 'down'}`, {
      postId: params?.postId,
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
