import React, { useState } from 'react';
import DialogComponent from '../dialog/dialog.component';
import { IUserProfile } from '../../models';
import EditProfileGeneral from './edit-profile-general';
import EditProfiledContact from './edit-profile-contact';
import EditProfileOrther from './edit-profile-orther';
import ChangePasswordForm from './edit-change-password';
import { useNavigate } from 'react-router-dom';

interface EditProfiledDialogProps {
  profile: IUserProfile;

  onUpdateSuccess: () => void;
}

const EditProfiledDialog = ({ profile, onUpdateSuccess }: EditProfiledDialogProps) => {
  const [visible, setVisible] = useState<boolean>(false);
  const [activeTab, setActiveTab] = useState('overview');
  const navigate = useNavigate();

  const handleTabClick = (tabName) => {
    setActiveTab(tabName);
  };
  const onUpdateSuccessfull = () => {
    onUpdateSuccess();
    setVisible(false);
  };

  return (
    <>
      <div id="edit-profile" onClick={() => setVisible(true)} className="text-sm text-blue-500 cursor-pointer">
        Chỉnh sửa
      </div>
      <DialogComponent
        size="xl"
        visible={visible}
        onClose={() => setVisible(false)}
        title="Cập nhật tài khoản"
        placement="CENTER"
      >
        <nav className="flex gap-0.5 -mb-px text-gray-600 font-medium text-[15px] dark:text-white max-md:w-full max-md:overflow-x-auto border-b">
          <div
            className={`inline-block cursor-pointer py-3 leading-8 px-3.5 border-b-2 ${
              activeTab === 'overview' ? 'border-black text-black' : ''
            }`}
            onClick={() => handleTabClick('overview')}
          >
            Tổng quan
          </div>
          <div
            className={`inline-block cursor-pointer py-3 leading-8 px-3.5 ${
              activeTab === 'contact' ? 'border-b-2 border-black text-black' : ''
            }`}
            onClick={() => handleTabClick('contact')}
          >
            Thông tin liên lạc
          </div>

          <div
            className={`inline-block cursor-pointer py-3 leading-8 px-3.5 ${
              activeTab === 'other' ? 'border-b-2 border-black text-black' : ''
            }`}
            onClick={() => handleTabClick('other')}
          >
            Khác
          </div>
          <div
            className={`inline-block cursor-pointer py-3 leading-8 px-3.5 ${
              activeTab === 'change-password' ? 'border-b-2 border-black text-black' : ''
            }`}
            onClick={() => handleTabClick('change-password')}
          >
            Thay đổi password
          </div>
          <div className={`inline-block cursor-pointer py-3 leading-8 px-3.5 `} onClick={() => navigate('/settings')}>
            Cài đặt donate
          </div>
        </nav>
        <div className="md:py-12 md:px-20 p-6 overflow-hidden text-black text-sm">
          {activeTab === 'overview' && <EditProfileGeneral profile={profile} onUpdateSuccess={onUpdateSuccessfull} />}
          {activeTab === 'contact' && <EditProfiledContact profile={profile} onUpdateSuccess={onUpdateSuccessfull} />}
          {activeTab === 'other' && <EditProfileOrther profile={profile} onUpdateSuccess={onUpdateSuccessfull} />}
          {activeTab === 'change-password' && <ChangePasswordForm />}
        </div>
      </DialogComponent>
    </>
  );
};

export default EditProfiledDialog;
