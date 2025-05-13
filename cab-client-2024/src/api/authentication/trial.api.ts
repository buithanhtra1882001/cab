import { API } from '../Api';

export const trialApi = async (deviceId: string, captcha = 'mock') => {
  return API.post<{ access_token: string; id_token: string }>('/account/login/fromDeviceId', {
    deviceId,
    captcha,
  });
};
