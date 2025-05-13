import React from 'react';
import { Helmet } from 'react-helmet-async';
import ListFriendRequest from '../../components/list-friend-request/list-friend-request.component';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import _get from 'lodash/get';
import { useFriend } from '../../hooks/use-friend/useFriend';

export const ListFriendPage = () => {
  const userProfile = useSelector((state: RootState) => state.features.auth.profile);

  const { data, loading } = useFriend({ pageSize: 100 });

  // if (loading) {
  //   return (
  //     <div className="h-screen flex justify-center items-center">
  //       <Loading />
  //     </div>
  //   );
  // }

  return (
    <div className="min-h-screen">
      <Helmet>
        <title>Danh sách bạn bè - Cab</title>
        <meta name="description" content="Danh sách bạn bè" />
      </Helmet>

      <div className="container mt-16">
        <div className="pt-5">
          <p className="text-md font-medium text-slate-800 dark:text-slate-50 mb-4">Danh sách bạn bè</p>
          {data && !loading ? (
            <ListFriendRequest
              data={data}
              userId={_get(userProfile, 'userId', '')}
              onSuccess={() => {
                //
              }}
              listFriendPage
              hiddenAddFriend
            />
          ) : null}
        </div>
      </div>
    </div>
  );
};
