import { getStore } from '../store';

export const announcementsSelector = () => {
  return getStore().announcements || [];
};
