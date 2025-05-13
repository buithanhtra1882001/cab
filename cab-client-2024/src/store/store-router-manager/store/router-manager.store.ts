import { createStore } from 'satcheljs';
import { IRouterManagerStore } from './router-manager.type';
import { RouterLinks } from '../../../routes/router';

const initStore: IRouterManagerStore = {
  mainNavigationTabs: undefined,
  history: RouterLinks,
};

export const getStore = createStore<IRouterManagerStore>('RouterManagerStore', initStore);
