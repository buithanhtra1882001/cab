import React, { useEffect, useState } from 'react';
import { IShareProps } from './share.type';
import { Button } from '../button';
import dayjs from 'dayjs';
import { useNavigate } from 'react-router-dom';
import { userListShare, contentSelectPublic } from './mocks/user-list-share.mock';
import { useSharePost } from '../../hooks/share-post/share-post-public';
import { useSharePostFriend } from '../../hooks/share-post/share-post-friend';
import { useUseSharePostFriend } from '../../hooks/share-post/get-user-share';
import { FetchShareParams, FetchShareFriendParams } from '../../models/share.model';
import { useUserProfile } from '../../hooks';
import InfiniteScroll from 'react-infinite-scroll-component';
import Loading from '../loading/loading.component';

export const Share: React.FC<IShareProps> = (props) => {
  const { fetchPostShare } = useSharePost();

  const { fetchPostShareFriend } = useSharePostFriend();

  const { userListShareFriendItems, isEndFetchingGetListUser, fetchUserListShareFriend } = useUseSharePostFriend();

  const { profile } = useUserProfile();

  const { post } = props;

  const navigate = useNavigate();

  const [params, setParams] = useState<FetchShareParams>({});

  const [paramsFriend, setParamsFriend] = useState<FetchShareFriendParams>({});

  const [selectedOption, setSelectedOption] = useState(contentSelectPublic.public);

  const [selectedUsers, setSelectedUsers] = useState<string[]>([]);

  const [searchKeyword, setSearchKeyword] = useState('');

  const handleSearchChange = (event) => {
    setSearchKeyword(event.target.value);
  };

  const filteredUserList = userListShare?.userInfo.filter((user) =>
    user.name.toLowerCase().includes(searchKeyword.toLowerCase()),
  );

  useEffect(() => {
    if (selectedOption === contentSelectPublic.public) {
      const param = {
        userId: profile ? profile.userId : '3fa85f64-5717-4562-b3fc-2c963f66afa6',
        postId: post?.id,
        shareLink: '',
        isPublic: selectedOption === contentSelectPublic.public,
      };
      setParams(param);
    } else if (selectedOption === contentSelectPublic.friend) {
      const paramsFriend = {
        userId: profile ? profile.userId : '3fa85f64-5717-4562-b3fc-2c963f66afa6',
        postId: post?.id,
        sharedUserIds: selectedUsers,
        shareLink: '',
        isPublic: selectedOption === contentSelectPublic.friend,
      };
      setParamsFriend(paramsFriend);
    }
  }, [selectedOption, selectedUsers]);

  const handleSelectChange = (value) => {
    setSelectedOption(value);
  };

  const handleRadioChange = (event) => {
    console.log(selectedUsers);
    const userId = event.target.value;
    setSelectedUsers((prevSelectedUsers) => {
      if (prevSelectedUsers.includes(userId)) {
        return prevSelectedUsers.filter((id) => id !== userId);
      }
      return [...prevSelectedUsers, userId];
    });
  };

  return (
    <div className="px-4 py-2">
      <div style={{ marginLeft: '20px' }}>
        <label htmlFor="selectBox" className="mr-30">
          Bạn muốn chia sẻ ở chế độ
        </label>
        <select
          id="selectBox"
          value={selectedOption}
          className="font-medium text-black border border-gray-10 dark:border-gray-1 dark:bg-zinc-900 dark:text-gray-3 ml-2 rounded-md px-1 py-1/2"
          onChange={(event) => {
            handleSelectChange(event.target.value);
          }}
        >
          <option value="Công khai">Công khai</option>
          <option value="Bạn bè">Bạn bè</option>
        </select>
      </div>
      {selectedOption === contentSelectPublic.public && (
        <div className="mb-4 post-item">
          <div className="rounded-xl flex transition duration-200 flex-col">
            <div className="p-4">
              <div>
                <div className="grid grid-cols-2 gap-1 mb-4">
                  {/* {post?.imageUrls &&
                    post.imageUrls.map((img, key) => {
                      if (key > 3) return null;

                      return (
                        <button
                          key={Math.random().toString(36).substring(2, 15)}
                          className="relative block rounded-md overflow-hidden"
                        >
                          {key === 3 && (
                            <div className="absolute z-[1] top-0 left-0 rounded-md w-full h-full lg:w-[270px] bg-black/50 text-white font-medium backdrop-blur p-4">
                              <div className="h-full flex justify-center items-center">Xem thêm</div>
                            </div>
                          )}
                          <img className="rounded-md object-cover w-full h-full lg:w-[270px]" src={img} alt="" />
                        </button>
                      );
                    })} */}
                </div>
                <div />
              </div>
              <div className="flex justify-between items-center">
                <div className="flex">
                  <div className="flex items-center mr-4">
                    <img className="rounded-full h-10 w-10" src={post?.userAvatar ?? ''} alt="" />
                  </div>
                  <div>
                    <a className="inline-block text-black dark:text-white text-sm font-bold mb-[2px]" href="/u/a">
                      {post?.userFullName ?? 'Người dùng không xác định'}
                    </a>
                    <div className="flex items-center space-x-2 text-xs">
                      <a className="text-branding font-semibold mr-2" href="/c/a">
                        Công nghệ
                      </a>{' '}
                      <span>•</span> <span>{dayjs(post?.updatedAt).fromNow()} trước</span> <span />
                      <Button>
                        <i className="fas fa-trophy" />
                      </Button>
                    </div>
                  </div>
                </div>
              </div>
              <div className="inline-block text-lg font-bold py-4">{post?.title}</div>
              <p className="text-sm">{post?.content}</p>
              <ul className="flex text-branding text-sm py-4">
                {Array.isArray(post?.hashtags) &&
                  post?.hashtags.map((ht, index) => {
                    return (
                      <li className="mr-1" key={post?.hashtag_Ids?.[index]}>
                        <a href={`/hashtag/${post?.hashtag_Ids?.[index]}`}>#{ht}</a>
                      </li>
                    );
                  })}
              </ul>
            </div>
          </div>
          <Button
            className="w-full bg-primary-9 text-white font-bold py-2 px-4 rounded-md"
            onClick={() => {
              fetchPostShare(params);
            }}
          >
            Chia sẻ
          </Button>
        </div>
      )}
      {selectedOption === contentSelectPublic.friend && (
        <div className="max-w-xl mx-auto mt-8 p-4">
          <div className="mb-4">
            <input
              type="text"
              placeholder="Tìm kiếm..."
              className="w-full border border-gray-300 p-2 rounded-md focus:outline-none focus:border-blue-500 dark:bg-gray-800 dark:text-gray-300"
              value={searchKeyword}
              onChange={handleSearchChange}
            />
          </div>
          <form>
            <ul className="flex flex-col w-">
              <InfiniteScroll
                dataLength={filteredUserList?.length || 0}
                next={fetchUserListShareFriend}
                hasMore={isEndFetchingGetListUser}
                loader={<Loading />}
              >
                {filteredUserList?.map((user) => (
                  <li key={user.id} className="flex justify-between items-center space-x-4 border-b py-2">
                    <div className="flex items-center space-x-4">
                      <img src={user.avatar} alt={user.name} className="w-10 h-10 rounded-full object-cover" />
                      <span className="text-gray-800 font-bold dark:text-white">{user.name}</span>
                    </div>
                    <div>
                      <input
                        type="checkbox"
                        name="selectedUsers"
                        value={user.id}
                        onChange={handleRadioChange}
                        checked={selectedUsers.includes(user.id.toString())}
                        className="w-4 h-4 mr-2 text-primary-600 border-gray-300 rounded-md focus:ring-primary-400 dark:border-gray-600 dark:focus:ring-primary-500"
                      />
                    </div>
                  </li>
                ))}
              </InfiniteScroll>
            </ul>
          </form>
          <Button
            className="w-full bg-primary-9 text-white font-bold py-2 px-4 rounded-md"
            onClick={() => {
              if (selectedUsers.length > 0) {
                fetchPostShareFriend(paramsFriend);
              }
              setSelectedUsers([]);
            }}
          >
            Chia sẻ
          </Button>
        </div>
      )}
    </div>
  );
};
