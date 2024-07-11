import type { IUserInfo } from "@/types/IUserInfo";
import axios from "axios";
import type { IResultObject } from "@/types/IResultObject";
import { useAuthStore } from '@/stores/auth'
import { ApiBaseUrl} from '@/configuration'

export default class AccountService {

    private static URL = ApiBaseUrl + 'identity/account/';

    
    private static httpClient = axios.create({
        baseURL: AccountService.URL,
    });

    static async loginAsync(email: string, password: string): Promise<IResultObject<IUserInfo>> {
        const loginData = { email, password };
        try {
            const response = await AccountService.httpClient.post<IUserInfo>("login", loginData);
            return { data: response.data };
        } catch (error: any) {
            return { errors: [error.response?.data.error || error.message] };
        }
    }

    static async logoutAsync(jwt: string, refreshToken: string): Promise<IResultObject<any>> {
        const logoutData = { refreshToken };
        try { 
            const response = await AccountService.httpClient.post<any>("logout", logoutData, {
                headers: { 'Authorization': `Bearer ${jwt}` }
            });
            return { data: response.data };
        } catch (error: any) {
            const authS = useAuthStore();
            if (error.response && error.response.status === 401 && authS.isAuthenticated) {
                if (!authS.isRefreshing) {
                    await authS.refreshTokenData();
                    return AccountService.logoutAsync(authS.jwt!, authS.refreshToken!);
                }
            }
            return { errors: [error.response?.data.error || error.message] };
        }
    }

    static async refreshTokenAsync(jwt: string, refreshToken: string, expiresInSeconds: number): Promise<IResultObject<IUserInfo>> {
        const refreshTokenData = { jwt, refreshToken };
        try {
            const response = await AccountService.httpClient.post<IUserInfo>(
                `RefreshTokenData?expiresInSeconds=${expiresInSeconds}`,
                refreshTokenData,
                { headers: { 'Authorization': `Bearer ${jwt}` } }
            );
            return { data: response.data };
        } catch (error: any) {
            return { errors: [error.response?.data.error || error.message] };
        }
    }

    static async registerAsync(email: string, password: string, firstName: string, lastName: string): Promise<IResultObject<IUserInfo>> {
        const registerData = { email, password, firstName, lastName };
        try {
            const response = await AccountService.httpClient.post<IUserInfo>("register", registerData);
            return { data: response.data };
        } catch (error: any) {
            return { errors: [error.response?.data.error || error.message] };
        }
    }
}
