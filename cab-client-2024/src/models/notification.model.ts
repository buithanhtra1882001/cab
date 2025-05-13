export interface INotificationModel {
  id: string;
  actor: any;
  createdAt: string;
  notificationType: string;
  message: string;
  referenceId: string;
}

export type FetchNotificationParams = {
  pageNumber?: number;
  pageSize?: number;
};
