import { GetListFriendApi } from '../../api/share/fetch-list-friend.api';
import { useCallback, useState } from 'react';
import { FetchGetListUserParams } from '../../models/share.model';
import { userListShare } from '../../components/Share/mocks/user-list-share.mock';
import { IUserInfoshares } from '../../components/Share';
import _ from 'lodash';

export const useUseSharePostFriend = () => {
  const LIMIT_LOAD_ITEMS_POST = 6;

  const [userListShareFriendItems, setUserListShareFriendItems] = useState<IUserInfoshares>();

  const [paramsGetListUser, setParamsGetListUser] = useState<FetchGetListUserParams>();

  const [isLoadingGetListUser, setIsLoadingGetListUser] = useState(false);

  const [isEndFetchingGetListUser, setIsEndFetchingGetListUser] = useState(false);

  const fetchUserListShareFriend = useCallback(() => {
    let params: FetchGetListUserParams;
    if (!paramsGetListUser) {
      params = { pageSize: LIMIT_LOAD_ITEMS_POST, pageNumber: 1 };
    } else {
      params = {
        pageNumber: (paramsGetListUser?.pageNumber ?? 1) + 1,
        pageSize: paramsGetListUser?.pageSize,
      };
    }
    const { pageSize = LIMIT_LOAD_ITEMS_POST, pageNumber = 1 } = params;
    setParamsGetListUser(params);
    setIsLoadingGetListUser(true);
    GetListFriendApi({ pageSize, pageNumber })
      .then((data) => {
        if (data.userInfo.length === 0) {
          setUserListShareFriendItems(userListShare);
        } else {
          setUserListShareFriendItems(data);
        }

        if (_.isEmpty(data.userInfo)) {
          setIsEndFetchingGetListUser(false);
        } else {
          setIsEndFetchingGetListUser(true);
        }
      })
      .catch(() => setUserListShareFriendItems(userListShare))
      .finally(() => setIsLoadingGetListUser(false));
  }, []);

  return {
    userListShareFriendItems,
    isLoadingGetListUser,
    paramsGetListUser,
    isEndFetchingGetListUser,
    fetchUserListShareFriend,
  };
};
