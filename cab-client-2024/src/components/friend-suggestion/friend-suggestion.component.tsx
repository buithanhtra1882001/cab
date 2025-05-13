import React, { useState } from 'react';
import {
  FriendRequestAction,
  FriendRequestType,
  IFriendSuggestion,
  ISendFriendRequestPayload,
} from '../../types/friend';
import { Link } from 'react-router-dom';
import ButtonPrimary from '../button-refactor/button-primary';
import _size from 'lodash/size';
import NoData from '../no-data/no-data.component';
import Avatar from '../avatar/avatar.component';
import { userService } from '../../services/user.service';
import ButtonSecondary from '../button-refactor/button-secondary';
import { useFollow } from '../../hooks/useFollow/useFollow';

interface ListFriendSuggestionProps {
  data: IFriendSuggestion[];
  userId: string;
  onSuccess: () => void;
}

const ListFriendSuggestion = ({ data, userId, onSuccess }: ListFriendSuggestionProps) => {
  const { handleFollow, handleUnFollow } = useFollow();

  const [loading, setLoading] = useState<boolean>(false);
  const [success, setSuccess] = useState<boolean>(false);

  const handleAddFriend = async (payload: ISendFriendRequestPayload) => {
    try {
      setLoading(true);
      await userService.sendFriendRequest(payload);
      setSuccess(true);

      onSuccess();
    } catch (error) {
      setSuccess(false);
    } finally {
      setLoading(false);
    }
  };

  const onAddFriend = async (user: IFriendSuggestion) => {
    const payload: ISendFriendRequestPayload = {
      requestUserId: userId,
      userId: user.requestUserId,
      statusAction: FriendRequestAction.SEND_REQUEST,
      typeRequest: FriendRequestType.FRIEND,
    };

    handleAddFriend(payload);
  };

  if (!_size(data)) {
    return (
      <div className="mt-16 mx-auto w-40">
        <NoData />
      </div>
    );
  }

  return (
    <div className="pr-5">
      <div className="space-y-3">
        {data.map((item) => (
          <div key={item.requestUserId}>
            <div
              className="rounded-md shadow-md dark:bg-zinc-800 p-5 
              transition-all duration-200 hover:shadow-lg"
            >
              <div className="flex gap-3">
                <Link to={`/user/${item.requestUserId}`}>
                  <Avatar avatar={item.avatar} label={item.requestFullName} size="sm" />
                </Link>

                <div>
                  <Link to={`/user/${item.requestUserId}`}>
                    <h3 className="text-slate-800 dark:text-slate-50 text-sm">{item.requestFullName}</h3>
                  </Link>

                  <div className="mt-3 flex gap-3">
                    <ButtonSecondary
                      onClick={async () => {
                        if (item.isFollow) {
                          await handleUnFollow(item.requestUserId);
                          onSuccess();
                        } else {
                          // onFollow(item.requestUserId);
                          await handleFollow(item.requestUserId);
                          onSuccess();
                        }
                      }}
                    >
                      {item.isFollow ? 'Hủy theo dõi' : 'Theo dõi'}
                    </ButtonSecondary>
                    <ButtonPrimary onClick={() => onAddFriend(item)}>Kết bạn</ButtonPrimary>
                  </div>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ListFriendSuggestion;
