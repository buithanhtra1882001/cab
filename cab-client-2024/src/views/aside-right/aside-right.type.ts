import { ISlideItem } from '../../components';

export interface IAsideRightContentModel {
  slides?: ISlideItem[];
  hashtags?: string[];
  leaderboard?: ILeaderboardModel[];
}

export interface ILeaderboardModel {
  id: string | number;
  name: string;
  avatar?: string;
  link?: string;
}
