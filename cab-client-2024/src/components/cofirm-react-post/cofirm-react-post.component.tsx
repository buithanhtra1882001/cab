import React from 'react';
import DialogComponent from '../dialog/dialog.component';

interface ConfirmReactPostProps {
  visible: boolean;
  isLike: boolean;
  onConfirm: () => void;
  onCancel: () => void;
  onClose: () => void;
}

const ConfirmReactPost = ({ visible, isLike, onConfirm, onCancel, onClose }: ConfirmReactPostProps) => {
  return (
    <div>
      <DialogComponent visible={visible} onClose={onClose} placement="CENTER">
        <div className="mt-2">
          <p className="text-slate-800 dark:text-slate-50 text-base font-medium">
            Bạn muốn {isLike ? 'thích' : 'không thích'} bài viết này?
          </p>
          <p className="text-slate-800 dark:text-slate-100 text-xs mt-2 font-medium">
            Lưu ý: Bạn chỉ có thể tương tác bài viết chỉ 1 lần!
          </p>

          <div className="flex justify-end gap-3 mt-5">
            <button
              className="text-slate-800  dark:text-slate-50 px-4 py-2 rounded-md border border-solid border-slate-500 text-sm cursor-pointer font-medium"
              onClick={onCancel}
            >
              Đóng
            </button>
            <button
              className="px-4 py-2 rounded-md bg-primary text-slate-50 text-sm cursor-pointer font-medium"
              onClick={onConfirm}
            >
              Đồng ý
            </button>
          </div>
        </div>
      </DialogComponent>
    </div>
  );
};

export default ConfirmReactPost;
