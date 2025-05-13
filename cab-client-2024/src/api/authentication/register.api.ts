import { IRegisterResponseData } from '../../models';
import { API } from '../Api';

export const registerAPI = (email: string, username: string, password: string, fullName: string) => {
  return API.post<IRegisterResponseData>(
    '/v1/identity-service/accounts/register',
    {
      email,
      username,
      password,
      fullName,
    },
    {
      headers: {
        'Content-Type': 'application/json',
        'clean-request': 'no-clean',
      },
    },
  );
};
