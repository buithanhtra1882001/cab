import { IUserProfile } from '../../../models';
import { IBalance } from '../../../configuration/balance';
import { IObservableValue } from 'mobx';

export interface IUserProfileStore {
  profile: IUserProfile | undefined;
  isFetchingProfile: boolean;
  fetchProfileError: IObservableValue<string | undefined | unknown>;

  balance: IBalance | undefined;
  isFetchingBalance: boolean;
  fetchBalanceError: IObservableValue<string | undefined | unknown>;
}
