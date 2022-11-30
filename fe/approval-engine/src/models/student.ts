export interface Student {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  created: string;
  modified: string;
  status: string;
}

export interface NewStudent {
  firstName: string;
  lastName: string;
  email: string;
}

export interface GetStudentsQuery {
  pageIndex: number;
  pageSize: number;
}
