import { useEffect, useState } from 'react';
import { FetchPostParams, GroupDatas, GroupElement } from '../../models';
import { groupService } from '../../services/group.service';

export const useGroupUserHook = ({ pageNumber }: { pageNumber?: number }) => {
  const [groups, setGroups] = useState<GroupDatas[]>([]);
  const [groupsRequest, setGroupsRequest] = useState<GroupDatas[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  const appendNewGroup = (newGroup: GroupElement) => {
    // setGroups((prev) => [newGroup, ...prev]);
  };

  const getGroups = async (payload: FetchPostParams) => {
    try {
      setLoading(true);
      const { data } = await groupService.getGroupOfUser(payload);
      setGroups((prev) => [...prev, ...data.elements]);
      const { data: dataRequest } = await groupService.getGroupRequestJoinOfUser(payload);
      setGroupsRequest((prev) => [...prev, ...dataRequest.elements]);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getGroups({
      pageNumber,
      pageSize: 10,
    });
  }, [pageNumber]);

  return {
    data: groups,
    loading,
    appendNewGroup,
    groupsRequest,
  };
};
