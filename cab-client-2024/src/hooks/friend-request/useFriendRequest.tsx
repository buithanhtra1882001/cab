import { useEffect, useState } from 'react';
import { userService } from '../../services/user.service';
import { IFriend, ISendFriendRequestPayload } from '../../types/friend';

export const useFriendRequest = () => {
  const [loading, setLoading] = useState<boolean>(false);

  const [loadingFriendRequest, setLoadingFriendRequest] = useState<boolean>(false);

  const [success, setSuccess] = useState<boolean>(false);

  const [listFriendRequest, setListFriendRequest] = useState<IFriend[] | null>(null);

  const handleAddFriend = async (payload: ISendFriendRequestPayload) => {
    try {
      setLoading(true);
      await userService.sendFriendRequest(payload);
      setSuccess(true);
    } catch (error) {
      setSuccess(false);
    } finally {
      setLoading(false);
    }
  };

  const getListFriendRequest = async () => {
    try {
      setLoadingFriendRequest(true);
      const data = await userService.getListFriendRequest();
      setListFriendRequest(data);
    } catch (error) {
      //
    } finally {
      setLoadingFriendRequest(false);
    }
  };

  useEffect(() => {
    getListFriendRequest();
  }, []);

  return {
    loading,
    handleAddFriend,
    success,
    listFriendRequest,
    getListFriendRequest,
    loadingFriendRequest,
  };
};
