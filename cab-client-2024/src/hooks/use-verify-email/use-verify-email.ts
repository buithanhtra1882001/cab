import { useEffect, useState } from 'react';
import { authenticationServices } from '../../services/authentication.service';
import _get from 'lodash/get';

interface VerifyEmailParams {
  token: string;
  userId: string;
  onVerifySuccess: () => void;
}

export const useVerifyEmail = ({ token, userId, onVerifySuccess }: VerifyEmailParams) => {
  const [loading, setLoading] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState<string>('');

  const handleVerify = async () => {
    try {
      setErrorMessage('');
      setLoading(true);
      await authenticationServices.verifyEmail({ token, userId });

      onVerifySuccess();
    } catch (error) {
      const errorCode = _get(error, 'response.data.detail', '');

      setErrorMessage(errorCode);
    } finally {
      setLoading(false);
    }
  };

  const handleResendVerifyEmail = async (email: string) => {
    try {
      setLoading(true);
      await authenticationServices.resendVerifyEmail(email);
    } catch (error) {
      //
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    handleVerify();
  }, []);

  return {
    loading,
    handleVerify,
    handleResendVerifyEmail,
    errorMessage,
  };
};
