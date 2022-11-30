import { defineStore } from 'pinia';
import { Login } from 'src/models/user';
import AuthService from '../services/auth.service';
import JWT from 'jwt-decode';

export const useAuthStore = defineStore('user', {
  state: () => ({
    users: [],
    user: null,
    token: '',
    loading: false,
    error: '',
  }),
  getters: {
    getUserProfile: (state) => {
      return state.user;
    },
  },
  actions: {
    async login(payload: Login) {
      this.user = null;
      this.loading = true;
      try {
        const response = await AuthService.login(payload);
        if (!response.error) {
          this.token = response.access_token;
          this.user = JWT(response.access_token);

          localStorage.setItem('user', JSON.stringify(this.user));
          localStorage.setItem('token', response.access_token);
          localStorage.setItem('refresh_token', response.refresh_token);
        } else {
          this.error = response.error;
        }
      } catch (error) {
        this.error = error as string;
      } finally {
        this.loading = false;
      }
    },
    logout() {
      this.user = null;
      localStorage.removeItem('user');
    },
  },
});
