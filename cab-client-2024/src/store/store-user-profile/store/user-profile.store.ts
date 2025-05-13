import { observable } from 'mobx';
import { createStore } from 'satcheljs';
import { IUserProfileStore } from './user-profile.type';

const initStore: IUserProfileStore = {
  profile: undefined,
  isFetchingProfile: false,
  fetchProfileError: observable.box(undefined),

  balance: undefined,
  isFetchingBalance: false,
  fetchBalanceError: observable.box(undefined),
};

export const getStore = createStore('USER_PROFILE_STORE', initStore);
