export interface IHashTag {
  slug: string;
  name: string;
  description: string;
  isActived: boolean;
  point: number;
}

export interface ISearchHashtagPayload {
  keyword: string;
  totalRecord: number;
}
