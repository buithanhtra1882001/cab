export interface ILoginPayLoad {
  email: string;
  password: string;
  passCode?: string;
}

export interface ILoginResponse {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
  fingerprintHash: string;
}

export interface IRegisterPayload {
  email: string;
  fullName: string;
  username: string;
  password: string;
  code: string;
}

export interface IVerifyEmailPayload {
  token: string;
  userId: string;
}

export interface IVerifyEmailResponse {
  message: string;
}
