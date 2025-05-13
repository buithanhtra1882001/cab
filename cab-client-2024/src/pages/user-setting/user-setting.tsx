import { Lock, LockKeyhole, ShieldCheck } from 'lucide-react';
import React, { useState } from 'react';
import ImageGif from '../../assets/gif/pikachu.gif';
import ReactAudioPlayer from 'react-audio-player';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import RequestUnlockDonate from '../../components/request-unlock-donate/request-unlock-donate.component';

const UserSetting = (props) => {
  const [activeTab, setActiveTab] = useState('tab1');
  const { host, protocol } = window.location;
  console.log('🚀 ~ UserSetting ~ protocol:', protocol);
  const profile = useSelector((state: RootState) => state.features.auth.profile);
  const openPopupWindow = () => {
    const url = `${protocol}//${host}/donate-screen/${profile?.userId}`;
    const windowFeatures = 'width=600,height=400,top=100,left=100';
    window.open(url, 'popup', windowFeatures);
  };
  const renderTabDonate = () => {
    if (profile?.canReceiveDonation) {
      return (
        <div className="box flex justify-center items-center flex-col gap-2 py-4">
          <div className="flex flex-col flex-start w-full px-6">
            <h2>Link donate của bạn</h2>
            <div className="text-xl text-start font-bold py-3 pl-3 px-14 border bg-gray-200 rounded-lg">
              {`${protocol}//${host}/user/${profile?.userId}`}
            </div>
          </div>
          <div className="flex flex-col flex-start w-full px-6">
            <h2>Link hiển thị donate ( Add Webpage URL trong phần mềm Stream )</h2>
            <div className="text-xl text-start font-bold py-3 pl-3 px-14 border bg-gray-200 rounded-lg">
              {`${protocol}//${host}/donate-screen/${profile?.userId}`}
            </div>
          </div>
          <img alt="gif" src={ImageGif} />
          <ReactAudioPlayer src="https://files.playerduo.net/production/images/donate_sound/14.mp3" controls />
          <button onClick={openPopupWindow} className="button bg-blue-500 text-white py-2 px-12 text-[14px]">
            Test
          </button>
        </div>
      );
    }
    return (
      <div className="flex flex-col justify-center items-center h-fix mt-10 box py-4">
        <Lock size={50} />
        {profile?.isCreateRequestReciveDonate ? (
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
    <div className="flex flex-row justify-evenly gap-6 max-w-[1320px] mx-auto mt-24">
      {/* Tab selectors */}
      <div className="flex flex-col w-[320px]">
        <div
          className={`py-4 flex gap-1 font-semibold ${activeTab === 'tab1' ? 'bg-blue-100 rounded-md ' : ''}`}
          onClick={() => setActiveTab('tab1')}
        >
          <div
            className={`border-[1.5px]  rounded-r-[3px] h-6 ${
              activeTab === 'tab1' ? 'border-blue-600' : 'border-transparent'
            } `}
          />
          <ShieldCheck />
          <span className={`px-2 ${activeTab === 'tab1' ? 'text-blue-600' : ''}`}>Yêu cầu xác minh</span>
        </div>
        <div
          className={`py-4 flex gap-1 font-semibold ${activeTab === 'tab2' ? 'bg-blue-100 rounded-md ' : ''}`}
          onClick={() => setActiveTab('tab2')}
        >
          <div
            className={`border-[1.5px]  rounded-r-[3px] h-6 ${
              activeTab === 'tab2' ? 'border-blue-600' : 'border-transparent'
            } `}
          />
          <LockKeyhole />
          <span className={`px-2 ${activeTab === 'tab2' ? 'text-blue-600' : ''}`}>Bảo mật</span>
        </div>
        <div
          className={`py-4 flex gap-1 font-semibold ${activeTab === 'donate' ? 'bg-blue-100 rounded-md ' : ''}`}
          onClick={() => setActiveTab('donate')}
        >
          <div
            className={`border-[1.5px]  rounded-r-[3px] h-6 ${
              activeTab === 'donate' ? 'border-blue-600' : 'border-transparent'
            } `}
          />
          <LockKeyhole />
          <span className={`px-2 ${activeTab === 'donate' ? 'text-blue-600' : ''}`}>Donate</span>
        </div>
      </div>

      {/* Content area */}
      <div className="rounded-md w-[894px] h-fit bg-none p-2">
        {activeTab === 'tab1' && (
          <div className="bg-white shadow-lg p-4 rounded-md">
            <span className="font-bold text-lg">Yêu cầu xác minh cho cá nhân</span>
            <div className="text-sm text-gray-600">
              <span className=" ">Để bảo vệ tài khoản của bạn, chúng tôi cần xác minh thông tin cá nhân của bạn.</span>
              <ul className="pl-6 py-2 list-disc">
                <li className="ng-star-inserted">
                  Tính xác thực: Tài khoản phải đại diện cho một cá nhân, doanh nghiệp hoặc thực thể thực sự. Điều này
                  có nghĩa là tài khoản phải được tạo bởi cá nhân hoặc tổ chức mà nó đại diện và thông tin trên tài
                  khoản phải chính xác.
                </li>
                <li className="ng-star-inserted">
                  Tính đầy đủ: Tài khoản phải có tiểu sử, ảnh hồ sơ và ít nhất một bài đăng. Tài khoản cũng phải được
                  công khai và hoạt động.
                </li>
                <li className="ng-star-inserted">
                  Tuân thủ: Tài khoản phải tuân thủ các điều khoản dịch vụ của mạng xã hội và nguyên tắc cộng đồng.
                </li>
                <li className="ng-star-inserted">
                  Độ nổi bật: Tài khoản phải đại diện cho cá nhân, thương hiệu hoặc tổ chức nổi tiếng, được tìm kiếm
                  nhiều. Điều này có nghĩa là tài khoản phải có lượng người theo dõi đáng kể và được nhắc đến trên nhiều
                  phương tiện truyền thông.
                </li>
                <li className="ng-star-inserted">
                  Tính duy nhất: Mỗi người hoặc tổ chức chỉ có thể có một tài khoản được xác minh. Điều này có nghĩa là
                  nếu bạn có nhiều tài khoản cho cùng một người hoặc tổ chức thì chỉ một trong những tài khoản đó có thể
                  được xác minh.
                </li>
              </ul>
              <div className="flex justify-end">
                <button className="bg-blue-600 text-white px-4 py-2 rounded-md">Xác minh ngay</button>
              </div>
            </div>
          </div>
        )}
        {activeTab === 'tab2' && <div>Content for Tab 2</div>}
        {activeTab === 'donate' && renderTabDonate()}
      </div>
    </div>
  );
};

export default UserSetting;
