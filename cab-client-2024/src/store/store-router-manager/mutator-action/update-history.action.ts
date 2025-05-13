import { mutatorAction } from 'satcheljs';
import { getStore } from '../store';
import { Router as RemixRouter } from '@remix-run/router';

export const updateHistoryAction = mutatorAction('updateHistoryAction', (history: RemixRouter) => {
  const store = getStore();

  store.history = history;
});
