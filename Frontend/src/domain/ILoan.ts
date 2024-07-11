import type ICompanySimple from "./ICompanySimple";
import type { ITransaction } from "./ITransaction";
import type {IUser} from "./IUser";

export interface ILoan {
    id: string;
    companyId: string;
    company: ICompanySimple;
    appUserId: string;
    appUser: IUser;
    amount: number;
    interest: number;
    period: number;
    active: boolean;
    comment: string;
    startDate: Date;
    transactions: ITransaction[];
}