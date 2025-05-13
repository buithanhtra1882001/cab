/* eslint-disable react/self-closing-comp */
import React, { useEffect, useRef, useState } from 'react';
import classNames from 'classnames';
import { Camera } from 'lucide-react';
import DialogComponent from '../dialog/dialog.component';
import { useAvatar } from '../../hooks/user-avatar/useAvatar';
import ButtonPrimary from '../button-refactor/button-primary';
import ButtonSecondary from '../button-refactor/button-secondary';
import { userService } from '../../services/user.service';
import toast from 'react-hot-toast';

type AvatarSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl';

interface AvatarProps {
  avatar: string;
  label: string;
  size?: AvatarSize;
  uploaded?: boolean;
  preview?: boolean;
  onUploadSuccess?: () => void;
  classNameLabel?: string;
  showFullLabel?: boolean;
}

const Avatar = ({
  avatar,
  label,
  size = 'md',
  uploaded,
  onUploadSuccess,
  classNameLabel,
  showFullLabel,
  preview,
}: AvatarProps) => {
  const { onInputChange, imageReviewSrc, visibleUpload, onClose } = useAvatar();

  const fileRef = useRef<HTMLInputElement>(null);

  const [selfAvatar, setSelfAvatar] = useState<string>(avatar);
  const [isError, setError] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);
  const [visiblePreview, setVisiblePreview] = useState<boolean>(false);

  const displayUserName = (): string => {
    if (showFullLabel) {
      return label;
    }

    if (!label) {
      return 'N/A';
    }

    const splitUserName = label.split(' ');
    if (splitUserName.length === 1) {
      return label[0];
    }

    const firstName = label[0];
    const lastName = splitUserName[splitUserName.length - 1][0];

    return firstName + lastName;
  };

  const handleUploadAvatar = async () => {
    const avatarFile = fileRef?.current?.files;
    if (!avatarFile) {
      return;
    }

    try {
      setLoading(true);
      const data = await userService.updateAvatar(avatarFile[0]);
      setSelfAvatar(data[0].url);

      onUploadSuccess?.();
      onClose();
    } catch (error) {
      toast.error('Không thể cập nhật ảnh đại diện!');
    } finally {
      setLoading(false);
    }
  };

  const handleCLick = () => {
    if (preview) {
      setVisiblePreview(true);
    }
  };

  const avatarSize: Record<AvatarSize, string> = {
    xs: 'w-8 h-8',
    sm: 'w-10 h-10',
    md: 'w-16 h-16',
    lg: 'w-20 h-20',
    xl: 'w-24 h-24',
    xxl: 'w-42 h-42',
  };

  useEffect(() => {
    setSelfAvatar(avatar);
  }, [avatar]);

  return (
    <>
      <div
        className={classNames(`relative  z-10`, {
          'cursor-pointer hover:before:bg-slate-50 hover:before:bg-opacity-50 dark:hover:before:bg-opacity-20':
            uploaded,
          'cursor-pointer': preview,
        })}
        onClick={handleCLick}
      >
        {!isError ? (
          <div
            className={classNames({
              'relative overflow-hidden rounded-full md:border-[6px] border-gray-100 shrink-0 dark:border-slate-900 shadow':
                size === 'xxl',
            })}
            onClick={() => uploaded && setVisiblePreview(true)}
          >
            <img
              className={`avatar   rounded-full ${avatarSize[size]}`}
              src={selfAvatar || 'https://www.w3schools.com/howto/img_avatar.png'}
              alt="avatar"
              onError={() => {
                setError(true);
              }}
            />
          </div>
        ) : null}

        {isError ? (
          <span
            className={classNames(
              `absolute left-1/2 top-1/2 transform -translate-x-1/2 -translate-y-1/2
            text-slate-800 dark:text-slate-50 uppercase`,
              classNameLabel,
              {
                'text-xs': size === 'xs',
                'text-sm': size === 'sm',
                'text-lg': size === 'lg',
                'text-xl': size === 'xl',
                'text-2xl': size === 'xxl',
              },
            )}
          >
            {displayUserName()}
          </span>
        ) : null}

        {uploaded ? (
          <button
            className="absolute -bottom-3 left-1/2 -translate-x-1/2 bg-white shadow p-1.5 rounded-full sm:flex hidden"
            onClick={() => {
              if (!uploaded) {
                return;
              }
              fileRef.current?.click();
            }}
          >
            <Camera size={24} />
            <input hidden type="file" ref={fileRef} onChange={onInputChange} accept="image/*" />
          </button>
        ) : null}
      </div>

      <DialogComponent visible={visibleUpload} onClose={onClose} placement="CENTER" title="Đổi ảnh đại diện" size="lg">
        <div className="pt-10">
          <img src={imageReviewSrc} alt="" className="mx-auto h-64 object-cover" />

          <div className="mt-10 flex justify-end items-center">
            <ButtonSecondary className="mr-4" onClick={onClose}>
              Hủy bỏ
            </ButtonSecondary>
            <ButtonPrimary onClick={() => handleUploadAvatar()} loading={loading}>
              Cập nhật
            </ButtonPrimary>
          </div>
        </div>
      </DialogComponent>

      <DialogComponent
        visible={visiblePreview}
        onClose={() => setVisiblePreview(false)}
        placement="CENTER"
        title="Xem ảnh đại diện"
        size="lg"
      >
        <div className="pt-10">
          <img src={selfAvatar || 'https://www.w3schools.com/howto/img_avatar.png'} alt="" className="mx-auto h-96" />

          <div className="mt-10 flex justify-end items-center">
            <ButtonPrimary onClick={() => setVisiblePreview(false)} loading={loading}>
              Đóng
            </ButtonPrimary>
          </div>
        </div>
      </DialogComponent>
    </>
  );
};

export default Avatar;
