import { useEffect, useState } from 'react';
import { userService } from '../../services/user.service';
import { IFriendSuggestion } from '../../types/friend';

export const useFriendSuggestion = () => {
  const [loading, setLoading] = useState<boolean>(false);
  const [listFriendSuggestion, setListFriendSuggestion] = useState<IFriendSuggestion[] | null>(null);

  const getListFriendSuggestion = async () => {
    try {
      setLoading(true);
      const data = await userService.getListFriendSuggestion();
      setListFriendSuggestion(data);
    } catch (error) {
      //
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getListFriendSuggestion();
  }, []);

  return {
    loading,
    listFriendSuggestion,
    getListFriendSuggestion,
  };
};
