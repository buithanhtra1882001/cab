import React, { useMemo, useRef, useState } from 'react';
import { IUserProfile } from '../../models';
import Avatar from '../avatar/avatar.component';
import _get from 'lodash/get';
import { Camera, CircleCheck } from 'lucide-react';
import ButtonPrimary from '../button-refactor/button-primary';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import { useFriendRequest } from '../../hooks/friend-request/useFriendRequest';
import { FriendRequestAction, FriendRequestType, ISendFriendRequestPayload } from '../../types/friend';
import ButtonSecondary from '../button-refactor/button-secondary';
import { useFollow } from '../../hooks/useFollow/useFollow';
import { useAvatar } from '../../hooks/user-avatar/useAvatar';
import DialogComponent from '../dialog/dialog.component';
import { userService } from '../../services/user.service';
import toast from 'react-hot-toast';
import { Tooltip } from 'antd';
import { useNavigate } from 'react-router-dom';
import ListFriend from '../list-friend/list-friend.component';
import DonateUser from '../donate-user/donate-user-component';

interface ProfileBannerProps {
  profile: IUserProfile;
  onUpdateSuccess: () => void;
  onContactSuccess?: () => void;
  changeTab?: any;
  tabActive?: any;
}

const ProfileBanner = ({ profile, onUpdateSuccess, onContactSuccess, changeTab, tabActive }: ProfileBannerProps) => {
  const [loading, setLoading] = useState<boolean>(false);
  const [coverImagePreview, setCoverImagePreview] = useState<string>('');
  const navigate = useNavigate();
  const { onInputChange, imageReviewSrc, visibleUpload, onClose } = useAvatar();

  const fileRef = useRef<HTMLInputElement>(null);

  const { handleAddFriend, success } = useFriendRequest();
  const { handleFollow, handleUnFollow, handleUnFriend } = useFollow();

  const userProfile = useSelector((state: RootState) => state.features.auth.profile);

  const isUserProfile = useMemo(() => {
    return userProfile?.userId === profile.userId;
  }, [userProfile, profile]);
  const isContentCreator = useMemo(() => {
    return userProfile?.userType === 'CONTENT_CREATOR';
  }, [userProfile, profile]);

  const onAddFriend = async () => {
    if (success) {
      return;
    }

    const payload: ISendFriendRequestPayload = {
      requestUserId: _get(userProfile, 'userId', ''),
      userId: profile.userId,

      statusAction: FriendRequestAction.SEND_REQUEST,
      typeRequest: FriendRequestType.FRIEND,
    };

    await handleAddFriend(payload);
    onContactSuccess?.();
  };

  const handleUploadCoverImage = async () => {
    const avatarFile = fileRef?.current?.files;
    if (!avatarFile) {
      return;
    }

    try {
      setLoading(true);
      const data = await userService.updateCoverImage(avatarFile[0]);

      setCoverImagePreview(data[0].url);
      onClose();
    } catch (error) {
      toast.error('Không thể cập nhật ảnh bìa!');
    } finally {
      setLoading(false);
    }
  };
  const opentEditProfile = () => {
    document.getElementById('edit-profile')?.click();
  };

  return (
    <div className="bg-white shadow lg:rounded-b-2xl lg:-mt-10 dark:bg-dark2">
      <div className="relative overflow-hidden w-full lg:h-72 h-48">
        <img src={profile?.coverImage} alt="" className="h-full w-full object-cover inset-0" />
        <div className="w-full bottom-0 absolute left-0 bg-gradient-to-t from-black/60 pt-20 z-10" />
        <div className="absolute bottom-0 right-0 m-4 z-20">
          <div className="flex items-center gap-3">
            {isUserProfile ? (
              <>
                {!isContentCreator && (
                  <button
                    className="button bg-white/20 text-white flex items-center gap-2 backdrop-blur-small"
                    onClick={() => navigate('/settings')}
                  >
                    Yêu cầu xác thực content creator
                  </button>
                )}
                <button
                  className="button bg-black/10 text-white flex items-center gap-2 backdrop-blur-small"
                  onClick={() => opentEditProfile()}
                >
                  Chỉnh sửa trang cá nhân
                </button>
              </>
            ) : (
              <div className="flex gap-3">
                <ButtonSecondary
                  onClick={async () => {
                    if (profile.isFollow) {
                      await handleUnFollow(profile.userId);
                    } else {
                      await handleFollow(profile.userId);
                    }
                    onContactSuccess?.();
                  }}
                >
                  {profile.isFollow ? 'Hủy theo dõi' : 'Theo dõi'}
                </ButtonSecondary>

                {profile.isFriend ? (
                  <ButtonPrimary
                    onClick={async () => {
                      await handleUnFriend(profile.userId);
                      onContactSuccess?.();
                    }}
                  >
                    Hủy kết bạn
                  </ButtonPrimary>
                ) : (
                  <ButtonPrimary
                    onClick={async () => {
                      if (success || profile.isFriendRequest) {
                        await handleUnFriend(profile.userId);
                        onContactSuccess?.();
                      } else {
                        onAddFriend();
                      }
                    }}
                  >
                    {profile.isFriendRequest ? 'Hủy yêu cầu' : 'Thêm bạn bè'}
                  </ButtonPrimary>
                )}
              </div>
            )}
          </div>
        </div>
        <div className="absolute top-4 right-4">
          {isUserProfile ? (
            <>
              <Tooltip color="blue" placement="bottom" title="Đổi ảnh bìa">
                <button
                  type="button"
                  className="custom-target-icon inline-block p-4 rounded-full mr-3 font-bold text-center uppercase align-middle transition-all  cursor-pointer bg-fuchsia-500/0 leading-pro text-xs ease-soft-in tracking-tight-soft bg-150 bg-x-25 hover:bg-blue-500/25 hover:scale-102 active:bg-fuchsia/45 text-black"
                  onClick={() => {
                    fileRef.current?.click();
                  }}
                >
                  <Camera size={24} className="" />
                </button>
              </Tooltip>
              <input hidden type="file" ref={fileRef} onChange={onInputChange} accept="image/*" />

              <DialogComponent
                visible={visibleUpload}
                onClose={onClose}
                placement="CENTER"
                title="Đổi ảnh bìa"
                size="lg"
              >
                <div className="pt-10">
                  <img src={imageReviewSrc} alt="" className="mx-auto h-64 object-cover" />

                  <div className="mt-10 flex justify-end items-center">
                    <ButtonSecondary className="mr-4" onClick={onClose}>
                      Hủy bỏ
                    </ButtonSecondary>
                    <ButtonPrimary onClick={() => handleUploadCoverImage()} loading={loading}>
                      Cập nhật
                    </ButtonPrimary>
                  </div>
                </div>
              </DialogComponent>
            </>
          ) : null}
        </div>
      </div>
      <div className="p-3">
        <div className="flex flex-col justify-center md:items-center lg:-mt-48 -mt-28">
          <div className="relative lg:h-48 lg:w-48 w-28 h-28 mb-4 z-10">
            <div className="">
              <Avatar
                avatar={_get(profile, 'avatar', '')}
                label={_get(profile, 'username', '')}
                size="xxl"
                uploaded={isUserProfile}
                onUploadSuccess={onUpdateSuccess}
                preview={!isUserProfile}
              />
            </div>
          </div>
          <h3 className="md:text-3xl text-base font-bold text-black dark:text-white flex items-center">
            {_get(profile, 'fullname', '')}
            {_get(profile, 'userType') === 'CONTENT_CREATOR' ? (
              <CircleCheck color="green" size={24} />
            ) : (
              profile?.isVerifyEmail && <CircleCheck color="black" size={24} />
            )}
          </h3>
          <p className="mt-2 max-w-xl text-sm md:font-normal font-light text-center ">
            {_get(profile, 'description', '')}
          </p>
          <ListFriend />
        </div>
      </div>
      <div className="flex items-center justify-between mt-3 border-t border-gray-100 px-2 max-lg:flex-col dark:border-slate-700">
        {userProfile?.userId !== profile.userId && (
          <div className="flex items-center gap-2 text-sm py-2 pr-1 max-md:w-full lg:order-2">
            <DonateUser user={profile} />
          </div>
        )}
        <nav className="flex gap-0.5 rounded-xl -mb-px text-gray-600 font-medium text-[15px]  dark:text-white max-md:w-full max-md:overflow-x-auto">
          <div
            className={`cursor-pointer  inline-block  py-3 leading-8 px-3.5 border-b-2  ${
              tabActive === 'timeline' && 'border-blue-600 text-blue-600'
            } `}
            onClick={() => changeTab && changeTab('timeline')}
          >
            Tường
          </div>
          <div
            className={`cursor-pointer  inline-block  py-3 leading-8 px-3.5 border-b-2  ${
              tabActive === 'freind' && 'border-blue-600 text-blue-600'
            } `}
            onClick={() => navigate('/friend')}
          >
            Bạn bè
            <span className="text-xs pl-2 font-normal lg:inline-block hidden"> {_get(profile, 'totalFriend', '')}</span>
          </div>

          <div
            className={`cursor-pointer  inline-block  py-3 leading-8 px-3.5 border-b-2  ${
              tabActive === 'group' && 'border-blue-600 text-blue-600'
            } `}
            onClick={() => changeTab && changeTab('group')}
          >
            Group
          </div>
          <div
            className={`cursor-pointer  inline-block  py-3 leading-8 px-3.5 border-b-2  ${
              tabActive === 'history-donate' && 'border-blue-600 text-blue-600'
            } `}
            onClick={() => changeTab && changeTab('history-donate')}
          >
            Lịch sử Donate
          </div>
        </nav>
      </div>
    </div>
  );
};
export default ProfileBanner;
