import React, { useState } from 'react';
import { useGroupUserHook } from '../../../hooks/group/useGroupUser';
import GroupItem from '../../../components/group-items/group-item';
import ModalCreateGroup from '../../group/components/modal-create-group';

const Group = () => {
  const [activeTab, setActiveTab] = useState('tab1');
  const [searchQuery, setSearchQuery] = useState('');
  const [isOpenCreateGroup, setIsOpenCreateGroup] = useState(false);
  const handleOpenCreateGroup = () => {
    setIsOpenCreateGroup(true);
  };
  const handleCloseCreateGroup = () => {
    setIsOpenCreateGroup(false);
  };
  const handleSearch = () => {
    // Implement search functionality here
    console.log('Searching for:', searchQuery);
  };
  return (
    <div className="border border-solid border-palette-gray-200 rounded-lg shadow-xs pt-4 pb-6 bg-white mt-8">
      <div className='class="flex flex-col gap-4"'>
        <div className="flex justify-between px-4 pt-0 pb-4 border-b border-solid border-palette-gray-200">
          <div className="flex gap-2 text-palette-gray-500 overflow-auto w-fit md:min-w-fit">
            <button
              className={`px-4 py-2 rounded-md  font-semibold  ${
                activeTab === 'tab1' ? 'bg-[#eff8ff] text-[#175cd3]' : 'text-gray-700'
              }`}
              onClick={() => setActiveTab('tab1')}
            >
              Nhóm của tôi
            </button>
            <button
              className={`px-4 py-2 rounded-md  font-semibold  ${
                activeTab === 'tab2' ? 'bg-[#eff8ff] text-[#175cd3]' : 'text-gray-700'
              }`}
              onClick={() => setActiveTab('tab2')}
            >
              Đã tham gia
            </button>
            <button
              className={`px-4 py-2 rounded-md font-semibold ${
                activeTab === 'tab3' ? 'bg-[#eff8ff] text-[#175cd3]' : ' text-gray-700'
              }`}
              onClick={() => setActiveTab('tab3')}
            >
              Đã gửi yêu cầu
            </button>
          </div>
        </div>
      </div>
      <div className="px-4 flex gap-3 py-2">
        <input
          type="text"
          className="border border-gray-300 rounded-lg p-2 flex-1"
          placeholder="Search..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />
        <button className="bg-blue-500 text-white px-4 py-2 rounded-lg" onClick={handleOpenCreateGroup}>
          Tạo nhóm
        </button>
        <ModalCreateGroup isOpen={isOpenCreateGroup} onClose={handleCloseCreateGroup} />
      </div>
      <div className="p-4">
        {activeTab === 'tab1' && <Tab1Content />}
        {activeTab === 'tab2' && <Tab1Content />}
        {activeTab === 'tab3' && <Tab3Content />}
      </div>
    </div>
  );
};

Group.propTypes = {};

export default Group;
const Tab1Content = () => {
  const [pageNumber, setPageNumber] = useState<number>(1);
  const { data: listGroup, loading } = useGroupUserHook({ pageNumber });
  return (
    <div>
      {loading ? (
        <div>Loading...</div>
      ) : (
        listGroup.map((group, index) => (
          <div key={index}>
            <GroupItem group={group.group} joinedTheGroup={group.joinedTheGroup} userStatus={group.userStatus} />
          </div>
        ))
      )}
    </div>
  );
};
const Tab2Content = () => {
  return <div>Nội dung cho Đã tham gia</div>;
};
const Tab3Content = () => {
  const { groupsRequest: listGroupRequest, loading } = useGroupUserHook({ pageNumber: 1 });
  return (
    <div>
      {loading ? (
        <div>Loading...</div>
      ) : (
        listGroupRequest.map((group, index) => (
          <div key={index}>
            <GroupItem group={group.group} joinedTheGroup={group.joinedTheGroup} userStatus={group.userStatus} />
          </div>
        ))
      )}
    </div>
  );
};
