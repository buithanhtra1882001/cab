import { useEffect, useState } from 'react';
import { IUserProfile } from '../../models';
import { userService } from '../../services/user.service';

export const useUserProfile = (userId: string) => {
  const [profile, setProfile] = useState<IUserProfile | null>(null);
  const [loading, setLoading] = useState<boolean>(false);

  const getUserProfile = async () => {
    try {
      setLoading(true);
      const { data } = await userService.profileUser(userId);
      setProfile(data);
      return data;
    } catch {
      setProfile(null);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getUserProfile();
  }, [userId]);

  return {
    loading,
    profile,
    getUserProfile,
  };
};
