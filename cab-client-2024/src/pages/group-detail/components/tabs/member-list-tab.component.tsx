import React from 'react';

const MemberListTab = ({ members, memberRequest }: { members: any[]; memberRequest: any[] }) => {
  return (
    <div className="p-4 mt-8">
      <h2 className="text-2xl font-bold mb-4">Thành viên</h2>
      <div className="bg-white rounded-lg border p-4 shadow-lg mb-4">
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {members?.map((member) => (
            <div key={member.userId} className="bg-white p-4 rounded-lg  flex gap-2 items-center ">
              <img src={member.avatar} alt={member.name} className="w-16 h-16 rounded-full mx-auto mb-3" />
              <div className="text-left">
                <h3 className="font-semibold text-lg">{member.fullName}</h3>
                <p className="text-sm text-gray-500">
                  {['admin', 'moderator'].includes(member.role) ? member.role : 'Member'}
                </p>
              </div>
            </div>
          ))}
          {members.length === 0 && <p className="text-center text-gray-500">Không có thành viên nào</p>}
        </div>
      </div>

      <h2 className="text-2xl font-bold mb-4">Danh sách chờ phê duyệt</h2>
      <div className="bg-white rounded-lg border p-4 shadow-lg">
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {memberRequest?.map((member) => (
            <div key={member.userId} className="bg-white p-4 rounded-lg  flex gap-2 items-center ">
              <img src={member.avatar} alt={member.name} className="w-16 h-16 rounded-full mx-auto mb-3" />
              <div className="text-left">
                <h3 className="font-semibold text-lg">{member.fullName}</h3>
                <p className="text-sm text-gray-500">
                  {['admin', 'moderator'].includes(member.role) ? member.role : 'Member'}
                </p>
              </div>
            </div>
          ))}
          {memberRequest.length === 0 && <p className="text-center text-gray-500">Không có yêu cầu chờ phê duyệt</p>}
        </div>
      </div>
    </div>
  );
};

export default MemberListTab;
