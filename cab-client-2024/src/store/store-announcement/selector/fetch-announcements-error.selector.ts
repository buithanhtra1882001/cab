import { CommonMessageStore } from '../../store-common-message';
import { getStore } from '../store';
import { IObservableValue } from 'mobx';

export const fetchAnnouncementsErrorSelector = (): IObservableValue<CommonMessageStore.TToastMessage> | undefined => {
  return CommonMessageStore.localErrorSelector(getStore().localErrorId);
};
