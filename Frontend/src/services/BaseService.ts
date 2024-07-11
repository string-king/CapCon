import Axios, { AxiosError, type AxiosRequestConfig } from 'axios';
import { ApiBaseUrl } from '@/configuration';
import type { IFetchResponse } from '@/types/IFetchResponse';
import type { IMessage } from '@/types/IMessage';
import { useAuthStore } from '@/stores/auth';
import AccountService from './AccountService';

export abstract class BaseService {
    protected static axios = Axios.create({
        baseURL: ApiBaseUrl,
        headers: {
            'Content-Type': 'application/json',
        },
    });

    static initializeInterceptors() {
        this.axios.interceptors.request.use(
            async (config) => {
                const authStore = useAuthStore();
                if (authStore.jwt) {
                    config.headers['Authorization'] = `Bearer ${authStore.jwt}`;
                }
                return config;
            },
            (error) => {
                return Promise.reject(error);
            }
        );

        this.axios.interceptors.response.use(
            (response) => {
                return response;
            },
            async (error) => {
                const originalRequest = error.config;
                const authStore = useAuthStore();

                if (error.response && error.response.status === 401 && authStore.isAuthenticated && !originalRequest._retry) {
                    if (!authStore.isRefreshing) {
                        originalRequest._retry = true;
                        try {
                            await authStore.refreshTokenData();
                            originalRequest.headers['Authorization'] = `Bearer ${authStore.jwt}`;
                            return this.axios(originalRequest);
                        } catch (refreshError) {
                            await authStore.logout();
                            return Promise.reject(refreshError);
                        }
                    }
                }

                return Promise.reject(error);
            }
        );
    }

    protected static getAxiosConfiguration(token: string | undefined): AxiosRequestConfig | undefined {
        if (!token) return undefined;
        const config: AxiosRequestConfig = {
            headers: {
                Authorization: 'Bearer ' + token,
            },
        };
        return config;
    }

    static async getAll<TEntity>(apiEndpoint: string, token?: string): Promise<IFetchResponse<TEntity[]>> {
        try {
            const response = await this.axios.get<TEntity[]>(apiEndpoint, BaseService.getAxiosConfiguration(token));
            return {
                ok: response.status <= 299,
                statusCode: response.status,
                data: response.data,
            };
        } catch (err) {
            const error = err as AxiosError;
            return {
                ok: false,
                statusCode: error.response?.status ?? 0,
                message: error.message,
            };
        }
    }

    static async get<TEntity>(apiEndpoint: string, token?: string): Promise<IFetchResponse<TEntity>> {
        try {
            const response = await this.axios.get<TEntity>(apiEndpoint, BaseService.getAxiosConfiguration(token));
            return {
                ok: response.status <= 299,
                statusCode: response.status,
                data: response.data,
            };
        } catch (err) {
            const error = err as AxiosError;
            return {
                ok: false,
                statusCode: error.response?.status ?? 0,
                message: error.message,
            };
        }
    }

    static async create<TReturnEntity, TBodyEntity = TReturnEntity>(body: TBodyEntity, apiEndpoint: string, token?: string): Promise<IFetchResponse<TReturnEntity>> {
        try {
            const response = await this.axios.post<TReturnEntity>(
                apiEndpoint,
                JSON.stringify(body),
                BaseService.getAxiosConfiguration(token)
            );

            return {
                ok: response.status <= 299,
                statusCode: response.status,
                data: response.data,
            };
        } catch (err) {
            const error = err as AxiosError;
            return {
                ok: false,
                statusCode: 0,
                message: error.message,
            };
        }
    }

    static async edit<TEntity>(body: TEntity, apiEndpoint: string, token?: string): Promise<IFetchResponse<TEntity>> {
        try {
            const response = await this.axios.put<TEntity>(
                apiEndpoint,
                JSON.stringify(body),
                BaseService.getAxiosConfiguration(token)
            );

            return {
                ok: response.status <= 299,
                statusCode: response.status,
                data: response.data,
            };
        } catch (err) {
            const error = err as AxiosError;
            return {
                ok: false,
                statusCode: 0,
                message: error.message,
            };
        }
    }

    static async delete(apiEndpoint: string, token?: string): Promise<IFetchResponse<IMessage>> {
        try {
            const response = await this.axios.delete(apiEndpoint, BaseService.getAxiosConfiguration(token));

            return {
                ok: response.status <= 299,
                statusCode: response.status,
                message: response.statusText,
            };
        } catch (r) {
            const reason = r as AxiosError;
            return {
                ok: false,
                statusCode: 0,
                message: reason.message,
            };
        }
    }
}

// Initialize interceptors when the service is imported
BaseService.initializeInterceptors();
