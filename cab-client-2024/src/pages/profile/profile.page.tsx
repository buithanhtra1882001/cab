/* eslint-disable react/self-closing-comp */
import React, { useEffect, useState } from 'react';
import { Helmet } from 'react-helmet-async';
import ProfileBanner from '../../components/profile-banner/profile-banner.component';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../../configuration';
import { fetchProfile } from '../../redux/features/auth/slice';
import { useBlockerHook } from '../../hooks/use-blocker/useBlocker';
import WarningUpdateProfile from '../../components/warning-update-profile/warning-update-profile.component';
import { HandCoins, Lock } from 'lucide-react';
import { totalReciveDonate } from '../../api';
import dayjs from 'dayjs';
import RequestUnlockDonate from '../../components/request-unlock-donate/request-unlock-donate.component';
import Timeline from './components/Timeline';
import Group from './components/Group';

export const tabLabels = {
  followers: 'Top tương tác',
  receives: 'Top Receives',
  donator: 'Top Donator',
  // Add other tab keys and their labels here
};
export const ProfilePage = () => {
  const dispatch = useDispatch<AppDispatch>();
  const profile = useSelector((state: RootState) => state.features.auth.profile);
  const [activeTab, setActiveTab] = useState('timeline');
  const [totalRecivesDonate, setTotalRecivesDonate] = useState('0');
  const [historyDonate, setHistoryDonate] = useState<any>();

  const { showPrompt, setShowPrompt } = useBlockerHook({
    shouldBlock: !profile?.isUpdateProfile,
    shouldUnblock: false,
    disable: false,
  });

  useEffect(() => {
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
  }, [profile?.userId]);

  const handleUpdateProfileSuccess = async () => {
    await dispatch(fetchProfile());
  };

  if (!profile) {
    return null;
  }
  const changeTag = (tab) => {
    setActiveTab(tab);
  };
  const { username, description } = profile;
  console.log('❤️ ~ ProfilePage ~ profile:', profile);
  const currencyFormatter = new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
  });

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
        {profile.isCreateRequestReciveDonate ? (
          <p className="text-sm text-gray-500 mt-2 dark:text-white/80 p-5">Bạn đã gửi yêu cầu chờ phản hồi !</p>
        ) : (
          <>
            <p className="text-sm text-gray-500 mt-2 dark:text-white/80 p-5">Bạn chưa mở chức năng nhận donate</p>
            <RequestUnlockDonate />
          </>
        )}
      </div>
    );
  };

  return (
    <div className="max-w-[1065px] mx-auto max-lg:-m-2.5">
      <Helmet>
        <title>{username || ''} - Cab</title>
        <meta name="description" content={description} />
      </Helmet>
      <WarningUpdateProfile profile={profile} visible={showPrompt} onClose={() => setShowPrompt(false)} />
      <ProfileBanner
        tabActive={activeTab}
        profile={profile}
        onUpdateSuccess={handleUpdateProfileSuccess}
        changeTab={changeTag}
      />
      {activeTab === 'timeline' && <Timeline profile={profile} />}
      {activeTab === 'history-donate' && renderTabDonate()}
      {activeTab === 'group' && <Group />}
    </div>
  );
};
