import { defineStore } from 'pinia';
import { Login, User } from 'src/models/user';
import AuthService from '../services/auth.service';
import JWT from 'jwt-decode';
import { Toast } from 'src/services/toast.helper';

export const useAuthStore = defineStore('user', {
  state: () => ({
    users: [],
    user: {} as User,
    token: '',
    loading: false,
    error: '',
  }),
  getters: {
    getUserProfile: (state) => {
      if (state.user)
        state.user = JSON.parse(localStorage.getItem('user') as string);
      return state.user;
    },
  },
  actions: {
    async login(payload: Login) {
      this.user = {} as User;
      this.loading = true;
      try {
        const response = await AuthService.Login(payload);
        if (!response.error) {
          this.token = response.access_token;
          this.user = JWT(response.access_token);

          localStorage.setItem('user', JSON.stringify(this.user));
          localStorage.setItem('token', response.access_token);
          localStorage.setItem('refresh_token', response.refresh_token);
          localStorage.setItem('expiry_date', (Date.now() + 3600).toString());
        } else {
          this.error = response.error;
          Toast(response.error);
        }
      } catch (error: any) {
        this.error = error as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },
    logout() {
      this.user = {} as User;
      localStorage.removeItem('user');
    },
  },
});
