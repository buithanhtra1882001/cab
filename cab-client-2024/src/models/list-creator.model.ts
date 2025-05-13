export interface IListCreatorModel {
  userId?: string;
  fullName?: string;
  avatar?: string;
  status?: 'ACCEPTED' | 'DELETE' | 'PENDING';
}

export type FetchListCreatorParams = {
  pageNumber?: number;
  pageSize?: number;
};
