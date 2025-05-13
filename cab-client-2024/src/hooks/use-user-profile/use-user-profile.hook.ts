import { useObserver } from 'mobx-react';
import { useEffect } from 'react';
import { CoreUserProfileStore, CoreAuthenticationStore } from '../../store';

export const useUserProfile = () => {
  const profile = useObserver(CoreUserProfileStore.profileSelector);

  const isChangedPasswordSuccess = useObserver(CoreAuthenticationStore.isChangedPasswordSuccessSelector);

  useEffect(() => {
    CoreUserProfileStore.fetchProfileAction();
  }, []);

  return {
    profile,
    isChangedPasswordSuccess,
    changePassword: CoreAuthenticationStore.changePasswordAction,
  };
};
