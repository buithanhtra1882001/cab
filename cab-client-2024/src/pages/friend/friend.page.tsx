import React, { useEffect, useState } from 'react';
import { Helmet } from 'react-helmet-async';
import { useFriendRequest } from '../../hooks/friend-request/useFriendRequest';
import ListFriendRequest from '../../components/list-friend-request/list-friend-request.component';
import ListFriendSuggestion from '../../components/friend-suggestion/friend-suggestion.component';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import _get from 'lodash/get';
import { useFriendSuggestion } from '../../hooks/friend-suggestion/useFriendSuggestion';
import { routePaths } from '../../routes/routes-path';
import { ITab, Tab } from '../../components/tab/tab.component';
import { useFriend } from '../../hooks/use-friend/useFriend';

const tabFriend: ITab[] = [
  {
    label: 'Lời mời kết bạn',
    value: '#',
  },
  {
    label: 'Danh sách bạn bè',
    value: routePaths.listFriend,
  },
];

export const FriendPage = () => {
  const userProfile = useSelector((state: RootState) => state.features.auth.profile);

  const { data: listFriend, loading } = useFriend({ pageSize: 100 });

  const [tabValue, setTabValue] = useState<string>(tabFriend[0].value);

  const { listFriendRequest, getListFriendRequest } = useFriendRequest();
  const { listFriendSuggestion, getListFriendSuggestion } = useFriendSuggestion();

  const handleAddFriendSuccess = async () => {
    await getListFriendRequest();
  };

  useEffect(() => {
    if (tabValue === tabFriend[0].value) {
      getListFriendRequest();
    }

    if (tabValue === tabFriend[1].value) {
      getListFriendSuggestion();
    }
  }, [tabValue]);

  return (
    <div className="min-h-screen">
      <Helmet>
        <title>Bạn bè - Cab</title>
        <meta name="description" content="Bạn bè" />
      </Helmet>

      <div className="container mt-16">
        <div className="">
          <div className="flex gap-8">
            <div className="max-w-sm w-full border-r border-solid border-slate-100 min-h-screen pt-5">
              <p className="text-md font-medium text-slate-800 dark:text-slate-50 mb-4">Những người bạn có thể biết</p>
              {listFriendSuggestion ? (
                <ListFriendSuggestion
                  data={listFriendSuggestion}
                  userId={_get(userProfile, 'userId', '')}
                  onSuccess={() => {
                    getListFriendSuggestion();
                  }}
                />
              ) : null}
            </div>
            <div className="flex-1 pt-5">
              <div className="mb-5">
                <Tab
                  defaultValue={tabValue}
                  items={tabFriend}
                  onChange={(value) => {
                    setTabValue(value);
                  }}
                />
              </div>

              {/* <div className="mb-4 flex justify-between">
                <p className="text-md font-medium text-slate-800 dark:text-slate-50">Lời mời kết bạn</p>
                <Link
                  className="text-md font-medium text-primary-6 dark:text-slate-50"
                  to={`/${routePaths.listFriend}`}
                >
                  Danh sách bạn bè
                </Link>
              </div> */}

              {tabValue === tabFriend[0].value ? (
                <div>
                  {listFriendRequest ? (
                    <ListFriendRequest
                      data={listFriendRequest}
                      userId={_get(userProfile, 'userId', '')}
                      onSuccess={handleAddFriendSuccess}
                    />
                  ) : null}
                </div>
              ) : null}

              {tabValue === tabFriend[1].value ? (
                <div className="mt-5">
                  {listFriendRequest ? (
                    <ListFriendRequest
                      data={listFriend}
                      userId={_get(userProfile, 'userId', '')}
                      onSuccess={handleAddFriendSuccess}
                      hiddenAddFriend
                    />
                  ) : null}
                </div>
              ) : null}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
