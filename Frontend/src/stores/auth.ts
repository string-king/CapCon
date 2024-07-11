import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { stringOrNull } from '@/types/types';
import AccountService from '@/services/AccountService';
import type { IUserInfo } from '@/types/IUserInfo';
import { useRouter } from 'vue-router';
import router from '@/router';

export const useAuthStore = defineStore('auth', () => {
  // state variables
  const jwt = ref<stringOrNull>(localStorage.getItem('jwt'));
  const refreshToken = ref<stringOrNull>(localStorage.getItem('refreshToken'));
  const userName = ref<stringOrNull>(localStorage.getItem('userName'));
  const userId = ref<stringOrNull>(localStorage.getItem('userId'));
  const isRefreshing = ref<boolean>(false);

  // computed getters
  const isAuthenticated = computed<boolean>(() => !!jwt.value);
  const getUserId = computed<string>(() => userId.value?.toString() || '');

  // actions
  const login = async (username: string, password: string) => {
    const response = await AccountService.loginAsync(username, password);
    if (response.data) {
      setUserData(response.data);
      return null;
    }
    return response.errors;
  };

  const logout = async () => {
    if (!jwt.value || !refreshToken.value) return;
    try {
      const response = await AccountService.logoutAsync(jwt.value, refreshToken.value);
      if (response.data) {
        clearUserData();
        router.push('/');
      } else {
        clearUserData();
        console.error('An error occurred during logout:', response.errors);
      }
    } catch (error: any) {
      clearUserData();
      console.error('An error occurred during logout:', error);
    }
  };

  const refreshTokenData = async () => {
    if (isRefreshing.value || !jwt.value || !refreshToken.value) return;
    isRefreshing.value = true;
    const response = await AccountService.refreshTokenAsync(jwt.value, refreshToken.value, 3600);
    if (response.data) {
      console.log('Refreshed token data:', response.data);
      setUserData(response.data);
    } else {
      clearUserData();
      console.error('An error occurred during token refresh:', response.errors);
    }
    isRefreshing.value = false;
  };

  const register = async (email: string, password: string, firstName: string, lastName: string) => {
    const response = await AccountService.registerAsync(email, password, firstName, lastName);
    if (response.data) {
      setUserData(response.data);
      return null;
    } else {
      return response.errors;
    }
  };

  const setUserData = (data: IUserInfo) => {
    jwt.value = data.jwt;
    refreshToken.value = data.refreshToken;
    userId.value = data.userId;
    userName.value = data.firstName + ' ' + data.lastName;
    localStorage.setItem('jwt', jwt.value!);
    localStorage.setItem('refreshToken', refreshToken.value!);
    localStorage.setItem('userId', data.userId);
    localStorage.setItem('userName', userName.value);
  };

  const clearUserData = () => {
    jwt.value = null;
    refreshToken.value = null;
    userName.value = null;
    localStorage.removeItem('jwt');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('userName');
  };

  return { jwt, refreshToken, userId, userName, isAuthenticated, isRefreshing, login, logout, refreshTokenData, register, getUserId }
});
