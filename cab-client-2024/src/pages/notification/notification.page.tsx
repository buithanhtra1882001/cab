import React from 'react';
import { INotificationPageProps } from './notification.type';
import { NotificationItem } from './components';
import InfiniteScroll from 'react-infinite-scroll-component';
import { useNotification } from './notification.hook';
import { Helmet } from 'react-helmet-async';
import Loading from '../../components/loading/loading.component';
import _ from 'lodash';

export const NotificationPage: React.FC<INotificationPageProps> = (props) => {
  // const { isDropdown } = props;

  const { notificationItems, isEndFetchingNotification, fetchNotification } = useNotification();

  return (
    <div className="notification-page flex justify-center py-6">
      <Helmet>
        {/* <title>Notification Page</title>
        <meta name="home" content="Notification Page" /> */}
      </Helmet>

      <div className="max-w-2xl w-full rounded-md">
        <InfiniteScroll
          dataLength={notificationItems?.length || 0}
          next={fetchNotification}
          hasMore={isEndFetchingNotification}
          loader={<Loading />}
          endMessage={null}
        >
          {_.map(notificationItems, (rc, index) => (
            <div key={_.get(rc, 'id', '') + index}>
              <NotificationItem
                imageUrl={_.get(rc, 'actor.avatar')}
                content={_.get(rc, 'message')}
                timeStamp={_.get(rc, 'createdAt')}
                userName={_.get(rc, 'actor.fullName')}
              />
            </div>
          ))}
        </InfiniteScroll>
      </div>
    </div>
  );
};
