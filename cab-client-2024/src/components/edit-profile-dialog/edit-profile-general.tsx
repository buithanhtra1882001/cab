import React, { useState } from 'react';
import { IUserProfile } from '../../models';
import InputComponent from '../input/input.component';
import * as yup from 'yup';
import Select, { IOption } from '../../select/select.component';
import ButtonSecondary from '../button-refactor/button-secondary';
import ButtonPrimary from '../button-refactor/button-primary';
import { SubmitHandler, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { userService } from '../../services/user.service';
import toast from 'react-hot-toast';
import ErrorValidateComponent from '../error-validate/error-validate.component';

interface EditProfileGeneralProps {
  profile: IUserProfile;

  onUpdateSuccess: () => void;
}

const schema = yup.object().shape({
  username: yup.string().required('Vui lòng nhập tên người dùng'),
  email: yup.string().email('Email không hợp lệ').required('Vui lòng nhập email'),
  dob: yup.string().required('Ngày sinh không được bỏ trống'),
  sex: yup.string().required('Vui lòng nhập giới tính'),
  customSex: yup.string(),
  city: yup.string().required('Vui lòng nhập tên thành phố'),
  description: yup.string().required('Vui lòng nhập mô tả'),
  fullname: yup.string().required('Vui lòng nhập họ tên'),
});

interface IFormValues {
  username: string;
  email: string;
  dob: string;
  sex: string;
  customSex?: string;
  city: string;
  description: string;
  fullname: string;
}

const options: IOption[] = [
  {
    label: 'Nam',
    value: 'male',
  },
  {
    label: 'Nữ',
    value: 'female',
  },
  {
    label: 'Tùy chọn',
    value: 'other',
  },
];

const EditProfileGeneral = ({ profile, onUpdateSuccess }: EditProfileGeneralProps) => {
  const { dob, city, description, sex, username, email, fullname } = profile;
  const [visible, setVisible] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<IFormValues>({
    resolver: yupResolver(schema),
    defaultValues: {
      dob,
      city,
      description,
      sex,
      username,
      email,
      fullname,
    },
  });

  const onSubmit: SubmitHandler<IFormValues> = async (formValues) => {
    try {
      setLoading(true);
      const gender = formValues.sex === 'other' ? formValues.customSex : formValues.sex;

      const payload = {
        ...profile,
        ...formValues,
        email: profile.email,
        sex: gender || 'Chưa rõ',
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
          <label htmlFor="fullname" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Họ Tên
          </label>
          <div className="w-full">
            <InputComponent placeholder="Tên tài khoản" id="fullname" register={register('fullname')} />
            <ErrorValidateComponent visible={!!errors.fullname}>{errors.fullname?.message}</ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group ">
        <div className="flex">
          <label htmlFor="gender" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Giới tính
          </label>
          <div className="w-full flex gap-2">
            <div className="w-1/4  font-normal">
              <Select
                className="bg-[#f1f5f9]"
                options={options}
                onChange={(data: any) => {
                  setValue('sex', data.value);
                }}
                placeholder="Giới tính"
                value={watch('sex')}
              />
            </div>
            {watch('sex') === 'other' && (
              <InputComponent placeholder="Nhập giới tính" register={register('customSex')} />
            )}
          </div>
          <ErrorValidateComponent visible={!!errors.sex}>{errors.sex?.message}</ErrorValidateComponent>
        </div>
      </div>
      <div className="form-group ">
        <div className="flex">
          <label htmlFor="dob" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Ngày sinh
          </label>
          <div className="w-full">
            <InputComponent placeholder="Ngày sinh" id="dob" register={register('dob')} />
            <ErrorValidateComponent visible={!!errors.dob}>{errors.dob?.message}</ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group ">
        <div className="flex">
          <label htmlFor="description" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Mô tả
          </label>
          <div className="w-full">
            <InputComponent type="textarea" placeholder="Mô tả" id="description" register={register('description')} />
            <ErrorValidateComponent visible={!!errors.description}>
              {errors.description?.message}
            </ErrorValidateComponent>
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

export default EditProfileGeneral;
