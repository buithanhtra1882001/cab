import React from 'react';
import { BsThreeDots } from 'react-icons/bs';
import { GroupElement } from '../../../models';
import { GroupPermissions } from '../../../constants/group.constant';

const GroupBanner = ({ data, permission }: { data?: GroupElement; permission: any }) => {
  const [dropdownOpen, setDropdownOpen] = React.useState(false); // State để mở dropdown
  return (
    <div className="bg-white shadow  lg:-mt-10 dark:bg-dark2">
      <div className="relative overflow-hidden w-full lg:h-72 h-36">
        <img src={data?.groupCoverImageUrl} alt="" className="h-full w-full object-cover inset-0" />

        <div className="w-full bottom-0 absolute left-0 bg-gradient-to-t from -black/60 pt-10 z-10" />

        {permission === GroupPermissions.GROUP_ADMIN && (
          <div className="absolute bottom-0 right-0 m-4 z-20">
            <div className="flex items-center gap-3">
              <button className="button bg-white/20 text-white flex items-center gap-2 backdrop-blur-small">
                Đổi ảnh bìa
              </button>
              <button className="button bg-black/10 text-white flex items-center gap-2 backdrop-blur-small">
                Chỉnh sửa thông tin nhóm
              </button>
            </div>
          </div>
        )}
      </div>

      <div className="lg:px-10 md:p-5 p-3">
        <div className="flex flex-col justify-center">
          <div className="flex lg:items-center justify-between max-md:flex-col">
            <div className="flex-1">
              <h3 className="md:text-2xl text-base font-bold text-black dark:text-white"> {data?.groupName} </h3>
              <p className="font-normal text-gray-500 mt-2 flex gap-2 flex-wrap dark:text-white/80">
                <span className="max-lg:hidden uppercase"> {data?.privacySettings} group </span>
              </p>
            </div>

            <div>
              {permission !== GroupPermissions.GROUP_ADMIN && (
                <div className="flex items-center gap-2 mt-1">
                  <button className="button bg-primary flex items-center gap-1 text-white py-2 px-3.5 shadow ml-auto">
                    <span className="text-sm">Join</span>
                  </button>
                  <div className="relative">
                    <button
                      type="button"
                      className="rounded-lg bg-slate-100 flex px-2.5 py-2 dark:bg-dark2"
                      aria-haspopup="true"
                      aria-expanded="false"
                      onClick={() => setDropdownOpen(!dropdownOpen)} // Sử dụng state để mở dropdown
                    >
                      <BsThreeDots />
                    </button>
                    {dropdownOpen && (
                      <div className="absolute right-0 w-[240px] bg-white dark:bg-dark2 rounded-lg shadow-md mt-2">
                        <nav>
                          <a href="#" className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-700">
                            Unfollow
                          </a>
                          <a href="#" className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-700">
                            Share
                          </a>
                          <a href="#" className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-700">
                            Copy link
                          </a>
                          <a href="#" className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-700">
                            Sort comments
                          </a>
                          <a href="#" className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-700">
                            Report group
                          </a>
                          <hr />
                          <a href="#" className="block px-4 py-2 text-red-400 hover:bg-red-50 dark:hover:bg-red-500/50">
                            Block
                          </a>
                        </nav>
                      </div>
                    )}
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

GroupBanner.propTypes = {};

export default GroupBanner;
