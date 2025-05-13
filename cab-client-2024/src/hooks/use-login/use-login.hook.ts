import { useCallback, useState } from 'react';
import { ILoginThreeRdFormData } from './use-login.type';
import { CoreAuthenticationStore } from '../../store';
import { loginContent } from './login-content.mock';
import { ILoginPayLoad } from '../../types/authentication';
import { authenticationServices } from '../../services/authentication.service';
import { LocalStorageService } from '../../configuration';
import _get from 'lodash/get';
import { AUTH_ERROR_CODE } from '../../constants';

export const useLogin = (onLoginSuccess: () => void) => {
  const [loading, setLoading] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState<string>('');
  const [isErrorEmailConfirm, setIsErrorEmailConfirm] = useState<boolean>(false);

  const handleLogin = async (payload: ILoginPayLoad) => {
    try {
      setErrorMessage('');
      setLoading(true);
      const { fingerprintHash, refreshToken, accessToken } = await authenticationServices.login(payload);

      LocalStorageService.setItem('accessToken', accessToken);
      LocalStorageService.setItem('fingerprintHash', fingerprintHash);
      LocalStorageService.setItem('refreshToken', refreshToken);

      onLoginSuccess();
    } catch (error) {
      let errorMessage = 'Sai thông tin đăng nhập';

      const errorCode = _get(error, 'response.data.title', '');
      if (errorCode === AUTH_ERROR_CODE.EMAIL_NOT_VERIFIED) {
        errorMessage = 'Vui lòng xác nhận email';
        setIsErrorEmailConfirm(true);
      }

      setErrorMessage(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    handleLogin,
    errorMessage,
    isErrorEmailConfirm,
  };
};

export const useLoginThreeRd = () => {
  const handleLoginThreeRd = useCallback((data: ILoginThreeRdFormData) => {
    CoreAuthenticationStore.loginThreeRdAction(data?.provider, data?.accessToken, data.passCode);
  }, []);

  return {
    handleLoginThreeRd,
    contentThreeRdData: loginContent,
  };
};
