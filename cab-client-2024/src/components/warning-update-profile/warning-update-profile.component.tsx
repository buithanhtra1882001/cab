import React from 'react';
import DialogComponent from '../dialog/dialog.component';
import { InfoIcon } from 'lucide-react';
import SelectHobbiesDialog from '../edit-profile-dialog/select-hobbies-dialog';

interface WarningUpdateProfileProps {
  visible: boolean;
  onClose: () => void;
  profile: any;
}

const WarningUpdateProfile = ({ profile, visible, onClose }: WarningUpdateProfileProps) => {
  return (
    <>
      <DialogComponent visible={visible} onClose={onClose} title="Cập nhật trang cá nhân">
        <div className="mt-2">
          <div className="flex justify-center py-6">
            <InfoIcon size={32} className="text-yellow-500" />
          </div>
          <p className="text-slate-8 dark:text-slate-50 text-sm font-medium text-center">
            Bạn cần cập nhật trang cá nhân để sử dụng các chức năng khác của chúng tôi!
          </p>
        </div>
      </DialogComponent>
      <SelectHobbiesDialog profile={profile} />
    </>
  );
};

export default WarningUpdateProfile;
