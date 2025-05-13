import React, { useState } from 'react';
import { useGroupHook } from '../../hooks/group/useGroup';
import { Link } from 'react-router-dom';
import { useGroupSuggestionHook } from '../../hooks/group/useGroupSuggestion';
import { groupService } from '../../services/group.service';
import toast from 'react-hot-toast';

const GroupPage = () => {
  const [pageNumber, setPageNumber] = useState<number>(1);
  const { data: listGroup } = useGroupHook({ pageNumber });
  const { data: listGroupSuggestion } = useGroupSuggestionHook();
  const handleJoinGroup = async (groupId: string) => {
    const res = await groupService.joinGroup(groupId, 2, 'test');
    if (res.status === 200) {
      toast.success('đã gửi yêu cầu tham gia nhóm thành công');
    }
  };
  const handleCancelJoinGroup = async (groupId: string) => {
    const res = await groupService.cancelJoinGroup(groupId);
    if (res.status === 200) {
      toast.success('đã hủy yêu cầu tham gia nhóm thành công');
    }
  };
  return (
    <div className="2xl:max-w-[1220px] max-w-[1065px] mx-auto">
      <div className="page-heading">
        <h1 className="page-title"> Nhóm </h1>

        {/* <nav className="nav__underline">
          <ul className="group flex">
            <li>
              <li>
                {' '}
                <a href="#"> Suggestions </a>{' '}
              </li>
              <li>
                {' '}
                <a href="#"> Popular </a>{' '}
              </li>
              <li>
                {' '}
                <a href="#"> My groups </a>{' '}
              </li>
            </li>
          </ul>
        </nav> */}
        <div className="grid md:grid-cols-4 sm:grid-cols-3 grid-cols-2 gap-2.5 pt-3">
          {listGroup.map((group, index) => (
            <div key={index.toString()} className="card">
              <Link to={`/group/${group.group.id}`}>
                <div className="card-media h-24">
                  <img src={group.group.groupCoverImageUrl} alt="" />
                  <div className="card-overly" />
                </div>
              </Link>
              <div className="card-body relative z-10">
                <Link to={`/group/${group.group.id}`}>
                  <h4 className="card-title"> {group.group.groupName} </h4>
                </Link>
                <div className="card-list-info font-normal mt-1">
                  <a href="#"> {group.group.tagList} </a>
                  <div className="md:block hidden">·</div>
                  <div> {group.group.memberCount} members </div>
                </div>
                <div className="flex gap-2">
                  <button
                    type="button"
                    className={`button  text-white flex-1 ${
                      group.userStatus === 'PENDING' ? 'bg-red-500' : 'bg-primary'
                    }`}
                    onClick={() => {
                      if (!group.userStatus) {
                        handleJoinGroup(group.group.id);
                      } else if (group.userStatus.toLocaleLowerCase() === 'PENDING'.toLocaleLowerCase()) {
                        handleCancelJoinGroup(group.group.id);
                      }
                    }}
                  >
                    {group.joinedTheGroup
                      ? 'đã tham gia'
                      : group.userStatus === 'PENDING'
                      ? ' Huỷ yêu cầu'
                      : 'tham gia'}
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
        <div className="sm:my-6 my-3 flex items-center justify-between lg:mt-10">
          <div>
            <h2 className="md:text-lg text-base font-semibold text-black"> Gợi ý </h2>
            <p className="font-normal text-sm text-gray-500 leading-6"> Tìm nhóm mà bạn có thể thích. </p>
          </div>
        </div>
        <div className="grid md:grid-cols-2 md:gap-2 gap-3">
          {listGroupSuggestion.map((group, index) => (
            <div key={index.toString()} className="flex md:items-center space-x-4 p-4 rounded-md box">
              <div className="sm:w-20 w-14 sm:h-20 h-14 flex-shrink-0 rounded-lg relative">
                <img
                  src={group.group.groupCoverImageUrl}
                  className="absolute w-full h-full inset-0 rounded-md object-cover shadow-sm"
                  alt=""
                />
              </div>
              <div className="flex-1">
                <Link
                  to={`/group/${group.group.id}`}
                  className="md:text-lg text-base font-semibold capitalize text-black dark:text-white"
                >
                  {group.group.groupName}
                </Link>
                <div className="flex space-x-2 items-center text-sm font-normal">
                  <div> {group.group.memberCount} Members</div>
                </div>
              </div>
              <button
                type="button"
                className={`button   dark:text-white gap-1 max-md:hidden ${
                  group.userStatus === 'PENDING' ? 'bg-red-200 text-red-500' : 'bg-primary-soft text-primary'
                }`}
                onClick={() => {
                  if (!group.userStatus) {
                    handleJoinGroup(group.group.id);
                  } else if (group.userStatus.toLocaleLowerCase() === 'PENDING'.toLocaleLowerCase()) {
                    handleCancelJoinGroup(group.group.id);
                  }
                }}
              >
                {group.joinedTheGroup ? 'đã tham gia' : group.userStatus === 'PENDING' ? ' Huỷ yêu cầu' : 'tham gia'}
              </button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default GroupPage;
