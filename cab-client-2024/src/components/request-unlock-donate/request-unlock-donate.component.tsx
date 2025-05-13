import React, { useState } from 'react';
import DialogComponent from '../dialog/dialog.component';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import { requestFeartureDonate } from '../../api';
import toast from 'react-hot-toast';
import useVietQR from '../../hooks/use-bank/use-bank';

const RequestUnlockDonate = ({ post }: any) => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const profileUser = useSelector((state: RootState) => state.features.auth.profile);
  const [identificationID, setIdentificationID] = useState<any>();
  const [content, setContent] = useState('');
  const [selectedBank, setSelectedBank] = useState('');
  const [accountName, setAccountName] = useState<any>('');
  const handleBankChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedBank(e.target.value);
  };
  const { banks, loading, error } = useVietQR({
    clientID: '69163d51-f531-4c8a-9f0b-04bebd4eac4d',
    apiKey: 'dcd8ec30-bf67-4146-96a2-dfe51fe42139',
  });

  const handleContentChange = (e) => {};
  // Function to toggle dialog visibility
  const toggleDialog = () => {
    setIsDialogOpen(!isDialogOpen);
  };
  // State for the money input
  // Function to handle changes to the input field

  const handleSubmit = () => {
    // Validate the identificationID here (e.g., check if it's not empty and is a valid number)
    if (!identificationID) {
      toast.error('Vui lòng nhập số tiền bạn muốn donate.');
      return;
    }

    // Assuming you have a function to handle the actual submission
    // For example, sendDonation(identificationID);
    requestFeartureDonate({
      nationalId: identificationID,
      referenceLinks: content,
      bankAccount: accountName,
      bankName: selectedBank,
    })
      .then((res) => {
        toast.success(`Bạn đã gửi yêu cầu thành công ! Chúng tôi sẽ liên hệ bạn sau .`, {
          duration: 4000,
          position: 'top-center',
        });
      })
      .catch((err) => {
        console.log('🚀 ~ handleSubmit ~ err:', err);
        toast.error(err.response.data.message);
      });
    // Optionally, clear the input field after submission
    setIdentificationID(null);
    toggleDialog();
  };

  return (
    <>
      <button type="button" className="bg-blue-500 text-white p-2 rounded-md mt-2" onClick={toggleDialog}>
        Yêu cầu mở khóa
      </button>
      <DialogComponent
        onClose={() => setIsDialogOpen(false)}
        visible={isDialogOpen}
        placement="CENTER"
        title="Yêu cần mở chức năng nhận donate"
        size="md"
      >
        <div className="w-full py-3">
          <div className="mt-4">
            <label htmlFor="identificationID" className="block text-sm font-medium text-gray-700">
              Căn cước công dân
            </label>
            <input
              name="identificationID"
              id="identificationID"
              value={identificationID}
              onChange={(e) => setIdentificationID(e.target.value)}
              className="mt-1 block w-full pl-3 pr-10 py-2 text-base border border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              placeholder="Nhập căn cước"
            />
          </div>
          <div className="mt-4">
            <label htmlFor="content" className="block text-sm font-medium text-gray-700">
              Reference Links
            </label>
            <textarea
              name="content"
              id="content"
              value={content}
              onChange={(e) => setContent(e.target.value)}
              className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              placeholder="Nhập nội dung ( Link Facebook, Link Youtube, Link Instagram, Link Tiktok, Link Twitter, Link Website, Link khác)"
            />
          </div>
          <div className="mt-4">
            <label htmlFor="bank" className="block text-sm font-medium text-gray-700">
              Ngân hàng
            </label>
            <select
              name="bank"
              id="bank"
              value={selectedBank}
              onChange={handleBankChange}
              className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
            >
              <option value="" disabled>
                Chọn ngân hàng
              </option>
              {banks.map((bank) => (
                <option key={bank?.id} value={bank?.code}>
                  {bank?.shortName}
                </option>
              ))}
            </select>
          </div>
          <div className="mt-4">
            <label htmlFor="identificationID" className="block text-sm font-medium text-gray-700">
              Tên tài khoản ngân hàng
            </label>
            <input
              name="accountName"
              id="accountName"
              value={accountName}
              onChange={(e) => setAccountName(e.target.value)}
              className="mt-1 block w-full pl-3 pr-10 py-2 text-base border border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              placeholder="Nhập tên tài khoản"
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

export default RequestUnlockDonate;
