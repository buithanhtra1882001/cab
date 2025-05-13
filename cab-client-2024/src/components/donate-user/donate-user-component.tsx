import React, { useState } from 'react';
import DialogComponent from '../dialog/dialog.component';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import { donateUser } from '../../api';
import toast from 'react-hot-toast';
import { InputNumber } from 'antd';

const DonateUser = ({ user }: any) => {
  console.log('🚀 ~ DonateUser ~ user:', user);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const profileUser = useSelector((state: RootState) => state.features.auth.profile);
  const [donationAmount, setDonationAmount] = useState<any>();
  const [content, setContent] = useState('');

  const handleContentChange = (e) => {
    setContent(e.target.value);
  };
  // Function to toggle dialog visibility
  const toggleDialog = () => {
    setIsDialogOpen(!isDialogOpen);
  };
  // State for the money input
  // Function to handle changes to the input field

  const handleSubmit = () => {
    // Validate the donationAmount here (e.g., check if it's not empty and is a valid number)
    if (!donationAmount) {
      toast.error('Vui lòng nhập số tiền bạn muốn donate.');
      return;
    }

    // Assuming you have a function to handle the actual submission
    // For example, sendDonation(donationAmount);
    donateUser({
      toUserId: user.userId,
      message: content,
      amount: donationAmount,
    })
      .then((res) => {
        const total = `${donationAmount}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        toast.success(`Bạn đã donate ${total}  VNĐ thành công cho ${user?.userFullName}`, {
          duration: 4000,
          position: 'top-center',
        });
      })
      .catch((err) => {
        toast.error(err.response.data.message);
      });
    // Optionally, clear the input field after submission
    setDonationAmount(null);
    toggleDialog();
  };

  function formatVND(value) {
    const amount = Number(value);
    if (isNaN(amount)) return '0 VND'; // Return "0 VND" if the value is not a number

    // Format the number with commas and add " VND" suffix
    return `${amount.toFixed(0).replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')} VND`;
  }

  return (
    <>
      <button
        className="button bg-primary flex items-center gap-2 text-white py-2 px-3.5 max-md:flex-1"
        onClick={toggleDialog}
      >
        <span className="text-sm"> DONATE </span>
      </button>
      <DialogComponent
        onClose={() => setIsDialogOpen(false)}
        visible={isDialogOpen}
        placement="CENTER"
        title="Donate"
        size="md"
      >
        <div className="w-full py-3">
          <p className="text-base font-medium leading-6 tracking-normal text-left mb-1.5">Bạn đang donate tới </p>
          <div className="box p-4">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <img
                  className="h-10 w-10 rounded-full"
                  src={user?.avatar || 'https://www.w3schools.com/howto/img_avatar.png'}
                  alt=""
                />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-gray-900">{user?.fullname}</div>
              </div>
            </div>
          </div>
          <label htmlFor="donationAmount" className="block text-sm font-medium text-gray-700 text-end">
            Số tiền có trong tài khoản của bạn:
            {formatVND(profileUser?.coin)}
          </label>
          <div className="mt-4">
            <label htmlFor="donationAmount" className="block text-sm font-medium text-gray-700">
              Số tiền (VND)
            </label>
            <InputNumber
              name="donationAmount"
              id="donationAmount"
              value={donationAmount}
              formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
              onChange={setDonationAmount}
              className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              placeholder="Nhập số tiền"
            />
          </div>
          <div className="mt-4">
            <label htmlFor="content" className="block text-sm font-medium text-gray-700">
              Lời nhắn
            </label>
            <textarea
              name="content"
              id="content"
              value={content}
              onChange={handleContentChange}
              className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              placeholder="Nhập nội dung"
            />
          </div>
          <div className="mt-4 flex justify-end">
            <button
              onClick={handleSubmit}
              className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              Gửi
            </button>
          </div>
        </div>
      </DialogComponent>
    </>
  );
};

export default DonateUser;
