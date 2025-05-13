export interface IAnnouncementModel {
  id?: string;
  content?: string;
  createdAt?: string;
  endAt?: string;
  startAt?: string;
  lastModifiedAt?: string;
  priority?: number;
  status?: string;
}

export interface IAnnouncementStore {
  announcements: IAnnouncementModel[] | undefined;
  loadingProcessId: string;
  localErrorId: string;
}
