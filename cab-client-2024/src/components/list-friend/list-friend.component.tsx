import React from 'react';
import { useFriend } from '../../hooks/use-friend/useFriend';
import Avatar from '../avatar/avatar.component';
import { Link } from 'react-router-dom';
import { routePaths } from '../../routes/routes-path';

const limitList = 5;

const ListFriend = () => {
  const { data, totalFriend } = useFriend({ pageSize: limitList });

  return (
    <ul className="flex items-center">
      {data.map((friend) => (
        <li key={friend.userId}>
          <Link to={`/user/${friend.userId}`}>
            <Avatar avatar={friend.avatar} size="xs" label={friend.fullName} />
          </Link>
        </li>
      ))}

      {totalFriend ? (
        <Link to={`/${routePaths.listFriend}`} className="text-sm text-slate-800 dark:text-slate-50 ml-3">
          Xem tất cả
        </Link>
      ) : null}

      {totalFriend > limitList ? (
        <li>
          <Avatar avatar="" size="xs" label={`${totalFriend}+`} showFullLabel classNameLabel="text-xs" />
        </li>
      ) : null}
    </ul>
  );
};

export default ListFriend;
