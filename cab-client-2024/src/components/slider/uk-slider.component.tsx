import classNames from 'classnames';
import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import GoldMedal from '../../assets/icons/gold-medal.png';

const UkSlider = ({ sliderItems }: any) => {
  useEffect(() => {
    // Reinitialize UIkit slider
  }, []);
  const currencyFormatter = new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
  });
  const getSpanClassNames = (index: number) => {
    return classNames(
      'relative grid select-none items-center whitespace-nowrap rounded-lg py-1.5 px-3 font-sans text-xs font-bold uppercase',
      {
        'bg-yellow-100 text-yellow-500': index === 1,
        'bg-green-100 text-green-500': index === 2,
        'bg-blue-100 text-blue-500': index === 3,
        'bg-red-100 text-red-500': index === 4,
        'bg-purple-100 text-purple-500': index === 5,
        'bg-gray-900 text-white': index > 5 || index < 1,
      },
    );
  };
  return (
    <div className=" font-normal text-sm ">
      <div className="overflow-hidden  flex flex-col gap-2">
        {sliderItems?.map((item, index) => (
          <div className="flex justify-center " key={item.id}>
            {index === 0 ? (
              <div className="w-1/2 pr-2">
                <div className="flex flex-col items-center shadow-sm p-2 rounded-xl border1">
                  <Link to={`user/${item?.userId}`}>
                    <div className="relative w-16 h-16 mx-auto mt-2">
                      <img
                        src={item?.avatar || 'https://www.w3schools.com/howto/img_avatar.png'}
                        alt=""
                        className="h-full object-cover rounded-full shadow w-full"
                      />
                      {/* <div className="absolute -top-5 left-1/2 transform -translate-x-1/2 ">
                        <img src={Crown} alt="" />
                      </div> */}
                      <div className="absolute -bottom-5 left-1/2 transform -translate-x-1/2 ">
                        <img src={GoldMedal} alt="" />
                      </div>
                    </div>
                  </Link>
                  <div className="mt-5 text-center w-full">
                    <Link to={`user/${item?.userId}`}>
                      <div className="text-[12px]  font-extrabold bg-clip-text text-transparent bg-primary bg-[length:200%_auto] animate-gradient">
                        {item?.userName}
                      </div>
                    </Link>
                    <div
                      className="text-[10px] font-extrabold bg-clip-text text-transparent bg-black
                        bg-[length:200%_auto] animate-gradient"
                    >
                      {item?.totalAmount
                        ? `${item?.totalAmount}`.replace(/\B(?=(\d{3})+(?!\d))/g, '.')
                        : item?.totalFollow || item?.totalAction}
                      {item?.totalFollow ? ' Followers' : item?.totalAmount ? ' đ' : ' Tương tác'}
                    </div>
                  </div>
                </div>
              </div>
            ) : (
              <div className="flex md:items-center space-x-4 p-2 rounded-md box w-60">
                <div className="sm:w-14 w-6 sm:h-14 h-6 flex-shrink-0 rounded-lg relative">
                  <img
                    src={item?.avatar || 'https://www.w3schools.com/howto/img_avatar.png'}
                    alt=""
                    className="h-full object-cover rounded-full shadow w-full"
                  />
                </div>
                <div className="flex-1">
                  <Link
                    to={`user/${item?.userId}`}
                    className="md:text-sm text-base font-semibold capitalize text-black dark:text-white"
                  >
                    {item?.userName}
                  </Link>

                  <div className="flex items-center mt-2">
                    <div className="text-xs text-gray-500 font-semibold">
                      {item?.totalAmount
                        ? `${item?.totalAmount}`.replace(/\B(?=(\d{3})+(?!\d))/g, '.')
                        : item?.totalFollow || item?.totalAction}
                      {item?.totalFollow ? ' Followers' : item?.totalAmount ? ' đ ' : ' Tương tác'}
                    </div>
                  </div>
                </div>
                <span className={getSpanClassNames(index + 1)}>{index + 1}</span>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default UkSlider;
