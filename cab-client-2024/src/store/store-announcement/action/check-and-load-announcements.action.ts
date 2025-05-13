import { action } from 'satcheljs';

export const checkAndLoadAnnouncementsAction = action('checkAndLoadAnnouncementsAction', (refresh: boolean = false) => {
  return {
    refresh,
  };
});
