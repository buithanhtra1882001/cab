import { ILoginResponseData, IRefreshTokenData } from '../../models';
import { API } from '../Api';

export const loginAPI = (email: string, password: string, passCode?: string) => {
  // const formData = new URLSearchParams();
  // formData.append('email', email);
  // formData.append('password', password);
  return API.post<ILoginResponseData>(
    '/v1/identity-service/accounts/login',
    { email, password, passCode },
    {
      headers: {
        // 'Content-Type': 'application/x-www-form-urlencoded',
        'Content-Type': 'application/json;charset=UTF-8',
        'clean-request': 'no-clean',
      },
    },
  );
};

export const loginThreeRdAPI = (provider: string, accessToken: string) => {
  return API.post<ILoginResponseData>(
    '/v1/identity-service/accounts/external-login',
    { provider, accessToken },
    {
      headers: {
        // 'Content-Type': 'application/x-www-form-urlencoded',
        'Content-Type': 'application/json;charset=UTF-8',
        'clean-request': 'no-clean',
      },
    },
  );
};

export const refreshTokenAPI = ({
  fingerprintHash,
  refreshToken,
}: {
  fingerprintHash: string;
  refreshToken: string;
}) => {
  return API.post<IRefreshTokenData>(
    '/v1/identity-service/tokens/refresh',
    { fingerprintHash, refreshToken },
    {
      headers: {
        'Content-Type': 'application/json;charset=UTF-8',
        'clean-request': 'no-clean',
      },
    },
  );
};
