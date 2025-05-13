import React from 'react';
import { useForm } from 'react-hook-form';
import { CreateGroupRequest } from '../../../models/group.model';
import { useGroupHook } from '../../../hooks/group/useGroup';

interface ModalCreateGroupProps {
  isOpen: boolean;
  onClose: () => void;
}

const ModalCreateGroup: React.FC<ModalCreateGroupProps> = ({ isOpen, onClose }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateGroupRequest>();
  const { appendNewGroup } = useGroupHook({});

  const [currentStep, setCurrentStep] = React.useState(1);
  const totalSteps = 3;

  const nextStep = () => setCurrentStep((prev) => Math.min(prev + 1, totalSteps));
  const prevStep = () => setCurrentStep((prev) => Math.max(prev - 1, 1));
  const onSubmit = async (data: CreateGroupRequest) => {
    if (currentStep < totalSteps) {
      nextStep();
    } else {
      await appendNewGroup(data);
      onClose();
    }
  };

  const renderStepIndicator = () => {
    return (
      <div className="flex items-center justify-between mb-8">
        {[...Array(totalSteps)].map((_, index) => (
          <React.Fragment key={index}>
            <div className="flex flex-col items-center">
              <div
                className={`w-10 h-10 rounded-full flex items-center justify-center ${
                  index + 1 <= currentStep ? 'bg-blue-500 text-white' : 'bg-gray-200 text-gray-600'
                }`}
              >
                {index + 1}
              </div>
              <span className="mt-2 text-xs">{`Bước ${index + 1}`}</span>
            </div>
            {index < totalSteps - 1 && (
              <div className={`flex-1 h-1 mx-2 ${index + 1 < currentStep ? 'bg-blue-500' : 'bg-gray-200'}`} />
            )}
          </React.Fragment>
        ))}
      </div>
    );
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-80 flex justify-center items-center z-[9999]">
      <div className="bg-white dark:bg-gray-800 rounded-lg p-8 pt-0 max-w-md w-full">
        <div className="flex justify-between py-4 items-center">
          <h2 className="text-2xl font-bold  text-gray-800 dark:text-white">Tạo nhóm mới</h2>
          <button
            onClick={onClose}
            className=" text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
            aria-label="Close"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 overflow-y-auto max-h-[calc(100vh-200px)]">
          {renderStepIndicator()}
          {/* Step 1: Basic Information */}
          {currentStep === 1 && (
            <div className="mb-6">
              <h3 className="text-lg font-semibold mb-3">Thông tin cơ bản</h3>
              <div>
                <label htmlFor="groupName" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Tên nhóm
                </label>
                <input
                  {...register('groupName', { required: 'Tên nhóm là bắt buộc' })}
                  type="text"
                  id="groupName"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                />
                {errors.groupName && <p className="mt-1 text-sm text-red-600">{errors.groupName.message}</p>}
              </div>
              <div className="mt-4">
                <label htmlFor="groupType" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Loại nhóm
                </label>
                <select
                  {...register('groupType', { required: 'Loại nhóm là bắt buộc' })}
                  id="groupType"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                >
                  <option value={0}>Công khai</option>
                  <option value={1}>Riêng tư</option>
                  <option value={2}>Bí mật</option>
                </select>
                {errors.groupType && <p className="mt-1 text-sm text-red-600">{errors.groupType.message}</p>}
              </div>

              <div className="mt-4">
                <label
                  htmlFor="groupDescription"
                  className="block text-sm font-medium text-gray-700 dark:text-gray-300"
                >
                  Mô tả nhóm
                </label>
                <textarea
                  {...register('groupDescription', { required: 'Mô tả nhóm là bắt buộc' })}
                  id="groupDescription"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                />
                {errors.groupDescription && (
                  <p className="mt-1 text-sm text-red-600">{errors.groupDescription.message}</p>
                )}
              </div>
            </div>
          )}

          {/* Step 2: Additional Details */}
          {currentStep === 2 && (
            <div className="mb-6">
              <h3 className="text-lg font-semibold mb-3">Thông tin bổ sung</h3>
              <div>
                <label htmlFor="groupTagline" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Khẩu hiệu nhóm
                </label>
                <input
                  {...register('groupTagline')}
                  type="text"
                  id="groupTagline"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                />
              </div>

              <div className="mt-4">
                <label htmlFor="rules" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Quy tắc nhóm
                </label>
                <textarea
                  {...register('rules')}
                  id="rules"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                />
              </div>

              <div className="mt-4">
                <label htmlFor="location" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Vị trí
                </label>
                <input
                  {...register('location')}
                  type="text"
                  id="location"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                />
              </div>
            </div>
          )}

          {/* Step 3: Settings and Media */}
          {currentStep === 3 && (
            <div className="mb-6">
              <h3 className="text-lg font-semibold mb-3">Cài đặt và Hình ảnh</h3>
              <div>
                <label htmlFor="privacySettings" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Cài đặt quyền riêng tư
                </label>
                <select
                  {...register('privacySettings')}
                  id="privacySettings"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                >
                  <option value="public">Công khai</option>
                  <option value="private">Riêng tư</option>
                </select>
              </div>

              <div className="mt-4">
                <input
                  {...register('approvalRequired')}
                  type="checkbox"
                  id="approvalRequired"
                  className="rounded border-gray-300 text-indigo-600 shadow-sm focus:border-indigo-300 focus:ring focus:ring-offset-0 focus:ring-indigo-200 focus:ring-opacity-50"
                />
                <label htmlFor="approvalRequired" className="ml-2 text-sm text-gray-700 dark:text-gray-300">
                  Yêu cầu phê duyệt khi tham gia nhóm
                </label>
              </div>

              <div className="mt-4">
                <label
                  htmlFor="groupCoverImageUrl"
                  className="block text-sm font-medium text-gray-700 dark:text-gray-300"
                >
                  URL ảnh bìa nhóm
                </label>
                <input
                  {...register('groupCoverImageUrl')}
                  type="url"
                  id="groupCoverImageUrl"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                />
              </div>

              <div className="mt-4">
                <label htmlFor="groupAvatarUrl" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  URL avatar nhóm
                </label>
                <input
                  {...register('groupAvatarUrl')}
                  type="url"
                  id="groupAvatarUrl"
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-300 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                />
              </div>
            </div>
          )}

          <div className="flex justify-end space-x-3">
            {currentStep > 1 ? (
              <button
                type="button"
                onClick={prevStep}
                className="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Quay lại
              </button>
            ) : (
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Hủy
              </button>
            )}

            <button
              type="submit"
              className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              {currentStep === totalSteps ? 'Tạo nhóm' : 'Tiếp tục'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ModalCreateGroup;
