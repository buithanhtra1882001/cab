import { API } from '../Api';

export const fetchAccountAppCcuApi = () => {
  return new Promise<number>((resolve, reject) => {
    API.get<number>('/account/app-ccu')
      .then((response) => resolve(response.data || 0))
      .catch(reject);
  });
};
