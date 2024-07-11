import type ICompanySimple from "./ICompanySimple";
import type { ILoanOffer } from "./ILoanOffer";

export interface ILoanRequest  {
    id?: string;
    companyId: string;
    company: ICompanySimple;
    amount: number;
    interest: number;
    period: number; 
    active: boolean;
    comment: string;
    createdAt: Date;
    loanOffers: ILoanOffer[];
}