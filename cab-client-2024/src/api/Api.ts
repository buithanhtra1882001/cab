import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
import { ApiError, refreshTokenAPI } from '.';
import { PUBLIC_API_ENDPOINT } from '../constants/platform';
import { errorResponse } from '../utils';
import { LocalStorageService } from '../configuration';

const API = axios.create({
  baseURL: `${PUBLIC_API_ENDPOINT}`,
  headers: {
    'Content-Type': 'application/json;charset=UTF-8',
  },
});

API.interceptors.request.use(
  (config) => {
    if (config.headers) {
      const accessToken = LocalStorageService.getItem('accessToken');
      if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
      }
    }
    return config;
  },
  (error: AxiosError): Promise<AxiosError> => {
    return Promise.reject(error);
  },
);

API.interceptors.response.use(
  (response: AxiosResponse): AxiosResponse => {
    return response;
  },
  async (error: AxiosError): Promise<AxiosError> => {
    const originalRequest = error.config as AxiosRequestConfig & {
      _retry?: boolean;
    };

    const { statusCode } = errorResponse(error);

    if (statusCode === 401 && !originalRequest?._retry) {
      LocalStorageService.removeItem('accessToken');

      originalRequest._retry = true;

      try {
        const fingerprintHashLocal = LocalStorageService.getItem('fingerprintHash');
        const refreshTokenLocal = LocalStorageService.getItem('refreshToken');

        const {
          data: { accessToken, refreshToken, fingerprintHash },
        } = await refreshTokenAPI({
          fingerprintHash: fingerprintHashLocal,
          refreshToken: refreshTokenLocal,
        });

        LocalStorageService.setItem('fingerprintHash', fingerprintHash);
        LocalStorageService.setItem('refreshToken', refreshToken);
        LocalStorageService.setItem('accessToken', accessToken);
      } catch (error) {
        // window.location.href = routePaths.login;
      }

      return API(originalRequest);
    }
    return Promise.reject(error);
  },
);

const ThrowApiError = (error: ApiError) => {
  if (error.isAxiosError) throw error.response?.data.title ?? (error?.response?.data?.message || 1);
  throw error;
};

const setApiAccessToken = (accessToken: string | undefined) => {
  if (accessToken) API.defaults.headers.common.Authorization = `Bearer ${accessToken}`;
  else delete API.defaults.headers.common.Authorization;
};

export { API, ThrowApiError, setApiAccessToken };
