import { useEffect, useState } from 'react';
import { IFriend, IGetFriendPayload } from '../../types/friend';
import { userService } from '../../services/user.service';

export const useFriend = ({ pageSize }: { pageSize: number }) => {
  const [friends, setFriends] = useState<IFriend[]>([]);
  const [totalFriend, setTotalFriend] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(false);

  const getFriends = async () => {
    try {
      setLoading(true);
      const payload: IGetFriendPayload = {
        pageNumber: 1,
        pageSize,
      };
      const { data, total } = await userService.getFriends(payload);
      setTotalFriend(total);
      setFriends(data);
    } catch (error) {
      //
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getFriends();
  }, []);

  return {
    data: friends,
    loading,
    totalFriend,
  };
};
