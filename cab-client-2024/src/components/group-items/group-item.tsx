import React from 'react';
import { Link } from 'react-router-dom';
import { groupService } from '../../services/group.service';
import toast from 'react-hot-toast';

interface GroupItemProps {
  group: {
    id: string;
    groupName: string;
    groupCoverImageUrl: string;
    memberCount: number;
  };
  joinedTheGroup: boolean;
  userStatus: string;
}

const GroupItem: React.FC<GroupItemProps> = ({ group, joinedTheGroup, userStatus }) => {
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
    <div className="flex md:items-center space-x-4 p-4 rounded-md box">
      <div className="sm:w-20 w-14 sm:h-20 h-14 flex-shrink-0 rounded-lg relative">
        <img
          src={group.groupCoverImageUrl}
          className="absolute w-full h-full inset-0 rounded-md object-cover shadow-sm"
          alt=""
        />
      </div>
      <div className="flex-1">
        <Link
          to={`/group/${group.id}`}
          className="md:text-lg text-base font-semibold capitalize text-black dark:text-white"
        >
          {group.groupName}
        </Link>
        <div className="flex space-x-2 items-center text-sm font-normal">
          <div> {group.memberCount} Members</div>
        </div>
      </div>
      <button
        type="button"
        className={`button   dark:text-white gap-1 max-md:hidden ${
          userStatus === 'PENDING' ? 'bg-red-200 text-red-500' : 'bg-primary-soft text-primary'
        }`}
        onClick={() => {
          if (!userStatus) {
            handleJoinGroup(group.id);
          } else if (userStatus.toLocaleLowerCase() === 'PENDING'.toLocaleLowerCase()) {
            handleCancelJoinGroup(group.id);
          }
        }}
      >
        {joinedTheGroup ? 'đã tham gia' : userStatus === 'PENDING' ? ' Huỷ yêu cầu' : 'tham gia'}
      </button>
    </div>
  );
};

export default GroupItem;
