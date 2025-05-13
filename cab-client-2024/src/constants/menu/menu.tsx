import React from 'react';
import { Bookmark, ChevronsUp, TrendingUp } from 'lucide-react';
import { routePaths } from '../../routes/routes-path';
import HomeIcon from '../../assets/icons/home.png';
import ChatIcon from '../../assets/icons/chat.png';
import VideoIcon from '../../assets/icons/video.png';
import DatingIcon from '../../assets/icons/dating.png';
import FriendsIcon from '../../assets/icons/friends.png';
import CreatorIcon from '../../assets/icons/creator.png';
import GroupIcon from '../../assets/icons/group.png';

export interface NavbarItem {
  id: string | number;
  name: string;
  icon?: React.ReactNode;
  link: string;
}

export const navbar: NavbarItem[] = [
  {
    id: 1,
    name: 'Bảng tin',
    link: routePaths.home,
    icon: <img src={HomeIcon} alt="feeds" className="w-5" />,
  },
  {
    id: 2,
    name: 'Chat',
    link: routePaths.chat,
    icon: <img src={ChatIcon} alt="feeds" className="w-5" />,
  },
  {
    id: 3,
    name: 'Video',
    link: '/videos',
    icon: <img src={VideoIcon} alt="feeds" className="w-6" />,
  },
  {
    id: 5,
    name: 'Hẹn hò',
    link: '/dating',
    icon: <img src={DatingIcon} alt="feeds" className="w-6" />,
  },
  {
    id: 6,
    name: 'Bạn bè',
    link: routePaths.friend,
    icon: <img src={FriendsIcon} alt="feeds" className="w-6" />,
  },
  {
    id: 7,
    name: 'Group',
    link: '/group',
    icon: <img src={GroupIcon} alt="feeds" className="w-6" />,
  },
  {
    id: 8,
    name: 'Gợi ý Creator',
    link: '/creators/request',
    icon: <img src={CreatorIcon} alt="feeds" className="w-6" />,
  },
];
export const navbarVideos: NavbarItem[] = [
  {
    id: 1,
    name: 'Trang chủ',
    link: routePaths.home,
    icon: <TrendingUp size={18} />,
  },
  {
    id: 2,
    name: 'Video trending',
    link: '/dating',
    icon: <TrendingUp size={18} />,
  },
  {
    id: 3,
    name: 'Video mới',
    link: routePaths.friend,
    icon: <ChevronsUp size={18} />,
  },
  {
    id: 4,
    name: 'Video đã lưu',
    link: '/creators/request',
    icon: <Bookmark size={18} />,
  },
];
