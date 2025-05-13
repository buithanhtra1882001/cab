/* eslint-disable react/self-closing-comp */
import React, { useEffect, useState } from 'react';
import { Helmet } from 'react-helmet-async';
import ProfileBanner from '../../components/profile-banner/profile-banner.component';
import { usePostUser } from '../../hooks/post/usePostUser';
import _toString from 'lodash/toString';
import { useParams } from 'react-router-dom';
import { useUserProfile } from '../../hooks/profile/use-profile';
import { size } from 'lodash';
import InfiniteScroll from 'react-infinite-scroll-component';
import Loading from '../../components/loading/loading.component';
import { Post } from '../../components';
import UkSlider from '../../components/slider/uk-slider.component';
import { tabLabels } from '../profile/profile.page';
import { ChevronDown, HandCoins, Lock } from 'lucide-react';
import { fetchLeaderboardUserApi } from '../../api/user-profile/leaderboard';
import { totalReciveDonate } from '../../api';
import dayjs from 'dayjs';

export const ProfileUserPage = () => {
  const { userId } = useParams();
  const [isDropdownVisible, setIsDropdownVisible] = useState(false);
  const toggleDropdown = () => setIsDropdownVisible(!isDropdownVisible);
  const [activeTab, setActiveTab] = useState('timeline');
  const [leaderboard, setLeaderboard] = useState<any>();
  const [activeTabLeaderboard, setActiveTabLeaderboard] = useState('donator');
  const [totalRecivesDonate, setTotalRecivesDonate] = useState('0');
  const [historyDonate, setHistoryDonate] = useState<any>();
  const currencyFormatter = new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
  });
  const handleClick = (tabName) => {
    handleTabClick(tabName);
    toggleDropdown();
  };
  const handleTabClick = (tabName) => {
    setActiveTabLeaderboard(tabName);
  };
  const changeTag = (tab) => {
    setActiveTab(tab);
  };
  useEffect(() => {
    const fetchLeaderboard = async () => {
      try {
        const response = await fetchLeaderboardUserApi(userId);
        setLeaderboard(response);
      } catch (error) {
        console.error('Failed to fetch leaderboard:', error);
      }
    };
    const fetchTotalDonate = async () => {
      try {
        const response = await totalReciveDonate();
        setHistoryDonate(response);
        const totalCoins = response.reduce((acc, item) => acc + item.coin, 0);
        const formattedTotalCoins = new Intl.NumberFormat('vi-VN', {
          style: 'currency',
          currency: 'VND',
          minimumFractionDigits: 0,
        }).format(totalCoins);
        setTotalRecivesDonate(formattedTotalCoins);
      } catch (error) {
        console.error('Failed to fetch leaderboard:', error);
      }
    };
    fetchTotalDonate();
    fetchLeaderboard();
  }, []);

  const { profile, getUserProfile } = useUserProfile(_toString(userId));

  const [pageNumber, setPageNumber] = useState<number>(1);

  const { data: listPost, hasMore } = usePostUser({ pageNumber, userId: userId || '' });

  const onContactSuccess = () => {
    getUserProfile();
  };

  if (!profile) {
    return null;
  }

  const { username, description, categoryFavorites, city, isShowMarry, company, school } = profile;
  const renderTabDonate = () => {
    if (profile.canReceiveDonation) {
      return (
        <div className="flex flex-col 2xl:gap-12 gap-10 mt-8 ">
          <div className="relative flex flex-col min-w-0 break-words box dark:bg-slate-850 dark:shadow-dark-xl rounded-2xl bg-clip-border">
            <div className="flex-auto p-4">
              <div className="flex flex-row -mx-3">
                <div className="flex-none w-2/3 max-w-full px-3">
                  <div>
                    <p className="mb-0 font-sans font-semibold leading-normal uppercase text-sm">Tổng thu nhập</p>
                    <h5 className="mb-2 font-bold dark:text-white">{totalRecivesDonate}</h5>
                    {/* <p className="mb-0 dark:text-white dark:opacity-60">
                  <span className="font-bold leading-normal text-sm text-emerald-500">+55% </span>
                  kể từ hôm qua
                </p> */}
                  </div>
                </div>
                <div className="px-3 text-right basis-1/3">
                  <div className="text-white inline-flex justify-center items-center w-12 h-12 rounded-full bg-gradient-to-tl from-blue-500 to-violet-500">
                    <HandCoins />
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div className="relative flex flex-col min-w-0 break-words bg-white shadow-xl dark:bg-slate-850 dark:shadow-dark-xl rounded-2xl bg-clip-border py-4">
            <span className="uppercase p-4 text-lg font-semibold text-gray-700 dark:text-white">Lịch sử donate</span>

            {historyDonate && historyDonate.length > 0 && (
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="rounded-t-xl">
                  <tr>
                    <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Người donate
                    </th>
                    <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Content
                    </th>
                    <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Số tiền donate
                    </th>
                    <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Thời gian donate
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {historyDonate.map((item, index) => (
                    <tr key={index}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{item.donater}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{item.content}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {currencyFormatter.format(item.coin)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {dayjs(item.createDonate).format('DD/MM/YYYY HH:mm:ss')}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      );
    }
    return (
      <div className="flex flex-col justify-center items-center h-fix mt-10 box py-4">
        <Lock size={50} />

        <p className="text-sm text-gray-500 mt-2 dark:text-white/80 p-5">Bạn đã gửi yêu cầu chờ phản hồi !</p>
      </div>
    );
  };

  return (
    <div className="max-w-[1065px] mx-auto max-lg:-m-2.5">
      <Helmet>
        <title>{username} - Cab</title>
        <meta name="description" content={description} />
      </Helmet>
      <ProfileBanner
        profile={profile}
        onUpdateSuccess={() => {
          //
        }}
        tabActive={activeTab}
        changeTab={changeTag}
        onContactSuccess={onContactSuccess}
      />
      {activeTab === 'timeline' && (
        <div className="flex 2xl:gap-12 gap-10 mt-8 max-lg:flex-col">
          <div className="flex-1 xl:space-y-6 space-y-3">
            {size(listPost) ? (
              <InfiniteScroll
                dataLength={listPost.length}
                next={() => {
                  setPageNumber((prev) => prev + 1);
                }}
                hasMore={hasMore}
                loader={<Loading classname="mx-auto" />}
                endMessage={null}
                scrollableTarget="body"
              >
                <div className="flex-1 xl:space-y-6 space-y-3">
                  {listPost?.map((post, index) => {
                    return <Post key={post.id} post={post} />;
                  })}
                </div>
              </InfiniteScroll>
            ) : null}

            {!size(listPost) ? (
              <p className="text-sm text-slate-800 userDonates:text-slate-50 font-medium text-center mt-10">
                Bạn chưa có bài viết nào
              </p>
            ) : null}
          </div>
          <div className="lg:w-[400px]">
            <div className="lg:space-y-4 lg:pb-8 max-lg:grid sm:grid-cols-2 max-lg:gap-6 uk-sticky uk-active uk-sticky-fixed">
              <div className="box p-5 px-6">
                <div className="flex items-ce justify-between text-black dark:text-white">
                  <h3 className="font-bold text-lg"> Giới thiệu </h3>
                </div>

                <ul className="text-gray-700 space-y-4 mt-4 text-sm dark:text-white/80">
                  <li className="flex items-center gap-3">
                    <div>
                      Sống tại <span className="font-semibold text-black dark:text-white"> {city} </span>
                    </div>
                  </li>
                  <li className="flex items-center gap-3">
                    <div>
                      Học tại <span className="font-semibold text-black dark:text-white">{school}</span>
                    </div>
                  </li>
                  {company && (
                    <li className="flex items-center gap-3">
                      <div>
                        Làm việc tại <span className="font-semibold text-black dark:text-white"> {company} </span>
                      </div>
                    </li>
                  )}
                  <li className="flex items-center gap-3">
                    <div>
                      Mối quan hệ
                      <span className="font-semibold text-black dark:text-white">
                        {isShowMarry ? ' Kết Hôn' : ' Độc thân'}
                      </span>
                    </div>
                  </li>
                </ul>

                <div className="flex flex-wrap gap-1 text-sm mt-4 font-semibold capitalize">
                  <div className="inline-flex items-center gap-2 py-0.5 px-2.5 border shadow rounded-full border-gray-100">
                    Shoping
                  </div>
                  <div className="inline-flex items-center gap-2 py-0.5 px-2.5 border shadow rounded-full border-gray-100">
                    code
                  </div>
                  <div className="inline-flex items-center gap-2 py-0.5 px-2.5 border shadow rounded-full border-gray-100">
                    art
                  </div>
                  <div className="inline-flex items-center gap-2 py-0.5 px-2.5 border shadow rounded-full border-gray-100">
                    design
                  </div>
                </div>
              </div>
              <section>
                <div className="box p-5 px-6 border1 dark:bg-dark2">
                  <div className="px-4 pb-4  rounded-b-lg   ">
                    <div className="flex justify-center text-black dark:text-white">
                      <div className="relative inline-flex">
                        <button
                          className="inline-flex items-center justify-center h-10 gap-2 px-5 text-sm font-medium tracking-wide text-primary transition duration-300 rounded focus-visible:outline-none whitespace-nowrap"
                          onClick={toggleDropdown}
                        >
                          <span>{activeTabLeaderboard ? tabLabels[activeTabLeaderboard] : 'Select an option'}</span>
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
                                onClick={() => handleClick('followers')}
                                className="flex items-start justify-start gap-2 p-2 px-5 transition-colors duration-300 text-slate-500 hover:bg-blue-50 hover:text-blue-600 focus:bg-blue-50 focus:text-blue-600 focus:outline-none focus-visible:outline-none w-full"
                              >
                                <span className="flex flex-col gap-1 overflow-hidden whitespace-nowrap">
                                  <span className="leading-5 truncate">Top tương tác</span>
                                </span>
                              </button>
                            </li>
                            <li>
                              <button
                                onClick={() => handleClick('donator')}
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
                    {activeTabLeaderboard === 'followers' && <UkSlider sliderItems={leaderboard?.topAction} />}
                    {activeTabLeaderboard === 'donator' && <UkSlider sliderItems={leaderboard?.topMoney} />}
                  </div>
                </div>
              </section>
            </div>
          </div>
        </div>
      )}
      {activeTab === 'history-donate' && renderTabDonate()}
    </div>
  );
};
