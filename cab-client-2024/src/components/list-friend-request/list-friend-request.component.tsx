import React, { useState } from 'react';
import { IAddFriendRequestPayload, IFriend } from '../../types/friend';
import ButtonPrimary from '../button-refactor/button-primary';
import { Link } from 'react-router-dom';
import { userService } from '../../services/user.service';
import _size from 'lodash/size';
import classNames from 'classnames';

// import toast from 'react-hot-toast';

interface ListFriendRequestProps {
  data: IFriend[];
  userId: string;
  onSuccess: () => void;
  listFriendPage?: boolean;
  hiddenAddFriend?: boolean;
}

const ListFriendRequest = ({ data, userId, onSuccess, listFriendPage, hiddenAddFriend }: ListFriendRequestProps) => {
  const [loading, setLoading] = useState<boolean>(false);
  const [success, setSuccess] = useState<boolean>(false);

  const acceptFriend = async (payload: IAddFriendRequestPayload) => {
    try {
      setLoading(true);
      await userService.addFriend(payload);
      setSuccess(true);

      onSuccess();
    } catch (error) {
      setSuccess(false);
    } finally {
      setLoading(false);
    }
  };

  const onAddFriend = (user: IFriend) => {
    const payload: IAddFriendRequestPayload = {
      requestUserId: user.userId,
      acceptStatus: 'ACCEPTED',
      userId,
    };

    acceptFriend(payload);
  };

  if (!_size(data)) {
    return (
      <div className="mt-16 text-center">
        <p className="text-sm font-medium text-slate-800 dark:text-slate-50">Không có lời mời kết bạn</p>
      </div>
    );
  }

  return (
    <div
      className={classNames(`grid gap-5`, {
        'xl:grid-cols-3 2xl:grid-cols-4': !listFriendPage,
        'xl:grid-cols-4 2xl:grid-cols-5': listFriendPage,
      })}
    >
      {data.map((item) => (
        <div key={item.userId}>
          <div
            className="rounded-md shadow-md bg-slate-50 dark:bg-zinc-800 p-5 
            transition-all duration-200 hover:shadow-lg"
          >
            <Link to={`/user/${item.userId}`}>
              <img src={item.avatar} alt={item.fullName} className="h-56 object-cover w-full" />
            </Link>

            <Link to={`/user/${item.userId}`}>
              <h3 className="mt-3 font-medium text-slate-800 dark:text-slate-50 text-base">{item.fullName}</h3>
            </Link>

            {!hiddenAddFriend ? (
              <div className="mt-6">
                <ButtonPrimary className="w-full flex justify-center" onClick={() => onAddFriend(item)}>
                  Chấp nhận
                </ButtonPrimary>

                {/* <ButtonSecondary className="w-full flex justify-center mt-3">Từ chối</ButtonSecondary> */}
              </div>
            ) : null}
          </div>
        </div>
      ))}
    </div>
  );
};

export default ListFriendRequest;
