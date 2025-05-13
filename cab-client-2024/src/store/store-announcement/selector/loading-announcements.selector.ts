import { CoreLoadingStore } from '../../store-loading';
import { getStore } from '../store';

export const loadingAnnouncementsSelector = () => {
  return CoreLoadingStore.localLoadingSelector(getStore().loadingProcessId);
};
