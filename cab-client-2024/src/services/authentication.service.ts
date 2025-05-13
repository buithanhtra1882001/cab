import { API } from '../api';
import {
  ILoginPayLoad,
  ILoginResponse,
  IRegisterPayload,
  IVerifyEmailPayload,
  IVerifyEmailResponse,
} from '../types/authentication';

const endPoint = {
  login: 'v1/identity-service/accounts/login',
  register: 'v1/identity-service/accounts/register',
  verifyEmail: 'v1/identity-service/accounts/email/confirm',
  resendVerifyEmail: 'v1/identity-service/accounts/resend-email-confirmation',
};

export const authenticationServices = {
  async login(payload: ILoginPayLoad) {
    const { data } = await API.post<ILoginResponse>(endPoint.login, payload);
    return data;
  },

  async register(payload: IRegisterPayload) {
    const { data } = await API.post(endPoint.register, payload);
    return data;
  },

  async verifyEmail(payload: IVerifyEmailPayload) {
    const { data } = await API.get<IVerifyEmailResponse>(endPoint.verifyEmail, { params: payload });
    return data;
  },

  async resendVerifyEmail(email: string) {
    const { data } = await API.get(endPoint.resendVerifyEmail, {
      params: {
        email,
      },
    });
    return data;
  },
};
