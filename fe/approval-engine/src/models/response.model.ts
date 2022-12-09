export interface ResponseModel<T> {
  payload: T;
  totalCount: number;
  code: string;
  description: string;
  errors: string[];
  hasErrors: boolean;
}

export interface LoginResponseModel {
  access_token: string;
  token_type: string;
  expires_in: number;
  scope: string;
  refresh_token: string;
  error: string;
  error_description: string;
  error_uri: string;
}

export interface PaginationModel {
  rowsNumber: number | undefined;
  sortBy?: string | undefined;
  descending?: boolean | undefined;
  page?: number | undefined;
  rowsPerPage?: number | undefined;
}
