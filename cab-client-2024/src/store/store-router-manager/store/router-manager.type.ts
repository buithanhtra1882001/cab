import { ObservableSet } from 'mobx';
import { Router as RemixRouter } from '@remix-run/router';

export interface IRouterManagerStore {
  mainNavigationTabs: ObservableSet<string> | undefined;
  history: RemixRouter;
}
