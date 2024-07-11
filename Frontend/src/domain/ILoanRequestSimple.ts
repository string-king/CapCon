import type ICompanySimple from "./ICompanySimple";

export interface ILoanRequestSimple {
    id?: string;
    companyId: string;
    company?: ICompanySimple;
    amount: number;
    interest: number;
    period: number;
    active: boolean;
    comment: string;
    createdAt: Date;
}