import type { IUser } from "./IUser";
import type { ILoanRequestSimple } from "./ILoanRequestSimple";

export interface ILoanOffer  {
    id?: string;
    appUserId: string;
    appUser?: IUser;
    loanRequestId: string;
    loanRequest?: ILoanRequestSimple;
    amount: number;
    interest: number;
    period: number;
    active: boolean;
    comment: string;
    createdAt: Date;
}