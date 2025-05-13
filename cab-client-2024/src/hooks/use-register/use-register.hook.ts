import { useState } from 'react';
import { IRegisterPayload } from '../../types/authentication';
import { authenticationServices } from '../../services/authentication.service';

export const useRegister = (onRegisterSuccess: () => void) => {
  const [loading, setLoading] = useState<boolean>(false);

  const handleRegister = async (payload: IRegisterPayload) => {
    try {
      setLoading(true);
      await authenticationServices.register(payload);
      onRegisterSuccess();
    } catch (error) {
      //
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    handleRegister,
  };
};
