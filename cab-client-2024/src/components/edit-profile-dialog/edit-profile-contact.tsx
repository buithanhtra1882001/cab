import React, { useState } from 'react';
import { IUserProfile } from '../../models';
import InputComponent from '../input/input.component';
import * as yup from 'yup';
import ButtonSecondary from '../button-refactor/button-secondary';
import ButtonPrimary from '../button-refactor/button-primary';
import { SubmitHandler, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { userService } from '../../services/user.service';
import toast from 'react-hot-toast';
import ErrorValidateComponent from '../error-validate/error-validate.component';

interface EditProfiledDialogProps {
  profile: IUserProfile;
  onUpdateSuccess: () => void;
}

const schema = yup.object().shape({
  email: yup.string().email('Email không hợp lệ').required('Vui lòng nhập email'),
});

interface IFormValues {
  email: string;
  phone?: string;
  city?: string;
  homeLand?: string;
}

const EditProfiledContact = ({ profile, onUpdateSuccess }: EditProfiledDialogProps) => {
  const { phone, city, email, homeLand } = profile;
  const [visible, setVisible] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IFormValues>({
    resolver: yupResolver(schema),
    defaultValues: {
      phone,
      email,
      city,
      homeLand,
    },
  });

  const onSubmit: SubmitHandler<IFormValues> = async (formValues) => {
    try {
      setLoading(true);

      const payload = {
        ...profile,
        ...formValues,
        email: profile.email,
      };
      await userService.updateProfile(payload as IUserProfile);

      onUpdateSuccess();
      setVisible(false);
    } catch (error) {
      toast.error('Không thể cập nhật tài khoản của bạn');
      setVisible(false);
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="space-y-4 py-5 font-bold" onSubmit={handleSubmit(onSubmit)}>
      <div className="form-group ">
        <div className="flex">
          <label htmlFor="email" className=" flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Email
          </label>
          <div className="w-full">
            <InputComponent placeholder="Email" id="email" register={register('email')} readOnly />
            <ErrorValidateComponent visible={!!errors.email}>{errors.email?.message}</ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group ">
        <div className="flex">
          <label htmlFor="phone" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Điện thoại
          </label>
          <div className="w-full">
            <InputComponent type="tel" placeholder="Ngày sinh" id="phone" register={register('phone')} />
            <ErrorValidateComponent visible={!!errors.phone}>{errors.phone?.message}</ErrorValidateComponent>
          </div>
        </div>
      </div>

      <div className="form-group ">
        <div className="flex">
          <label htmlFor="city" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Nơi ở hiện tại
          </label>
          <div className="w-full">
            <InputComponent placeholder="Thành phố" id="city" register={register('city')} />
            <ErrorValidateComponent visible={!!errors.city}>{errors.city?.message}</ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group ">
        <div className="flex">
          <label htmlFor="homeLand" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Quê quán
          </label>
          <div className="w-full">
            <InputComponent placeholder="Quê quán" id="homeLand" register={register('homeLand')} />
            <ErrorValidateComponent visible={!!errors.homeLand}>{errors.homeLand?.message}</ErrorValidateComponent>
          </div>
        </div>
      </div>

      <div className="form-group flex justify-end items-center">
        <ButtonSecondary className="mr-4" onClick={() => setVisible(false)}>
          Hủy bỏ
        </ButtonSecondary>
        <ButtonPrimary loading={loading}>Cập nhật</ButtonPrimary>
      </div>
    </form>
  );
};

export default EditProfiledContact;
