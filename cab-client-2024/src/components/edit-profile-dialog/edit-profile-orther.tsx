import React, { useEffect, useState } from 'react';
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
import Select, { IOption } from '../../select/select.component';
import { fetchAllCategory } from '../../api';
import ReactSelect, { MultiValue } from 'react-select';

const options: IOption[] = [
  {
    label: 'Nam',
    value: 1,
  },
  {
    label: 'Nữ',
    value: 2,
  },
  {
    label: 'LGBT',
    value: 3,
  },
  {
    label: 'LGBTQ+',
    value: 4,
  },
];

interface EditProfiledDialogProps {
  profile: IUserProfile;
  onUpdateSuccess: () => void;
}

const schema = yup.object().shape({});

interface IFormValues {
  school?: string;
  company?: string;
  marry?: number;
  categoryFavorites?: any;
  sexualOrientation?: number;
}

const EditProfileOrther = ({ profile, onUpdateSuccess }: EditProfiledDialogProps) => {
  const { school, company, categoryFavorites, marry, sexualOrientation } = profile;
  const [visible, setVisible] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);
  const [categoryOptions, setCategoryOptions] = useState<IOption[]>([]);
  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<IFormValues>({
    resolver: yupResolver(schema),
    defaultValues: {
      school,
      company,
      categoryFavorites,
      marry,
      sexualOrientation,
    },
  });
  const fetchCategoryOptions = async () => {
    try {
      // Replace this URL with the actual endpoint from which you're fetching category options
      const response = await fetchAllCategory();
      const data: any[] = response.map((item: any) => ({ value: item.id, label: item.name }));
      setCategoryOptions(data);
      // Optionally, update form default values here if necessary
    } catch (error) {
      console.error('Failed to fetch category options:', error);
    }
  };

  // Fetch category options on component mount
  useEffect(() => {
    fetchCategoryOptions();
  }, []);

  const onSubmit: SubmitHandler<IFormValues> = async (formValues) => {
    try {
      setLoading(true);

      const payload = {
        ...profile,
        ...formValues,
        categoryFavorites: formValues.categoryFavorites ? formValues.categoryFavorites : null,
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
          <label htmlFor="fullname" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Trường học
          </label>
          <div className="w-full">
            <InputComponent placeholder="Tên trường" id="fullname" register={register('school')} />
            <ErrorValidateComponent visible={!!errors.school}>{errors.school?.message}</ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group ">
        <div className="flex">
          <label htmlFor="fullname" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Nơi làm việc
          </label>
          <div className="w-full">
            <InputComponent placeholder="Nơi làm việc" id="fullname" register={register('company')} />
            <ErrorValidateComponent visible={!!errors.school}>{errors.school?.message}</ErrorValidateComponent>
          </div>
        </div>
      </div>
      <div className="form-group ">
        <div className="flex">
          <label
            htmlFor="categoryFavorites"
            className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 "
          >
            Sở thích
          </label>
          <div className="w-full flex gap-2">
            <ReactSelect
              isMulti
              options={categoryOptions}
              onChange={(selectedOptions: MultiValue<any>) => {
                setValue(
                  'categoryFavorites',
                  selectedOptions.map((option) => option.value),
                );
              }}
              placeholder="Sở thích"
              value={categoryOptions.filter((option) => watch('categoryFavorites').includes(option.value))}
              styles={{
                container: (provided) => ({
                  ...provided,
                  width: '100%',
                }),
                control: (provided) => ({
                  ...provided,
                  width: '100%',
                }),
              }}
            />
          </div>
        </div>
      </div>
      <div className="form-group ">
        <div className="flex">
          <label htmlFor="gender" className="flex items-center w-[20%] text-sm text-slate-800 dark:text-slate-50 ">
            Bạn thích
          </label>
          <div className="w-full flex gap-2">
            <Select
              options={options}
              onChange={(data: any) => {
                setValue('sexualOrientation', data.value);
              }}
              placeholder="Giới tính"
              value={watch('sexualOrientation')}
            />
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

export default EditProfileOrther;
