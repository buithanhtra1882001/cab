import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import * as yup from 'yup';
import { yupResolver } from '@hookform/resolvers/yup';
import InputComponent from '../input/input.component';
import ErrorValidateComponent from '../error-validate/error-validate.component';
import ButtonSecondary from '../button-refactor/button-secondary';
import ButtonPrimary from '../button-refactor/button-primary';
import { changePasswordApi } from '../../api';
import toast from 'react-hot-toast';

interface IFormValues {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

const schema = yup.object().shape({
  currentPassword: yup.string().required('Current password is required'),
  newPassword: yup.string().required('New password is required').min(6, 'Password must be at least 6 characters'),
  confirmPassword: yup
    .string()
    .oneOf([yup.ref('newPassword')], 'Passwords must match')
    .required('Confirm password is required'),
});

const ChangePasswordForm: React.FC = () => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IFormValues>({
    resolver: yupResolver(schema),
  });
  const [loading, setLoading] = useState<boolean>(false);
  const onSubmit = async (data: IFormValues) => {
    try {
      await changePasswordApi(data.currentPassword, data.newPassword);
      toast.success('Thay đổi mật khẩu thành công');
    } catch (error) {
      toast.error('Không thể thay đổi mật khẩu của bạn');
      return;
    } finally {
      setLoading(false);
    }
    // Handle form submission logic here
  };

  return (
    <form className="space-y-4 py-5 font-bold" onSubmit={handleSubmit(onSubmit)}>
      <div className="form-group">
        <div className="flex">
          <label
            htmlFor="currentPassword"
            className="flex items-center w-[30%] text-sm text-slate-800 dark:text-slate-50"
          >
            Mật khẩu hiện tại
          </label>
          <div className="w-full">
            <InputComponent
              type="password"
              placeholder="Mật khẩu hiện tại"
              id="currentPassword"
              register={register('currentPassword')}
            />
            <ErrorValidateComponent visible={!!errors.currentPassword}>
              {errors.currentPassword?.message}
            </ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group">
        <div className="flex">
          <label htmlFor="newPassword" className="flex items-center w-[30%] text-sm text-slate-800 dark:text-slate-50">
            Mật khẩu mới
          </label>
          <div className="w-full">
            <InputComponent
              type="password"
              placeholder="Mật khẩu mới"
              id="newPassword"
              register={register('newPassword')}
            />
            <ErrorValidateComponent visible={!!errors.newPassword}>
              {errors.newPassword?.message}
            </ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group">
        <div className="flex">
          <label
            htmlFor="confirmPassword"
            className="flex items-center w-[30%] text-sm text-slate-800 dark:text-slate-50"
          >
            Xác nhận mật khẩu mới
          </label>
          <div className="w-full">
            <InputComponent
              type="password"
              placeholder="Xác nhận mật khẩu mới"
              id="confirmPassword"
              register={register('confirmPassword')}
            />
            <ErrorValidateComponent visible={!!errors.confirmPassword}>
              {errors.confirmPassword?.message}
            </ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group flex justify-end items-center">
        <ButtonSecondary className="mr-4">Hủy bỏ</ButtonSecondary>
        <ButtonPrimary loading={loading}>Cập nhật</ButtonPrimary>
      </div>
    </form>
  );
};

export default ChangePasswordForm;
