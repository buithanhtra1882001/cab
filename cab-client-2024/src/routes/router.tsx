import React from 'react';
import { createBrowserRouter } from 'react-router-dom';
import {
  HomePage,
  LoginPage,
  RegisterPage,
  ProfilePage,
  NotFoundPage,
  NotificationPage,
  CreatorRequest,
  VerifyAccountPage,
  ProfileUserPage,
  FriendPage,
} from '../pages';

import { routePaths } from './routes-path';
import DefaultLayout from '../components/layouts/default-layout';
import BlankLayout from '../components/layouts/blank-layout';
import PrivateRoute from '../components/route/private-route.component';
import { ListFriendPage } from '../pages/list-friend';
import { ChatPage } from '../pages/chat';
import { VideosPage } from '../pages/videos';
import VideosLayout from '../components/layouts/videos-layout';
import UserSetting from '../pages/user-setting/user-setting';
import PageDonateLivestream from '../pages/page-donate-livestream/page-donate-livestream.component';
import GroupPage from '../pages/group/group.page';
import GroupDetail from '../pages/group-detail';

export const RouterLinks = createBrowserRouter([
  {
    path: '/',
    children: [
      {
        element: <DefaultLayout />,
        children: [
          {
            element: <PrivateRoute />,
            children: [
              {
                index: true,
                element: <HomePage />,
              },
              {
                path: routePaths.notifications,
                Component: NotificationPage,
              },
              {
                path: routePaths.creatorsRequest,
                Component: CreatorRequest,
              },
              {
                path: routePaths.profile,
                element: <ProfilePage />,
              },
              {
                path: routePaths.chat,
                element: <ChatPage />,
              },
              {
                path: routePaths.group,
                element: <GroupPage />,
              },
              {
                path: routePaths.groupDetail,
                element: <GroupDetail />,
              },
            ],
          },
        ],
      },
      {
        element: <VideosLayout />,
        children: [
          {
            element: <PrivateRoute />,
            children: [
              {
                path: routePaths.videos,
                Component: VideosPage,
              },
              {
                path: routePaths.videoDetail,
                element: <VideosPage />,
              },
            ],
          },
        ],
      },
      {
        element: <BlankLayout />,
        children: [
          {
            element: <PrivateRoute />,
            children: [
              {
                path: routePaths.profileUser,
                element: <ProfileUserPage />,
              },
              {
                path: routePaths.friend,
                element: <FriendPage />,
              },
              {
                path: routePaths.listFriend,
                element: <ListFriendPage />,
              },

              {
                path: routePaths.userSetting,
                Component: UserSetting,
              },
            ],
          },
        ],
      },
      {
        path: routePaths.notiDonate,
        element: <PageDonateLivestream />,
      },
    ],
  },

  {
    path: routePaths.register,
    Component: RegisterPage,
  },
  {
    path: routePaths.login,
    Component: LoginPage,
  },
  {
    path: routePaths.verifyEmail,
    Component: VerifyAccountPage,
  },
  {
    path: '*',
    Component: NotFoundPage,
  },
]);
