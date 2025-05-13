import { orchestrator } from 'satcheljs';
import { checkAndLoadAnnouncementsAction } from '../action';
import { fetchAnnouncementsAPI } from '../api';
import { getStore } from '../store';
import { CommonMessageStore, CoreLoadingStore } from '../..';

orchestrator(checkAndLoadAnnouncementsAction, (actionMessage) => {
  const { refresh } = actionMessage;
  const store = getStore();

  if (refresh || !store.announcements) {
    CoreLoadingStore.updateLocalLoadingAction(store.loadingProcessId, true);
    fetchAnnouncementsAPI()
      .then((announcements) => {
        store.announcements = announcements;
      })
      .catch((error) => {
        CommonMessageStore.updateLocalErrorAction(store.localErrorId, error);
      })
      .finally(() => {
        CoreLoadingStore.updateLocalLoadingAction(store.loadingProcessId, false);
      });
  }
});
