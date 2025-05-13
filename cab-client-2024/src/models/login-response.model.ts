export interface ILoginResponseData {
  accessToken: string;
  expiresIn: string;
  fingerprintHash: string;
  refreshToken: string;
  tokenType: string;
}

export interface IRefreshTokenData {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
  fingerprintHash: string;
}
