export interface IChatListProps {
  userList?: IUserchat[];
  onUserSelect?: (user: IUserchat | null) => void;
}

export interface IChatProps {
  selectedUser?: IUserchat;
  onClose?: () => void;
}

export interface IUserchat {
  userId?: string;
  url?: string;
  name?: string;
}
