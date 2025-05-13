import _get from 'lodash/get';

export interface IRouteActionError {
  message: string;
  error: string;
  statusCode: number;
}

export const errorResponse = (error: unknown): IRouteActionError => {
  const result: IRouteActionError = {
    message: _get(error, 'response.data.error.message', ''),
    error: _get(error, 'response.data.error.error', ''),
    statusCode: _get(error, 'response.status', 400),
  };

  return result;
};
