import { useState } from 'react';
import { userService } from '../../services/user.service';

export const useFollow = () => {
  const [loading, setLoading] = useState<boolean>(false);
  const [success, setSuccess] = useState<boolean>(false);

  const handleFollow = async (userFollowId: string) => {
    try {
      setLoading(true);
      await userService.followUser(userFollowId);
      setSuccess(true);
    } catch (error) {
      setSuccess(false);
    } finally {
      setLoading(false);
    }
  };

  const handleUnFollow = async (userFollowId: string) => {
    try {
      setLoading(true);
      await userService.unFollowUser(userFollowId);
      setSuccess(true);
    } catch (error) {
      setSuccess(false);
    } finally {
      setLoading(false);
    }
  };

  const handleUnFriend = async (userFollowId: string) => {
    try {
      setLoading(true);
      await userService.unFriend(userFollowId);
      setSuccess(true);
    } catch (error) {
      setSuccess(false);
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    success,
    handleFollow,
    handleUnFollow,
    handleUnFriend,
  };
};
