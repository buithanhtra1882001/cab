import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { IHashTag } from '../../types/hashtag';
import { fetchLeaderboardApi } from '../../api/user-profile/leaderboard';
import { LeaderboardData } from '../../models/leaderboard.model';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import _get from 'lodash/get';
import { ChevronDown, CircleCheck } from 'lucide-react';
import { fetchStatisticalByUserIdApi } from '../../api';
import { UserProfileStats } from '../../models';
import UkSlider from '../../components/slider/uk-slider.component';

interface AsideRightProps {
  hashtags: IHashTag[];
}
const tabLabels = {
  followers: 'Top Follows',
  receives: 'Top Receives',
  donator: 'Top Donator',
  // Add other tab keys and their labels here
};
export const AsideRight = ({ hashtags }: AsideRightProps) => {
  const [leaderboard, setLeaderboard] = useState<LeaderboardData>();
  const [statisticalUser, setStatisticalUser] = useState<UserProfileStats>();
  const [activeTab, setActiveTab] = useState('followers');
  const profileUser = useSelector((state: RootState) => state.features.auth.profile);
  const handleTabClick = (tabName) => {
    setActiveTab(tabName);
    toggleDropdown();
  };
  const [isDropdownVisible, setIsDropdownVisible] = useState(false);

  const toggleDropdown = () => setIsDropdownVisible(!isDropdownVisible);

  useEffect(() => {
    const fetchLeaderboard = async () => {
      try {
        const response = await fetchLeaderboardApi();
        setLeaderboard(response);
      } catch (error) {
        console.error('Failed to fetch leaderboard:', error);
      }
    };

    fetchLeaderboard();
  }, []);

  useEffect(() => {
    const fetchStatisticalByUserId = async (id) => {
      try {
        const response = await fetchStatisticalByUserIdApi(id);
        setStatisticalUser(response);
      } catch (error) {
        console.error('Failed to fetch leaderboard:', error);
      }
    };

    fetchStatisticalByUserId(profileUser?.userId);
  }, [profileUser]);

  return (
    <aside
      className="hidden lg:block flex-1 sticky top-20 overflow-auto no-scrollbar"
      style={{
        height: 'calc(100vh - 84px)',
      }}
    >
      <div className="space-y-4">
        <section>
          <div className="box  border1 dark:bg-dark2">
            <div className="rounded-t-lg h-32 overflow-hidden">
              <img
                className="object-cover object-top w-full h-32"
                src={_get(profileUser, 'coverImage', '') || 'https://www.w3schools.com/howto/img_avatar.png'}
                alt="Mountain"
              />
            </div>
            <div className="mx-auto w-32 h-32 relative -mt-16 border-4 border-white rounded-full overflow-hidden">
              <img
                className="object-cover object-center h-32"
                src={_get(profileUser, 'avatar', '') || 'https://www.w3schools.com/howto/img_avatar.png'}
                alt="Woman looking front"
              />
            </div>
            <div className="text-center mt-2">
              <h2 className="font-semibold flex justify-center items-center gap-2">
                {profileUser?.fullname}

                {_get(profileUser, 'userType') === 'CONTENT_CREATOR' ? (
                  <CircleCheck color="green" size={16} />
                ) : (
                  profileUser?.isVerifyEmail && <CircleCheck color="black" size={16} />
                )}
              </h2>
              <p className="text-blue-500">@{profileUser?.username}</p>
            </div>
            <ul className="flex items-center justify-center mt-1">
              <li className="m-2">
                <h4 className="font-bold text-sm text-center">
                  {statisticalUser?.post || 0}
                  <span className="text-[10px] font-medium mt-1 text-gray-500 block">Bài viết</span>
                </h4>
              </li>
              <li className="m-2">
                <h4 className="font-bold text-sm text-center">
                  {statisticalUser?.like || 0}
                  <span className="text-[10px] font-medium mt-1 text-gray-500 block">Like</span>
                </h4>
              </li>
              <li className="m-2">
                <h4 className="font-bold text-sm text-center">
                  {statisticalUser?.comment || 0}
                  <span className="text-[10px] font-medium mt-1 text-gray-500 block">Bình luận</span>
                </h4>
              </li>
              <li className="m-2">
                <h4 className="font-bold text-sm text-center">
                  {statisticalUser?.newFriend || 0}
                  <span className="text-[10px] font-medium mt-1 text-gray-500 block">Bạn bè</span>
                </h4>
              </li>
            </ul>
          </div>
        </section>
        <section>
          <div className="box p-5 px-6 border1 dark:bg-dark2">
            <div className="px-4 pb-4  rounded-b-lg   ">
              <div className="flex justify-center text-black dark:text-white">
                <div className="relative inline-flex">
                  <button
                    className="inline-flex items-center justify-center h-10 gap-2 px-5 text-sm font-medium tracking-wide text-primary transition duration-300 rounded focus-visible:outline-none whitespace-nowrap"
                    onClick={toggleDropdown}
                  >
                    <span>{activeTab ? tabLabels[activeTab] : 'Select an option'}</span>
                    <span
                      className="relative only:-mx-5"
                      style={{
                        transition: 'transform 0.3s ease',
                        transform: isDropdownVisible ? 'rotate(180deg)' : 'rotate(0deg)',
                      }}
                    >
                      <ChevronDown />
                    </span>
                  </button>
                  {isDropdownVisible && (
                    <ul className="absolute z-20 flex flex-col py-2 mt-1 list-none bg-white rounded shadow-md w-48 top-full shadow-slate-500/10 justify-center">
                      <li>
                        <button
                          onClick={() => handleTabClick('receives')}
                          className="flex items-start justify-start gap-2 p-2 px-5 transition-colors duration-300 text-slate-500 hover:bg-blue-50 hover:text-blue-500 focus:bg-blue-50 focus:text-blue-600 focus:outline-none focus-visible:outline-none w-full"
                        >
                          <span className="flex flex-col gap-1 overflow-hidden whitespace-nowrap">
                            <span className="leading-5 truncate">Top Receives</span>
                          </span>
                        </button>
                      </li>
                      <li>
                        <button
                          onClick={() => handleTabClick('followers')}
                          className="flex items-start justify-start gap-2 p-2 px-5 transition-colors duration-300 text-slate-500 hover:bg-blue-50 hover:text-blue-600 focus:bg-blue-50 focus:text-blue-600 focus:outline-none focus-visible:outline-none w-full"
                        >
                          <span className="flex flex-col gap-1 overflow-hidden whitespace-nowrap">
                            <span className="leading-5 truncate">Top Followers</span>
                          </span>
                        </button>
                      </li>
                      <li>
                        <button
                          onClick={() => handleTabClick('donator')}
                          className=" flex items-start justify-start gap-2 p-2 px-5 overflow-hidden transition-colors duration-300 text-slate-500 hover:bg-blue-50 hover:text-blue-500 focus:bg-blue-50 focus:text-blue-600 focus:outline-none focus-visible:outline-none w-full"
                        >
                          <span className="flex flex-col gap-1 overflow-hidden whitespace-nowrap">
                            <span className="leading-5 truncate">Top Donator</span>
                          </span>
                        </button>
                      </li>
                    </ul>
                  )}
                </div>
              </div>
              {activeTab === 'followers' && <UkSlider sliderItems={leaderboard?.userFollows} />}
              {activeTab === 'donator' && <UkSlider sliderItems={leaderboard?.userDonates} />}
              {activeTab === 'receives' && <UkSlider sliderItems={leaderboard?.userRecieveDonates} />}
            </div>
          </div>
        </section>

        <section>
          <div className="box p-5 px-6 border1 dark:bg-dark2">
            <div className="flex justify-between text-black dark:text-white">
              <h3 className="font-bold text-base">Hashtag phổ biến</h3>
            </div>
            <div className="space-y-3.5 capitalize text-xs font-normal mt-5  text-gray-600 dark:text-white/80">
              {hashtags.map((hashtag, index) => {
                return (
                  <Link
                    className="block"
                    key={index}
                    // to={hashtag.slug}
                    to="#"
                  >
                    <div className="flex items-center gap-3">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                        strokeWidth="1.5"
                        stroke="currentColor"
                        className="w-5 h-5 -mt-2"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          d="M5.25 8.25h15m-16.5 7.5h15m-1.8-13.5l-3.9 19.5m-2.1-19.5l-3.9 19.5"
                        />
                      </svg>
                      <div className="flex-1">
                        <h4 className="font-semibold text-black dark:text-white text-sm">{hashtag.name} </h4>
                        <div className="mt-0.5">{hashtag.point.toFixed(2)} point </div>
                      </div>
                    </div>
                  </Link>
                );
              })}
            </div>
          </div>
        </section>
      </div>
    </aside>
  );
};
