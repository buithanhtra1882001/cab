import React from 'react';
import { creatorRequest } from '../../api/creator-request';

export const CreatorRequest = () => {
  const handleAcceptRequest = () => {
    handleCreatorRequest();
  };

  const handleCreatorRequest = async () => {
    const res = await creatorRequest();
    console.log('res accept 122', res);
    if (res) {
      alert('Successfully!');
    }
  };
  return (
    <div className="flex justify-center items-center h-screen">
      <div className="">
        <button
          onClick={handleAcceptRequest}
          className="border-[2px] border-[#1D8DE3] text-[#1D8DE3] p-[8px] min-w-[100px] rounded-[8px] hover:text-[#fff] hover:bg-[rgba(29,141,227,0.4)] font-medium"
        >
          Accept
        </button>
      </div>
    </div>
  );
};
