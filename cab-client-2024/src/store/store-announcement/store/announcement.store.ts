import { createStore } from 'satcheljs';
import { IAnnouncementStore } from './announcement.type';

const initStore: IAnnouncementStore = {
  announcements: undefined,
  loadingProcessId: 'announcement-loading',
  localErrorId: 'announcement-local-error',
};

export const getStore = createStore('announcementStore', initStore);
