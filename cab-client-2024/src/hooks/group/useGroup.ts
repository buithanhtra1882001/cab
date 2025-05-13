import { useEffect, useState } from 'react';
import { CreateGroupRequest, FetchPostParams, GroupDatas, GroupElement, UserElement } from '../../models';
import { groupService } from '../../services/group.service';
import toast from 'react-hot-toast';

export const useGroupHook = ({
  pageNumber,
  groupId,
  pageMemberNumber,
  pageMemberRequestNumber,
}: {
  pageNumber?: number;
  groupId?: string;
  pageMemberNumber?: number;
  pageMemberRequestNumber?: number;
}) => {
  const [groups, setGroups] = useState<GroupDatas[]>([]);
  const [groupPermission, setGroupPermission] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);
  const [group, setGroup] = useState<GroupElement | null>(null);
  const [users, setUser] = useState<UserElement[]>([]);
  const [memberRequest, setMemberRequest] = useState<UserElement[]>([]);
  const appendNewGroup = async (newGroup: CreateGroupRequest) => {
    try {
      const { data } = await groupService.createGroup(newGroup);
      if (data) {
        toast.success('Tạo nhóm thành công');
        setGroups((prev) => [data, ...prev]);
      }
    } catch {
      toast.error('Tạo nhóm thất bại');
    }
  };

  const getGroups = async (payload: FetchPostParams) => {
    try {
      setLoading(true);
      const { data } = await groupService.getGroup(payload);
      setGroups((prev) => [...prev, ...data.elements]);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };
  const getGroupDetail = async (id: string) => {
    try {
      const { data } = await groupService.getGroupDetail(id);
      setGroup(data);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };
  const getGroupPermission = async (id: string) => {
    try {
      const { data } = await groupService.getGroupPermission(id);
      setGroupPermission(data);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };
  const getGroupMembers = async (id: string, payload: FetchPostParams) => {
    try {
      const { data } = await groupService.getGroupMembers(id, payload);
      setUser(data?.elements);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };
  const getGroupMemberRequest = async (id: string, payload: FetchPostParams) => {
    try {
      const { data } = await groupService.getGroupMemberRequest(id, payload);
      setMemberRequest(data?.elements);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    groupId && getGroupDetail(groupId);
    groupId && getGroupPermission(groupId);
    groupId && getGroupMembers(groupId, { pageNumber: pageMemberNumber, pageSize: 10 });
    groupId && getGroupMemberRequest(groupId, { pageNumber: pageMemberRequestNumber, pageSize: 10 });
  }, [groupId]);

  useEffect(() => {
    getGroups({
      pageNumber,
      pageSize: 10,
    });
  }, [pageNumber]);

  return {
    data: groups,
    loading,
    detail: group,
    appendNewGroup,
    permission: groupPermission,
    members: users,
    memberRequest,
  };
};
