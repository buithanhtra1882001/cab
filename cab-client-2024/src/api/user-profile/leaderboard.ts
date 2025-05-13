import { API } from '..';
import { LeaderboardData } from '../../models/leaderboard.model';

export const fetchLeaderboardApi = () => {
  return new Promise<LeaderboardData>((resolve, reject) => {
    API.get<LeaderboardData>(`/v1/user-service/users/leaderboard`)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
export const fetchLeaderboardUserApi = (userId?: any) => {
  return new Promise<LeaderboardData>((resolve, reject) => {
    API.get<LeaderboardData>(`/v1/post-service/donatepost/statistical-donate?UserId=${userId}`)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
