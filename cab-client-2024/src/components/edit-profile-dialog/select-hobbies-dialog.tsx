import React, { useEffect, useState } from 'react';
import { IUserProfile } from '../../models';
import * as yup from 'yup';
import ButtonSecondary from '../button-refactor/button-secondary';
import ButtonPrimary from '../button-refactor/button-primary';
import { SubmitHandler, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import toast from 'react-hot-toast';
import { IOption } from '../../select/select.component';
import { fetchAllCategory } from '../../api';
import DialogComponent from '../dialog/dialog.component';
import { Tag } from 'antd';
import { userService } from '../../services/user.service';

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
  onUpdateSuccess?: () => void;
}

const schema = yup.object().shape({});

interface IFormValues {
  school?: string;
  company?: string;
  marry?: number;
  categoryFavorites?: any;
  sexualOrientation?: number;
}

const SelectHobbiesDialog = ({ profile, onUpdateSuccess }: EditProfiledDialogProps) => {
  const { school, company, categoryFavorites, marry, sexualOrientation } = profile;
  const [visible, setVisible] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);
  const [clickedTags, setClickedTags] = useState<string[]>([]);
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
    !profile.isUpdateProfile && setVisible(true);
  }, []);

  const onSubmit: SubmitHandler<IFormValues> = async (formValues) => {
    try {
      setLoading(true);

      const payload = {
        ...profile,
        categoryFavorites: clickedTags || null,
        email: profile.email,
      };
      await userService.updateProfile(payload as IUserProfile);

      onUpdateSuccess && onUpdateSuccess();
      setVisible(false);
    } catch (error) {
      toast.error('Không thể cập nhật tài khoản của bạn');
      setVisible(false);
    } finally {
      setLoading(false);
    }
  };

  const handleTagClick = (value: string) => {
    setClickedTags((prevClickedTags) =>
      prevClickedTags.includes(value) ? prevClickedTags.filter((tag) => tag !== value) : [...prevClickedTags, value],
    );
  };
  return (
    <DialogComponent
      size="xl"
      visible={visible}
      onClose={() => setVisible(false)}
      title="Vui lòng  chọn sở thích của bạn"
      placement="CENTER"
    >
      <form className="space-y-4 py-5 font-bold" onSubmit={handleSubmit(onSubmit)}>
        <div className="form-group ">
          {/* <div className="flex">
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
                isDisabled
              />
            </div>
          </div> */}
          <div className="w-full">
            <div className="flex flex-wrap gap-2">
              {categoryOptions.map((option) => (
                <Tag
                  key={option.value}
                  onClick={() => handleTagClick(option.value)}
                  className={
                    clickedTags.includes(option.value)
                      ? 'inline-flex items-center gap-2 py-0.5 px-2.5 border shadow rounded-full  bg-blue-500 text-white border-blue-500 cursor-pointer'
                      : 'inline-flex items-center gap-2 py-0.5 px-2.5 border shadow rounded-full border-gray-100 cursor-pointer'
                  }
                >
                  {option.label}
                </Tag>
              ))}
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
    </DialogComponent>
  );
};

export default SelectHobbiesDialog;
