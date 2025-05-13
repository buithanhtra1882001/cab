export type Size = 'xs' | 'sm' | 'md';

export interface IDataResponse<T> {
  statusCode: number;
  message: string;
  data: T;
}
