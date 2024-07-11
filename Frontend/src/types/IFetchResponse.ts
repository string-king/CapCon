export interface IFetchResponse<TData> {
    ok: boolean;
    statusCode: number;
    data?: TData;
    message?: string;
}
