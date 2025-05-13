export interface UserFollow {
  userId: string;
  avatar: string;
  userName: string;
  totalFollow: number;
}

export interface UserDonate {
  userId: string;
  avatar: string;
  userName: string;
  totalAmount: number;
}

export interface UserReceiveDonate {
  userId: string;
  avatar: string;
  userName: string;
  totalAmount: number;
}

export interface LeaderboardData {
  userFollows: UserFollow[];
  userDonates: UserDonate[];
  userRecieveDonates: UserReceiveDonate[];
}
