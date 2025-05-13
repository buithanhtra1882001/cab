import { API } from '..';
import { FetchGetListUserParams } from '../../models/share.model';
import { IUserInfoshares } from '../../components/Share';

export const GetListFriendApi = (params: FetchGetListUserParams) => {
  return new Promise<IUserInfoshares>((resolve, reject) => {
    API.get('', {
      params,
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
