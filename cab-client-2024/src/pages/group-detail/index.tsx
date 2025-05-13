import React, { useState } from 'react';
import GroupBanner from './components/group-banner.component';
import { useGroupHook } from '../../hooks/group/useGroup';
import { useParams } from 'react-router-dom';
import { usePostHook } from '../../hooks/post/usePost';
import { useHashTag } from '../../hooks/hashtag/useHashtag';
import { GroupPermissions } from '../../constants/group.constant';
import TimelineTab from './components/tabs/timeline-tab.component';
import MemberListTab from './components/tabs/member-list-tab.component';

const GroupDetail = () => {
  // get parms from url
  const { groupId } = useParams<{ groupId: string }>();
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [pageMemberNumber, setPageMemberNumber] = useState<number>(0);
  const [pageMemberRequestNumber, setPageMemberRequestNumber] = useState<number>(0);
  const { detail, permission, members, memberRequest } = useGroupHook({
    groupId,
    pageMemberNumber,
    pageMemberRequestNumber,
  });
  const { data: listPost, hasMore, loading, appendNewPost } = usePostHook({ pageNumber });
  const { getHashTags } = useHashTag();
  const tabs = [
    { name: 'Thảo luận', key: 'thao-luan', isShow: true },
    { name: 'Thành viên', key: 'thanh-vien', isShow: true },
    { name: 'Ảnh của nhóm', key: 'anh-cua-nhom', isShow: true },
    { name: 'Sự kiện', key: 'su-kien', isShow: true },
    { name: 'Giới thiệu', key: 'gioi-thieu', isShow: true },
    { name: 'Thiết lập', key: 'thiet-lap', isShow: permission === GroupPermissions.GROUP_ADMIN },
  ];
  const [activeTab, setActiveTab] = useState<string>(tabs[0].key);

  return (
    <div className="max-w-[1065px] mx-auto">
      {detail && <GroupBanner permission={permission} data={detail} />}
      <div className="lg:rounded-b-2xl bg-white shadow  flex items-center justify-between  border-t border-gray-100 px-2 dark:border-slate-700">
        <div className="flex gap-0.5 rounded-xl overflow-hidden -mb-px text-gray-500 font-medium text-sm overflow-x-auto dark:text-white">
          {tabs.map(
            (tab) =>
              tab.isShow && (
                <div
                  key={tab.key}
                  className={`inline-block py-3 leading-8 px-3.5 cursor-pointer ${
                    activeTab === tab.key ? 'border-b-2 border-blue-600 text-blue-600' : 'text-gray-500'
                  }`}
                  onClick={() => setActiveTab(tab.key)}
                >
                  {tab.name}
                </div>
              ),
          )}
        </div>
        <div className="flex items-center gap-1 text-sm p-3 bg-secondery py-2 mr-2 rounded-xl max-md:hidden dark:bg-white/5">
          <input placeholder="Search .." className="!bg-transparent" />
        </div>
      </div>
      {activeTab === 'thao-luan' && (
        <TimelineTab appendNewPost={appendNewPost} getHashTags={getHashTags} detail={detail} />
      )}
      {activeTab === 'thanh-vien' && <MemberListTab members={members} memberRequest={memberRequest} />}
    </div>
  );
};

export default GroupDetail;
