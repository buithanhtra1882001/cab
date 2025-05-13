import { orchestrator } from 'satcheljs';
import { fetchBalanceAction } from '../action';
import { updateBalanceAction, updateFetchBalanceError, updateIsFetchingBalance } from '../mutator-action';
import { fetchMyBalance } from '../../../configuration/balance';

let isFetching = false;

orchestrator(fetchBalanceAction, async () => {
  if (isFetching) return;
  isFetching = true;
  try {
    updateIsFetchingBalance(true);
    updateBalanceAction(await fetchMyBalance());
  } catch (error) {
    updateFetchBalanceError(error);
  } finally {
    updateIsFetchingBalance(false);
  }
  isFetching = false;
});
