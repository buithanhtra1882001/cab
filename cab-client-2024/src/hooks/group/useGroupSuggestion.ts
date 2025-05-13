import { useEffect, useState } from 'react';
import { GroupDatas, GroupElement } from '../../models';
import { groupService } from '../../services/group.service';

export const useGroupSuggestionHook = () => {
  const [groups, setGroups] = useState<GroupDatas[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [group, setGroup] = useState<GroupElement | null>(null);

  const appendNewGroup = (newGroup: GroupElement) => {
    // setGroups((prev) => [newGroup, ...prev]);
  };

  const getGroups = async () => {
    try {
      setLoading(true);
      const { data } = await groupService.getGroupSuggestion();
      setGroups((prev) => [...prev, ...data]);
    } catch {
      //
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getGroups();
  }, []);

  return {
    data: groups,
    loading,
    detail: group,
    appendNewGroup,
  };
};
