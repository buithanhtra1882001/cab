import { API } from '..';
import { FetchNotificationParams, INotificationModel } from '../../models';

export const fetchNotificationsApi = (params: FetchNotificationParams) => {
  return new Promise<INotificationModel[]>((resolve, reject) => {
    API.post('/v1/notification-service/notifications/user-notifications', {
      params,
    })
      .then((res) => {
        resolve(res.data.data);
      })
      .catch(reject);
  });
};
