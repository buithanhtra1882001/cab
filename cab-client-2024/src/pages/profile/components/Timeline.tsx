import React, { useEffect, useState } from 'react';
import InfiniteScroll from 'react-infinite-scroll-component';
import { usePostUser } from '../../../hooks/post/usePostUser';
import { size } from 'lodash';
import Loading from '../../../components/loading/loading.component';
import CreatePost from '../../../components/create-post/create-post.component';
import EditProfiledDialog from '../../../components/edit-profile-dialog/edit-profile-dialog';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '../../../configuration';
import { fetchProfile } from '../../../redux/features/auth/slice';
import { tabLabels } from '../profile.page';
import { ChevronDown } from 'lucide-react';
import { Post } from '../../../components';
import UkSlider from '../../../components/slider/uk-slider.component';
import { fetchLeaderboardUserApi } from '../../../api/user-profile/leaderboard';

interface Props {
  profile: any;
}

const Timeline: React.FC<Props> = ({ profile }) => {
  const dispatch = useDispatch<AppDispatch>();
  const [leaderboard, setLeaderboard] = useState<any>();
  const [isDropdownVisible, setIsDropdownVisible] = useState(false);
  const [activeTabLeaderboard, setActiveTabLeaderboard] = useState('donator');
  const [pageNumber, setPageNumber] = useState<number>(1);
  const { data: listPost, hasMore, appendNewPost } = usePostUser({ pageNumber, userId: profile.userId });
  const handleUpdateProfileSuccess = async () => {
    await dispatch(fetchProfile());
  };
  const { city, school, company, isShowMarry } = profile;
  const handleClick = (tab: string) => {
    setActiveTabLeaderboard(tab);
    toggleDropdown();
  };
  const toggleDropdown = () => setIsDropdownVisible(!isDropdownVisible);
  useEffect(() => {
    const fetchLeaderboard = async () => {
      try {
        const response = await fetchLeaderboardUserApi(profile?.userId);
        setLeaderboard(response);
      } catch (error) {
        console.error('Failed to fetch leaderboard:', error);
      }
    };
    fetchLeaderboard();
  }, [profile?.userId]);
  return (
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
              <CreatePost
                onCreatedSuccess={(newPost) => {
                  appendNewPost(newPost);
                }}
              />
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
              <EditProfiledDialog profile={profile} onUpdateSuccess={handleUpdateProfileSuccess} />
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
              {/* <li className="flex items-center gap-3">
            <div>
              Flowwed By <span className="font-semibold text-black dark:text-white"> 3,240 People </span>
            </div>
          </li> */}
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
  );
};

Timeline.propTypes = {};

export default Timeline;
