import { api } from 'boot/axios';
import { LoginResponseModel } from 'src/models/response.model';
import { Login } from '../models/user';

export default class AuthService {
  static async Login(payload: Login) {
    const requestOptions = {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
    };

    const formPayload = new URLSearchParams();
    formPayload.append('grant_type', payload.grant_type);
    formPayload.append('password', payload.password);
    formPayload.append('username', payload.username);

    return (
      await api.post<LoginResponseModel>(
        '/connect/token',
        formPayload,
        requestOptions
      )
    ).data;
  }

  static async RefreshAccessToken(): Promise<string> {
    const refresh_token = localStorage.getItem('refresh_token');

    const requestOptions = {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
    };

    const formPayload = new URLSearchParams();
    formPayload.append('grant_type', 'refresh_token');
    formPayload.append('refresh_token', refresh_token as string);

    const response = (
      await api.post<LoginResponseModel>(
        '/connect/token',
        formPayload,
        requestOptions
      )
    ).data;

    //localStorage.setItem('user', JSON.stringify(this.user));
    localStorage.setItem('token', response.access_token);
    localStorage.setItem('refresh_token', response.refresh_token);
    return response.access_token;
  }
}