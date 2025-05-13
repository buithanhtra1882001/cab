import { API } from '..';
import { IListCreatorModel } from '../../models';

export const fetchCreatorsApi = () => {
  return new Promise<IListCreatorModel[]>((resolve, reject) => {
    API.get('/v1/user-service/users/get-request-creator')
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
