export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
}

export interface Login {
  username: string;
  password: string;
}
