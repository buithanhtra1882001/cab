import React from 'react';

interface SliderItemProps {
  href: string;
  imgSrc: string;
  name: string;
  followers: string;
}

const SliderItem: React.FC<SliderItemProps> = ({ href, imgSrc, name, followers }) => {
  return (
    <div className="flex flex-col items-center shadow-sm p-2 rounded-xl border1">
      <a href="timeline.html">
        <div className="relative w-16 h-16 mx-auto mt-2">
          <img
            src="assets/images/avatars/avatar-4.jpg"
            alt=""
            className="h-full object-cover rounded-full shadow w-full"
          />
        </div>
      </a>
      <div className="mt-5 text-center w-full">
        <a href="timeline.html">
          <h5 className="font-semibold">James Lewis</h5>
        </a>
        <div className="text-xs text-gray-400 mt-0.5 font-medium">15K Followers</div>
        <button
          type="button"
          className="bg-secondery block font-semibold mt-4 py-1.5 rounded-lg text-sm w-full border1"
        >
          Follow
        </button>
      </div>
    </div>
  );
};

export default SliderItem;
